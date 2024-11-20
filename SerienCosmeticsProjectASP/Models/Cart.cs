 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SerienCosmeticsProjectASP.Models
{
    public class CartItem
    {
        public Product _product { get; set; }
        public int _quantity { get; set; }

    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items { get { return items; } }
        public void AddProductToCart(Product pro, int quan = 1)
        {
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var item = Items.FirstOrDefault(x => x._product.ProductID == pro.ProductID);
            if (item == null)
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm sản phẩm mới
                items.Add(new CartItem
                {
                    _product = pro,
                    _quantity = quan
                });
            }
            else
            {
                // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
                item._quantity += quan;
            }
        }
        public int TotalQuantity() {  return items.Sum(s => s._quantity); }
        public decimal? TotalMoney() { return items.Sum(s => s._quantity * s._product.Price); }
        public void UpdateQuantity(int id, int newquan) 
        { 
            var item = items.Find(s => s._product.ProductID == id);
            if (item != null)
            {
                item._quantity = newquan;
            }
        }
        public void RemoveCartItem(int id)
        {
            items.RemoveAll(s => s._product.ProductID == id);
        }
        public void ClearCart()
        {
            items.Clear();
        }
    }

}