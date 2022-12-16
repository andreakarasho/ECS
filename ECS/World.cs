using System.Collections.Generic;

namespace ActionGame.ECS
{
    //TODO: query caching function, iterating over components in query
    public class World
    {
        public static World main;

        private EntityManager entityManager;

        //map of entities to associated archetypes
        private Dictionary<Entity, Archetype> entityArchetypes
            = new Dictionary<Entity, Archetype>();

        //map of entities to row of component data within archetype
        private Dictionary<Entity, int> entityRows
            = new Dictionary<Entity, int>();

        //map of component set signatures to associated archetype
        public Dictionary<uint, Archetype> componentArchetypes 
            = new Dictionary<uint, Archetype>();

        public Dictionary<uint, List<uint>> signatureSuperSets
            = new Dictionary<uint, List<uint>>();
        private List<Signature> uniqueSignatures = new List<Signature>();

        public World()
        {
            main = this;
            entityManager = new EntityManager();
        }

        public Entity AddEntity(Component[] components) //returns entity
        { //TODO: make so component array mustnt be ordered. use separate indexing dictionary in archetype
            uint entity = entityManager.CreateEntityIndex();
            byte[] componentIDs = new byte[components.Length];
            for (int i = 0; i < components.Length; i++) componentIDs[i] = components[i].componentType;
            Signature signature = new Signature(componentIDs);
            Archetype entityArchetype;
            if (componentArchetypes.ContainsKey(signature.id))
            { //reference existing archetype
                entityArchetype = componentArchetypes[signature.id];
            }
            else
            { //archetype does not exist, create it
                entityArchetype = new Archetype(signature);
                componentArchetypes.Add(signature.id, entityArchetype);

                //updating signature superset map
                uniqueSignatures.Add(signature);
                signatureSuperSets.Add(signature.id, new List<uint>());
                string sig = signature.id.ToString();
                for(int i = 0; i < uniqueSignatures.Count - 1; i++)
                {
                    string compSig = uniqueSignatures[i].id.ToString();

                    #region SupersetCheck
                    if(sig.Length < compSig.Length)
                    {
                        int lo = 0;
                        int hi = compSig.Length - 1;
                        while (lo < hi)
                        {
                            if (compSig[lo] == sig[0] && compSig[hi] == sig[sig.Length - 1])
                            {
                                signatureSuperSets[signature.id].Add(uniqueSignatures[i].id);
                                break;
                            }
                            if (compSig[lo] != sig[0]) lo++;
                            if (compSig[hi] != sig[sig.Length - 1]) hi--;
                        }
                    }
                    #endregion

                    #region SubsetCheck
                    if(sig.Length > compSig.Length)
                    {
                        int lo = 0;
                        int hi = sig.Length - 1;
                        while (lo < hi)
                        {
                            if (sig[lo] == compSig[0] && sig[hi] == compSig[sig.Length - 1])
                            {
                                signatureSuperSets[uniqueSignatures[i].id].Add(signature.id);
                                break;
                            }
                            if (sig[lo] != compSig[0]) lo++;
                            if (sig[hi] != compSig[compSig.Length - 1]) hi--;
                        }
                    }
                    #endregion
                }
            }

            Entity e = new Entity(entity);
            entityArchetypes.Add(e, entityArchetype);
            int entityRow = entityArchetype.AddEntity(components);
            entityRows.Add(e, entityRow);
            return e;
        }

        public void RemoveEntity(Entity entity)
        {
            Archetype entityArchetype = entityArchetypes[entity];
            int entityRow = entityRows[entity];
            entityArchetypes.Remove(entity);
            entityRows.Remove(entity);
            entityArchetype.RemoveComponentsAtRow(entityRow);
        }
    }
}
