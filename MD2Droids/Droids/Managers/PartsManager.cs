using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class PartsManager : IExposable
    {
        private Droid parent;
        private Thing PowerCell;
        private float? maxEnergyCached = null;
        //public Thing ShieldGenerator;

        public PartsManager(Droid droid)
        {
            parent = droid;
        }

        public PartsManager(Thing powerCell, Droid droid)
        {
            parent = droid;
            PowerCell = powerCell;
        }

        public void ExposeData()
        {
            Scribe_References.LookReference(ref this.PowerCell, "PowerCell");
            //Scribe_References.LookReference(ref this.ShieldGenerator, "ShieldGenerator");
        }

        public float MaxEnergy
        {
            get
            {
                if (PowerCell == null)
                {
                    Log.Error(parent.ThingID + " had a null power cell.");
                    return 1f;
                }
                if (maxEnergyCached == null)
                {
                    maxEnergyCached = PowerCell.GetStatValue(StatDef.Named("MD2PowerCellMaxStorage"));
                }
                return (float)maxEnergyCached;
            }
        }

        public void ReplacePowerCell(Thing newCell)
        {
            if (newCell == null)
            {
                Log.Error("Tried to add a new power cell to " + parent.ThingID + " but it was null");
                return;
            }
            bool hasStat = false;
            StatDef sDef = StatDef.Named("MD2PowerCellMaxStorage");
            if (newCell.def.statBases != null)
            {
                for (int i = 0; i < parent.def.statBases.Count; i++)
                {
                    if (newCell.def.statBases[i].stat == sDef)
                    {
                        hasStat = true;
                        break;
                    }
                }
            }
            if (!hasStat)
            {
                Log.Error(newCell.ThingID + " has no power cell stat");
                return;
            }
            if (PowerCell != null)
                GenSpawn.Spawn(PowerCell, parent.Position);
            PowerCell = newCell;
            parent.TotalCharge = newCell.GetStatValue(sDef) * 0.1f;
            if (newCell.SpawnedInWorld)
                newCell.DeSpawn();
        }
    }
}
