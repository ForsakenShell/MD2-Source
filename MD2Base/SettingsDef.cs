using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class SettingsDef : Def
    {
        //ManufacturingPlant settings
        public List<ListItem> BuildingCost;
        public int buildingLengthInt;
        public int godModeBuildingLengthInt;
        public int constructionPowerUsage;
        public int productionPowerUsage;
        public int idlePowerUsage;
        public bool instaBuild = false;

        //Manufacturing plant manager settings
        public int maximumAssemblyLines;

        //Droid settings
        public int healDelayTicks;
        public int skillLevel = 5;
        public RimWorld.Passion passion = RimWorld.Passion.None;

        //Cart settings
        public bool drawTrackDebug = false;
    }
}
