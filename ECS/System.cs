using System;
using System.Collections.Generic;
using System.Text;

namespace ActionGame.ECS
{
    /*
     * Update-Draw System
     * Abstract class for systems that get Update and Draw calls
     */
    public abstract class System
    {
        public abstract void Update();
        public abstract void Draw();
    }
}
