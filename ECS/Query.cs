using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ActionGame.ECS
{
    public struct Query /*: IEnumerable<Archetype>*/ //make it enumerate and cast to specific <T extends Component>[]
    {
        public enum Type
        {
            EXACT,
            SUPERSETSOF
        };

        private World world;
        private List<Archetype> archetypes = new List<Archetype>();
        private Type queryType;
        private uint querySignature;

        public Query(World world, Type queryType, byte[] components)
        {
            this.world = world;
            this.queryType = queryType;
            Signature querySignature = new Signature(components);
            this.querySignature = querySignature.id;
            if (!world.signatureSuperSets.ContainsKey(querySignature.id))
                throw new Exception("Invalid component array. Archetype does not exist");

            // Add the "NONE" where you exclude entities with one or more specific components 
            switch (queryType)
            {
                case Type.EXACT:
                    archetypes.Add(world.componentArchetypes[querySignature.id]);
                    break;
                case Type.SUPERSETSOF:
                    List<uint> archetypeIds = world.signatureSuperSets[querySignature.id];
                    archetypes.Add(world.componentArchetypes[querySignature.id]);
                    for (int i = 0; i < archetypeIds.Count; i++)
                    {
                        archetypes.Add(world.componentArchetypes[archetypeIds[i]]);
                    }
                    break;
            }
        }

        public void UpdateQuery()
        {
            switch (queryType)
            {
                case Type.SUPERSETSOF:
                    archetypes.Clear();
                    List<uint> archetypeIds = world.signatureSuperSets[querySignature];
                    archetypes.Add(world.componentArchetypes[querySignature]);
                    for (int i = 0; i < archetypeIds.Count; i++)
                    {
                        archetypes.Add(world.componentArchetypes[archetypeIds[i]]);
                    }
                    break;
            }
        }

        public QueryEnumerator GetEnumerator() => new QueryEnumerator(archetypes);
    }


    public ref struct QueryEnumerator
    {
        private Span<Archetype> _archetypes;
        private int _index;

        public QueryEnumerator(List<Archetype> archetypes)
        {
            _archetypes = CollectionsMarshal.AsSpan(archetypes);
            _index = -1;
        }

        public ref Archetype Current => ref _archetypes[_index];

        public bool MoveNext() => ++_index < _archetypes.Length;

        public void Reset() => _index = -1;
    }
}
