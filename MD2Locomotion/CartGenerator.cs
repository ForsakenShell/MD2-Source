using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD2
{
    public static class CartGenerator
    {
        public static Cart GenerateCart(CartDef def)
        {
            Cart cart = (Cart)Verse.ThingMaker.MakeThing(def);
            cart.pather = new Cart_Pather(cart);
            cart.drawer = new Cart_DrawerTracker(cart);
            return cart;
        }
    }
}
