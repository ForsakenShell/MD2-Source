using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Cart_GraphicSet
    {
        private Cart cart;
        private bool graphicsResolved = false;

        public Graphic upGraphic;
        public Graphic downGraphic;
        public Graphic leftGraphic;
        public Graphic rightGraphic;
        public Graphic sideGraphic;
        public Graphic frontGraphic;

        public Cart_GraphicSet(Cart cart)
        {
            this.cart = cart;
        }

        public Graphic GraphicAt(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return upGraphic;
                case Direction.Down:
                    return downGraphic;
                case Direction.Left:
                    return leftGraphic;
                case Direction.Right:
                    return rightGraphic;
                case Direction.Any:
                    return upGraphic;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Graphic GraphicAt(Rot4 rot)
        {
            return GraphicAt(rot.ToDirection());
        }

        public void ResolveGraphics()
        {
            if (graphicsResolved)
                return;

            graphicsResolved = true;

            string path;
            try
            {
                path = cart.Def.frontGraphicPath ?? cart.Def.upGraphicPath ?? cart.Def.downGraphicPath;
                frontGraphic = GraphicDatabase.Get<Graphic_Single>(path, ShaderDatabase.Cutout, Vector2.one, Color.white);
            }
            catch (Exception ex)
            {
                graphicsResolved = false;
                Log.Error("No valid graphic path found for front graphic in " + cart.Def.defName + ":  " + ex.Message);
            }

            try
            {
                path = cart.Def.sideGraphicPath ?? cart.Def.leftGraphicPath ?? cart.Def.rightGraphicPath;
				sideGraphic = GraphicDatabase.Get<Graphic_Single>(cart.Def.sideGraphicPath, ShaderDatabase.Cutout, Vector2.one, Color.white);
            }
            catch (Exception ex)
            {
                graphicsResolved = false;
                Log.Error("No valid graphic path found for side graphic in " + cart.Def.defName + ":  " + ex.Message);
            }

            if (cart.Def.leftGraphicPath.NullOrEmpty())
                leftGraphic = sideGraphic;
            else
				leftGraphic = GraphicDatabase.Get<Graphic_Single>(cart.Def.leftGraphicPath, ShaderDatabase.Cutout, Vector2.one, Color.white);

            if (cart.Def.rightGraphicPath.NullOrEmpty())
                rightGraphic = sideGraphic;
            else
				rightGraphic = GraphicDatabase.Get<Graphic_Single>(cart.Def.rightGraphicPath, ShaderDatabase.Cutout, Vector2.one, Color.white);

            if (cart.Def.upGraphicPath.NullOrEmpty())
                upGraphic = frontGraphic;
            else
				upGraphic = GraphicDatabase.Get<Graphic_Single>(cart.Def.upGraphicPath, ShaderDatabase.Cutout, Vector2.one, Color.white);

            if (cart.Def.downGraphicPath.NullOrEmpty())
                downGraphic = frontGraphic;
            else
				downGraphic = GraphicDatabase.Get<Graphic_Single>(cart.Def.downGraphicPath, ShaderDatabase.Cutout, Vector2.one, Color.white);
        }

        public bool GraphicsResolved
        {
            get
            {
                return this.graphicsResolved;
            }
        }
    }
}
