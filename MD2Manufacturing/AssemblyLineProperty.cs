using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class AssemblyLineProperty: IExposable
    {
        private float value;
        private string label = "";

        public AssemblyLineProperty(string label, float initialValue)
        {
            this.value = initialValue;
            this.label = label;
        }
        public AssemblyLineProperty(string label):this(label, 1f)
        {

        }
        public AssemblyLineProperty():this("Unnamed",1f)
        {

        }

        public string Label
        {
            get
            {
                return label;
            }
        }

        public static implicit operator float(AssemblyLineProperty p)
        {
            return p.Value;
        }

        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                if (this.value <= 0) this.value = 0.01f;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.value, "value", 1f);
            Scribe_Values.LookValue(ref this.label, "label");
        }
    }
}
