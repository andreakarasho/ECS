using System;
using System.Diagnostics;

namespace ActionGame.ECS
{
    public class CaseTesting
    {
        public CaseTesting()
        {
            int testSize = 1000000;

            Console.WriteLine("Testing ECS vs normal structure w/ " + testSize + " samples:");

            CostlyOperationObject[] cossers = new CostlyOperationObject[testSize];
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < testSize; i++)
            {
                cossers[i] = new CostlyOperationObject(i);
            }
            watch.Stop();
            Console.WriteLine("Normal Structure Time: " + watch.ElapsedMilliseconds + "ms");
            
            watch = new Stopwatch();
            watch.Start();
            World world = new World();
            for (int i = 0; i < testSize; i++)
            {
                world.AddEntity(new Component[] { new CostlyOperationECSComponent(i) }) ;
            }
            watch.Stop();
            Console.WriteLine("ECS Initialization Time: " + watch.ElapsedMilliseconds + "ms");

            watch = new Stopwatch();
            watch.Start();
            Query query = new Query(Query.Type.SUPERSETSOF, new byte[] { 2 });
            watch.Stop();
            Console.WriteLine("Query Initialization Time: " + watch.ElapsedMilliseconds + "ms");

            watch = new Stopwatch();
            watch.Start();
            foreach (Archetype arc in query) //This should be abstracted as a System in a real implementation
            {
                int acosCol = arc.componentColumns[2];
                for (int i = 0; i < arc.entityCount; i++)
                {
                    CostlyOperationECSComponent s = arc.componentData[acosCol][i] as CostlyOperationECSComponent;
                    for(int j = 0; j < 20; j++)
                    {
                        s.res[j] = MathF.Atan(s.start) / MathF.Sqrt(s.start);
                        s.start += .02f;
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("ECS Time: " + watch.ElapsedMilliseconds + "ms");
        }

        private class CostlyOperationObject
        {
            private float[] res;
            public CostlyOperationObject(float start)
            {
                res = new float[100];
                for(int i = 0; i < 100; i++)
                {
                    res[i] = MathF.Atan(start) / MathF.Sqrt(start);
                    start += .02f;
                }
            }
        }

        public class CostlyOperationECSComponent : Component
        {
            public const int INDEX = 2;
            public float[] res;
            public float start;

            public CostlyOperationECSComponent(float start)
            {
                componentType = INDEX;
                this.start = start;
                res = new float[20];
            }
        }
    }
}
