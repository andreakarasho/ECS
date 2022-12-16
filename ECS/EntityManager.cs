using System;
using System.Collections.Generic;

namespace ActionGame.ECS
{
    public class EntityManager
    {
        public const uint MAX_ENTITIES = 5000000;
        private Stack<uint> recycleIndicies = new Stack<uint>();
        private uint entityIndex;
        private uint entityCount;
        public EntityManager()
        {
        }
        public uint CreateEntityIndex()
        {
            if (recycleIndicies.Count == 0 && entityIndex >= MAX_ENTITIES) 
                throw new Exception("Too many entities");
            entityCount++;
            if (recycleIndicies.Count > 0) return recycleIndicies.Pop();
            entityIndex++;
            return entityIndex - 1;
        }
        public void FreeEntityIndex(Entity entity)
        {
            recycleIndicies.Push(entity.entityId);
            entityCount--;
        }
    }
}
