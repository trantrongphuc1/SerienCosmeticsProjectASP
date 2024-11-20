using SerienCosmeticsProjectASP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;
using System.Security.Cryptography;
using System.Text;
namespace SerienCosmeticsProjectASP.Controllers
{
    public class AccessController : Controller
    {
        private PasswordHelper passwordHelper = new PasswordHelper();
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Customer user)
        {

            var passwordHash = passwordHelper.GetMD5(user.CustomerPassword);
            var check = database.Customers.Where(x => x.CustomerEmail == user.CustomerEmail && x.CustomerPassword == passwordHash).FirstOrDefault();
                if (check == null)
                {

                TempData["error"] = "Sai email hoặc mật khẩu kìa!";
                return RedirectToAction("Login");

                }
                else
                {

                //Lưu Session 
                Session["User"] = new
                {
                    FullName = check.CustomerName,
                    Email = check.CustomerEmail,
                    Phone = check.CustomerPhoneNum,
                    Address = user.CustomerAddress
                };
                //Lưu Session (để Layout đổi thành tài khoản/Đăng xuất)
                Session["use"] = check;
                    return RedirectToAction("Index","Home");
                }
            
        }
        public ActionResult Logout()
        {
            Session.Clear();//Xóa session
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer user)
        {
            // Kiểm tra và lưu vào database
            if (ModelState.IsValid)
            {
                var check = database.Customers.FirstOrDefault(s => s.CustomerEmail == user.CustomerEmail);
                if(check == null)
                {
                    // Mã hóa mật khẩu
                    user.CustomerPassword = passwordHelper.GetMD5(user.CustomerPassword);
                    database.Configuration.ValidateOnSaveEnabled = false;
                    
                    // Thêm thông tin người dùng vào database
                    database.Customers.Add(user);
                    database.SaveChanges();

                    // Lưu thông tin khách hàng vào session sau khi đăng ký thành công
                    Session["User"] = new
                    {
                        FullName = user.CustomerName,
                        Email = user.CustomerEmail,
                        Phone = user.CustomerPhoneNum,
                        Address = user.CustomerAddress
                    };

                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Email đã tồn tại";
                    return View();
                }
            }
            
            return View();
        }

        [HttpGet]
        public ActionResult AccountInfo()
        {
            // Kiểm tra Session use đã đăng nhập chưa
            if (Session["use"] != null)
            {
                var user = (Customer)Session["use"];
                return View(user);
            }
            else
            {
                // Chưa đăng nhập thì chuyển lại trang Login
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            // Kiểm tra Session use đã đăng nhập chưa
            if (Session["use"] != null)
            {
                var user = (Customer)Session["use"];
                return View(user);
            }
            // Chưa đăng nhập thì chuyển lại trang Login
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult EditProfile(Customer user)
        {
            ViewBag.IsRegistering = false;
            if (ModelState.IsValid)
            {
                if (Session["use"] is Customer currentUser)
                {
                    var dbUser = database.Customers.FirstOrDefault(c => c.CustomerID == currentUser.CustomerID);

                    if (dbUser != null)
                    {
                        
                        if (user.CustomerName != dbUser.CustomerName)
                        {
                            bool nameExists = database.Customers.Any(c => c.CustomerName == user.CustomerName);
                            if (nameExists)
                            {
                                ViewBag.Error = "Tên khách hàng đã tồn tại.";
                                return View(user);
                            }
                        }

                        
                        Customer newUser = new Customer
                        {
                            CustomerName = user.CustomerName,
                            CustomerPhoneNum = user.CustomerPhoneNum,
                            CustomerAddress = user.CustomerAddress,
                            CustomerEmail = user.CustomerEmail,
                            CustomerPassword = string.IsNullOrEmpty(user.CustomerPassword)
                                ? dbUser.CustomerPassword
                                : passwordHelper.GetMD5(user.CustomerPassword)
                        };

                        
                        database.Customers.Add(newUser);
                        database.SaveChanges();
                        // Chuyển đơn hàng ở tài khoản cũ sang tài khoản mới cập nhật
                        var orders = database.Orders.Where(o => o.CustomerID == dbUser.CustomerID).ToList();
                        foreach (var order in orders)
                        {
                            order.CustomerID = newUser.CustomerID; 
                            order.CustomerName = newUser.CustomerName;
                            order.CustomerPhoneNum = newUser.CustomerPhoneNum;
                            order.CustomerAddress = newUser.CustomerAddress;
                            order.CustomerEmail = newUser.CustomerEmail;
                            
                        }
                        database.SaveChanges();

                        database.Customers.Remove(dbUser);
                        database.SaveChanges();

                        Session["use"] = dbUser;
                        Session["User"] = new
                        {
                            FullName = dbUser.CustomerName, 
                            Email = dbUser.CustomerEmail,
                            Phone = dbUser.CustomerPhoneNum,
                            Address = dbUser.CustomerAddress
                        };

                        
                        TempData["success"] = "Cập nhật thông tin thành công!";
                        Session.Clear();//Xóa session
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Người dùng không tồn tại!";
                    }
                }
                else
                {
                    ViewBag.Error = "Session expired. Please log in again.";
                    return RedirectToAction("Login");
                }
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult MyOrders()
        {
            
            var currentUser = (Customer)Session["use"];
            if (currentUser != null)
            {
                var lstorddetails = database.Orders.Include(p => p.OrderDetails).ToList();
                var orders = database.Orders.Where(o => o.CustomerID == currentUser.CustomerID).ToList();
                if (orders == null)
                {
                    // Return a partial view if there are no orders
                    return View("EmptyOrder");
                }
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        

    }
}
