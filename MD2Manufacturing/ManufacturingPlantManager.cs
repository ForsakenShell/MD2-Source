using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class MPmanager : MapComponent
    {
        public static SettingsDef Settings = DefDatabase<SettingsDef>.GetNamed("MD2MPManagerSettings");
        private List<AssemblyLine> assemblyLines = new List<AssemblyLine>();
        public static MPmanager manager;
        private bool needSetup = true;

        public MPmanager()
        {
            MPmanager.manager = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            MPmanager.manager = this;
            Scribe_Collections.LookList<AssemblyLine>(ref this.assemblyLines, "assemblyLines", LookMode.Deep, null);
        }

        public ManufacturingControlConsole console
        {
            get
            {
                if (Find.ListerBuildings.AllBuildingsColonistOfClass<ManufacturingControlConsole>().Count() > 0)
                    return Find.ListerBuildings.AllBuildingsColonistOfClass<ManufacturingControlConsole>().First();
                else
                    return null;
            }
        }


        public override void MapComponentTick()
        {
            if (console == null) return;

            if (needSetup)
            {
                foreach (AssemblyLine line in assemblyLines)
                {
                    line.SpawnSetup();
                }
                needSetup = false;
            }
            base.MapComponentTick();

            if (console != null && console.Power != null && !console.Power.PowerOn) return;

            foreach (AssemblyLine line in assemblyLines)
            {
                line.Tick();
            }
        }

        public List<AssemblyLine> AssemblyLines
        {
            get
            {
                return this.assemblyLines;
            }
        }

        public void RemoveAssemblyLine(AssemblyLine line)
        {
            line.Delete();
            this.assemblyLines.Remove(line);
        }

        public void AddNewAssemblyLine(bool instaBuild)
        {
            //Log.Error("Did this!");
            if (CanAddAssemblyLine)
                this.assemblyLines.Add(AssemblyLine.NewAssemblyLine(instaBuild));
        }

        public int PowerUsage
        {
            get
            {
                int i = 0;
                if (manager.assemblyLines.Count <= 0)
                {
                    i = AssemblyLine.Settings.idlePowerUsage;
                }

                foreach (var l in MPmanager.manager.AssemblyLines)
                {
                    i += l.PowerUsage;
                }
                return i*-1;
            }
        }

        public int AssemblyLineLimit
        {
            get
            {
                return Settings.maximumAssemblyLines;
            }
        }

        public bool CanAddAssemblyLine
        {
            get
            {
                return assemblyLines.Count < AssemblyLineLimit;
            }
        }
    }
}
