using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class CartDef : ThingDef
    {
        public int cellsPerSecond;

        public string leftGraphicPath;
        public string rightGraphicPath;
        public string upGraphicPath;
        public string downGraphicPath;

        public string frontGraphicPath;
        public string sideGraphicPath;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach(var e in base.ConfigErrors())
            {
                yield return e;
            }
            if (frontGraphicPath.NullOrEmpty() && upGraphicPath.NullOrEmpty() && downGraphicPath.NullOrEmpty())
                yield return "frontGraphicPath in cartDef " + defName + "cannot be null or empty with up and down paths also null or empty";
            if(sideGraphicPath.NullOrEmpty() && leftGraphicPath.NullOrEmpty() && rightGraphicPath.NullOrEmpty())
            {
                yield return "sideGraphicPath in cartDef " + defName + "cannot be null or empty with left and right paths also null or empty";
            }
        }
    }
}
