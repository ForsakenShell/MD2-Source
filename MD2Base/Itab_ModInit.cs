using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Itab_ModInit  : ITab
    {
        public Itab_ModInit()
        {
            GameObject obj = new GameObject("MD2BaseModController");
            obj.AddComponent<MD2.ModController>();
            Log.Message("Initialised MD2 base module");
        }

        protected override void FillTab()
        {
            return;
        }
    }
}
