using SerienCosmeticsProjectASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;

namespace SerienCosmeticsProjectASP.Controllers
{
    public class ShoppingCartController : Controller
    {
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1();
        // GET: ShoppingCart
        List<CartItem> items = new List<CartItem>();
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
            {
                return View("EmptyCart");
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;    
        }
        public ActionResult AddToCart(int id) 
        {
            var pro = database.Products.SingleOrDefault(s => s.ProductID == id);
            if (pro != null)
            {
                GetCart().AddProductToCart(pro);
            }
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.RemoveCartItem(id);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public PartialViewResult BagCart()
        {
            int TotalQuantityItem = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                TotalQuantityItem = cart.TotalQuantity();
            }
                ViewBag.QuantityCart = TotalQuantityItem;
                return PartialView("BagCart");   
        }
        public PartialViewResult TotalCart()
        {
            decimal? TotalCart = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                TotalCart = cart.TotalMoney();
            }
                ViewBag.TotalCart = TotalCart;
            return PartialView("TotalCart");
        }
        [HttpPost]
        public ActionResult UpdateCartQuantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(form["idPro"]);
            int quan = int.Parse(form["cartQuantity"]);
            cart.UpdateQuantity(id_pro, quan);
            return RedirectToAction("ShowCart","ShoppingCart");
        }
       
        
        public ActionResult CustomerInfo()
        {
            // Kiểm tra xem giỏ hàng có sản phẩm không
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || cart.Items.Count() ==0)
            {
                return RedirectToAction("ShowCart");
            }

            // Tính tổng tiền trong giỏ hàng và truyền vào ViewBag để sử dụng trong View
            ViewBag.TotalAmount = cart.TotalMoney();

            return View(cart);
        }
        
        [HttpPost]
        public ActionResult PlaceOrder(FormCollection form)
        {
            // Kiểm tra giỏ hàng
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || cart.Items.Count() == 0)
            {
                return RedirectToAction("ShowCart");
            }

            try
            {
                // Kiểm tra trạng thái đăng nhập bằng Session
                var user = Session["use"] as Customer;
                if (user != null)
                {

                    // Giả sử `user` chứa các thông tin như Tên, Địa chỉ, Số điện thoại, Email
                    int customerId = user.CustomerID;
                    string customerName = user.CustomerName;
                    string phone = user.CustomerPhoneNum;
                    string email = user.CustomerEmail;
                    string address = user.CustomerAddress;
                    // Tạo đơn hàng và lưu vào cơ sở dữ liệu
                    Order order1 = new Order
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        CustomerName = customerName,
                        CustomerPhoneNum = phone,
                        CustomerEmail = email,
                        CustomerAddress = address
                    };

                    database.Orders.Add(order1);
                    database.SaveChanges();
                    // Thêm chi tiết sản phẩm vào đơn hàng
                    foreach (var item in cart.Items)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderID = order1.OrderID,
                            ProductID = item._product.ProductID,
                            NamePro = item._product.NamePro,
                            OrderDate = order1.OrderDate,
                            UnitPrice = (double)item._product.Price,
                            Quantity = item._quantity
                        };
                        database.OrderDetails.Add(orderDetail);

                    }

                    
                    database.SaveChanges();

                    // Xóa giỏ hàng sau khi đặt hàng thành công
                    cart.ClearCart();

                    // Chuyển hướng đến trang đặt hàng thành công
                    return RedirectToAction("OrderSuccess");
                }
                else
                {
                   

                    // Tạo đối tượng Order và lưu thông tin khách hàng
                    Order order = new Order
                    {
                        OrderDate = DateTime.Now,
                        
                        CustomerAddress = form["Address"],
                        CustomerName = form["FullName"],
                        CustomerEmail = form["Email"],
                        CustomerPhoneNum = form["Phone"],
                    };

                    database.Orders.Add(order);
                    database.SaveChanges();



                    // Thêm từng sản phẩm trong giỏ hàng vào chi tiết đơn hàng
                    foreach (var item in cart.Items)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item._product.ProductID,
                            NamePro = item._product.NamePro,
                            OrderDate = DateTime.Now,
                            UnitPrice = (double)item._product.Price,
                            Quantity = item._quantity
                        };
                        database.OrderDetails.Add(orderDetail);
                    }

                    // Lưu vào cơ sở dữ liệu
                    database.SaveChanges();



                    // Xóa giỏ hàng sau khi đặt hàng thành công
                    cart.ClearCart();

                    // Điều hướng đến trang OrderSuccess
                    return RedirectToAction("OrderSuccess");
                }
                
            }
            catch
            {
                return Content("Đặt hàng thất bại. Vui lòng thử lại.");
            }
        }
        public ActionResult OrderSuccess()
        {
            Cart cart = Session["Cart"] as Cart;
            cart.ClearCart();
            return View();
        }


    }
}