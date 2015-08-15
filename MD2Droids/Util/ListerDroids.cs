using Backstories;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class ListerDroids : MapComponent
    {
        public static ListerDroids listerDroids;
        private List<Droid> allDroids = new List<Droid>();
        private List<Thing> chargers = new List<Thing>();
        private List<Building_RepairStation> repairStations = new List<Building_RepairStation>();

        public ListerDroids()
        {
            ListerDroids.listerDroids = this;
        }

        public static List<Droid> AllDroids
        {
            get
            {
                return listerDroids.allDroids;
            }
        }

        public static List<Thing> AllChargers
        {
            get
            {
                return listerDroids.chargers;
            }
        }

        public static List<Building_RepairStation> AllRepairStations
        {
            get
            {
                return listerDroids.repairStations;
            }
        }

        public static IEnumerable<Thing> AllRepairStationsThings
        {
            get
            {
                foreach (var rps in listerDroids.repairStations)
                    yield return (Thing)rps;
            }
        }

        public static string GetNumberedName()
        {
            string name = string.Empty;
            int num = 1;
            do
            {
                name = "Droid " + num.ToString();
                num++;
            } while (HaveDroidNamed(name));
            return name;
        }

        public static bool HaveDroidNamed(string name)
        {
            foreach (var d in (from t in ListerDroids.AllDroids
                select t.story.name.nick))
            {
                if (d == name) return true;
            }
            return false;
        }

        public static void RegisterDroid(Droid droid)
        {
            if (!listerDroids.allDroids.Contains(droid))
                listerDroids.allDroids.Add(droid);
        }

        public static void DeregisterDroid(Droid droid)
        {
            if (listerDroids.allDroids.Contains(droid))
                listerDroids.allDroids.Remove(droid);
        }

        public static void RegisterCharger(Thing charger)
        {
            if (charger != null && charger.TryGetComp<CompDroidCharger>() != null && !listerDroids.chargers.Contains(charger))
            {
                listerDroids.chargers.Add(charger);
            }
        }

        public static void DeregisterCharger(Thing charger)
        {
            if (charger != null && listerDroids.chargers.Contains(charger))
            {
                listerDroids.chargers.Remove(charger);
            }
        }

        public static void RegisterRepairStation(Building_RepairStation rps)
        {
            if (rps != null && !listerDroids.repairStations.Contains(rps))
                listerDroids.repairStations.Add(rps);
        }

        public static void DeregisterRepairStation(Building_RepairStation rps)
        {
            if (rps != null && listerDroids.repairStations.Contains(rps))
                listerDroids.repairStations.Remove(rps);
        }

        public static Thing ClosestChargerFor(ICharge droid, float distance = 9999f)
        {
            Predicate<Thing> pred = (Thing thing) => { return thing.TryGetComp<CompDroidCharger>() != null && thing.TryGetComp<CompDroidCharger>().IsAvailable(droid); };
            return GenClosest.ClosestThing_Global_Reachable(droid.Parent.Position, ListerDroids.AllChargers.AsEnumerable(), PathEndMode.OnCell, TraverseParms.For(droid.Parent), distance, pred);
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public static Building_RepairStation ClosestRepairStationFor(Pawn pawn, Thing t)
        {
            Predicate<Thing> pred = (Thing thing) =>
            {
                Building_RepairStation rps = thing as Building_RepairStation;
                if (rps != null)
                    return rps.IsAvailableForReactivation;
                return false;
            };
            return (Building_RepairStation)GenClosest.ClosestThing_Global_Reachable(t.Position, ListerDroids.AllRepairStationsThings, PathEndMode.InteractionCell, TraverseParms.For(pawn), 9999f, pred);
        }
    }
}
