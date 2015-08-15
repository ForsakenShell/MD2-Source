using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class Keys
    {
        public static KeyBindingDef Named(string defName)
        {
            return DefDatabase<KeyBindingDef>.GetNamed(defName);
        }

        public static KeyBindingDef BuildingOnOffToggle
        {
            get
            {
                return Keys.Named("BuildingOnOffToggle");
            }
        }

        public static KeyBindingDef DeactivateDroid
        {
            get
            {
                return Keys.Named("DeactivateDroid");
            }
        }

        public static KeyBindingDef FissureGeneratorChangeFissure
        {
            get
            {
                return Keys.Named("FissureGeneratorChangeFissure");
            }
        }
    }
}
