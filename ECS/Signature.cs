using System;

namespace ActionGame.ECS
{
    public class Signature : IEquatable<Signature>
    {
        public uint id;
        public byte[] components;

        public Signature(byte[] components)
        {
            Array.Sort(components);
            this.components = components;
            for(int i = 0; i < components.Length; i++)
            {
                if (components[i] > 89) throw new Exception("Component id out of bounds. Cannot exceed 89");
                components[i] += 10;
                id *= 100;
                id += components[i];
            }
        }
        public bool Equals(Signature other)
        {
            return other.id == id;
        }
    }
}
