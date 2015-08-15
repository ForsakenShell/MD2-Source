using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class CremationTarget : IExposable
    {
        private CremationOperationMode mode = CremationOperationMode.Off;
        private int naturalPriority;
        private Predicate<Thing> predicate;
        private string label = "";
        private Droid crematorius;
        public bool OnlyRotten = false;

        public CremationOperationMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        public int NaturalPriority
        {
            get
            {
                return naturalPriority;
            }
        }

        public Predicate<Thing> Accepts
        {
            get
            {
                if (mode == CremationOperationMode.Off) return (Thing c) => false;
                if (mode == CremationOperationMode.Butcher)
                {
                    Predicate<Thing> p = (Thing t) => predicate(t) && (t.TryGetComp<CompRottable>() == null || t.TryGetComp<CompRottable>().Stage == RotStage.Fresh);
                    return p;
                }
                return predicate;

            }
            set
            {
                predicate = value;
                if (predicate == null)
                    predicate = (Thing c) => false;
            }
        }

        public string Label
        {
            get
            {
                return label;
            }
        }

        public CremationTarget(string label, Predicate<Thing> p, int naturalPriority, Droid c)
        {
            this.label = label;
            this.naturalPriority = naturalPriority;
            this.Accepts = p;
            this.crematorius = c;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.mode, "mode", CremationOperationMode.Off);
            Scribe_Values.LookValue(ref this.OnlyRotten, "OnlyRotten");
        }
    }
}
