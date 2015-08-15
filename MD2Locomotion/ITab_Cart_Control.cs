using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class ITab_Cart_Control : ITab
    {
        public static readonly Vector2 winSize = new Vector2(200f, 200f);
        protected Cart SelCart
        {
            get
            {
                return (Cart)SelThing;
            }
        }

        public ITab_Cart_Control()
        {
            this.size = winSize;
            this.labelKey = "TabCartControl";
        }

        protected override void FillTab()
        {
            
        }
    }
}
