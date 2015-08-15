using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class GraphicDef : Def
    {
        public GraphicData graphicData;
        //public string texPath;
        public bool supportsHead = true;
        public bool isHead = false;

        public static GraphicDef Named(string defName)
        {
            return DefDatabase<GraphicDef>.GetNamed(defName);
        }
    }
}
