using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Cart_DrawerTracker : IExposable //Saveable
    {
        private Cart cart;
        private Cart_DrawPosTracker drawPosTracker;
        private Cart_Renderer renderer;

        public Cart_DrawerTracker(Cart cart)
        {
            this.cart = cart;
            this.drawPosTracker = new Cart_DrawPosTracker(cart);
            this.renderer = new Cart_Renderer(cart);
        }

        public Vector3 DrawPos
        {
            get
            {
                Vector3 pos = drawPosTracker.TweenedPos;
                pos.y = cart.def.Altitude;
                return pos;
            }
        }

        public virtual void DrawAt(Vector3 pos)
        {
            renderer.RenderCartAt(pos);
        }

        public virtual void Tick()
        {
            this.drawPosTracker.Tick();
        }

        public void ExposeData()
        {
        }
    }
}
