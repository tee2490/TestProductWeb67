﻿using ProductWeb.Data;
using ProductWeb.Services.IService;

namespace ProductWeb.Services
{
    public class ShoppingCartService : IShoppingCartService<ShoppingCart>
    {
        private readonly ProductContext productContext;

        public ShoppingCartService(ProductContext productContext)
        {
            this.productContext = productContext;
        }

        public void DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
        }

        public void IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count; //update
        }

        public void Save()
        {
            productContext.SaveChanges();
        }

        public void Add(ShoppingCart shoppingCart)
        {
            productContext.Add(shoppingCart);
        }
    }

}
