using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MD2
{
    public class Cart_DrawPosTracker
    {
        private const float SpringTightness = 0.09f;

        private Cart cart;
        private Vector3 springPos = new Vector3(0, 0, 0);
        private Vector3 lastTickSpringPos;

        public Cart_DrawPosTracker(Cart cart)
        {
            this.cart = cart;
            if (cart.SpawnedInWorld && !cart.Destroyed)
            {
                springPos = cart.Position.ToVector3Shifted();
            }
        }

        public Vector3 TweenedPos
        {
            get
            {
                return this.springPos;
            }
        }

        public Vector3 TweenedPosRoot
        {
            get
            {
                if (cart.pather.Moving)
                {
                    float num = 0f;
                    if (cart.pather.Moving)
                    {
                        num = 1f - (float)cart.pather.TicksUntilMove / (float)cart.pather.TotalMoveDuration;
                    }
                    return cart.pather.Destination.Position.ToVector3Shifted() * num + cart.Position.ToVector3Shifted() * (1f - num);
                }
                else
                    return cart.Position.ToVector3Shifted();
            }
        }

        public Vector3 LastTickTweenedVelocity
        {
            get
            {
                return this.TweenedPos - lastTickSpringPos;
            }
        }

        public void Tick()
        {
            lastTickSpringPos = springPos;
            Vector3 a = TweenedPosRoot - springPos;
            springPos += a * SpringTightness;
        }
    }
}
