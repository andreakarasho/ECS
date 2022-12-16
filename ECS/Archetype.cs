using System.Collections.Generic;

namespace ActionGame.ECS
{
    public class Archetype
    {
        public int entityCount;
        public int componentCount;
        public Signature signature;
        public List<Component>[] componentData;
        public Dictionary<byte, int> componentColumns = new Dictionary<byte, int>();
        public List<bool> nullRows = new List<bool>();
        private Stack<int> availEntityRows = new Stack<int>();

        public Archetype(Signature signature)
        {
            componentCount = signature.components.Length;
            this.signature = signature;
            componentData = new List<Component>[componentCount];
            for(int i = 0; i < componentCount; i++)
            {
                componentColumns.Add((byte)(signature.components[i] - 10), i);
                componentData[i] = new List<Component>();
            }
        }
        public int AddEntity(Component[] components) //returns entity row
        {
            entityCount++;
            if (availEntityRows.Count > 0)
            { //assigns to available empty pool row
                int row = availEntityRows.Pop();
                nullRows[row] = false;
                for(int i = 0; i < componentCount; i++)
                {
                    componentData[i][row] = components[i];
                }
                return row;
            }
            else
            { //appends row to component matrix
                nullRows.Add(false);
                for (int i = 0; i < componentCount; i++)
                {
                    componentData[i].Add(components[i]);
                }
                return entityCount - 1;
            }
        }
        public void RemoveComponentsAtRow(int row)
        {
            availEntityRows.Push(row);
            nullRows[row] = true;
            for (int i = 0; i < componentCount; i++)
            {
                componentData[i][row] = null;
            }
        }
    }
}