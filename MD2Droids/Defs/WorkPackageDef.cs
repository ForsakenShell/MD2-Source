using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class WorkPackageDef : Def
    {
        public List<WorkTypeDef> workTypes = new List<WorkTypeDef>();
        public float PowerRequirement = 0f;
        public bool displayInMenu = true;
        public Type specialistWorker = null;
        public bool specialist = false;
        public int workAmount = 0;
        public List<ThingCount> packageCost = new List<ThingCount>();

        public static WorkPackageDef Named(string defName)
        {
            return DefDatabase<WorkPackageDef>.GetNamed(defName);
        }

        public string Tooltip
        {
            get
            {
                string s = "";
                s += description;
                s += "\n\n";
                s += "Enables".Translate() + "\n";
                foreach (var d in (from t in workTypes
                    orderby t.labelShort
                    select t).ToList())
                {
                    s += d.labelShort;
                    s += "\n";
                }
                s += "\n";
                s += "PowerUsage".Translate(PowerRequirement.ToString("0.#"));
                return s;
            }
        }
    }
}
