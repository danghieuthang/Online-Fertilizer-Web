using OnlineFertilizerWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineFertilizerWeb.Domain
{
    public class CartItem
    {
        private Product product;
        private int quantity;

        public Product Product { get => product; set => product = value; }
        public int Quantity { get => quantity; set => quantity = value; }
    }
    public class Cart
    {
        private List<CartItem> listProduct = new List<CartItem>();
        public Cart()
        {

        }
        public void AddToCart(Product p, int quantity)
        {
            CartItem item = listProduct.Where(pro => pro.Product.ID == p.ID).FirstOrDefault();
            if (item == null)
            {
                listProduct.Add(new CartItem { Product = p, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
        }
        public void Update(Product p, int quantity)
        {
            CartItem item = listProduct.Where(pro => pro.Product.ID == p.ID).FirstOrDefault();

            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
        public void RemoveItem(Product p)
        {
            listProduct.RemoveAll(pro => pro.Product.ID == p.ID);
        }
        public void Clear()
        {
            listProduct.Clear();

        }
        public IEnumerable<CartItem> Products
        {
            get { return listProduct; }
        }
        public double Total()
        {
            double total = 0;
            foreach (CartItem item in listProduct)
            {
                total += item.Product.Price * item.Quantity;
            }
            return total;
        }
    }
}