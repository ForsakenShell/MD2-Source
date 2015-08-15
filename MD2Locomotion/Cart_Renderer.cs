using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Cart_Renderer
    {
        private Cart cart;

        public Cart_Renderer(Cart cart)
        {
            this.cart = cart;
        }

        public void RenderCartAt(Vector3 loc)
        {
            if (!cart.graphics.GraphicsResolved)
                cart.graphics.ResolveGraphics();
            RenderCartInternal(loc, Quaternion.identity, cart.pather.Direction.ToRot4(), cart.graphics.GraphicAt(cart.pather.Direction));
        }

        public void RenderCartInternal(Vector3 loc, Quaternion quat, Rot4 rotation, Graphic graphic, int layer = 0)
        {
            Mesh mesh = graphic.MeshAt(rotation);
            Material mat = graphic.MatSingle;
            Graphics.DrawMesh(mesh, loc, quat, mat, layer);
        }
    }
}
