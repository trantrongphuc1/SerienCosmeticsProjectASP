using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;
using System.Data.Entity;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.Web.Helpers;
using System.Xml.Linq;

namespace SerienCosmeticsProjectASP.Controllers
{

    [RouteArea("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1();
        private PasswordHelper passwordHelper = new PasswordHelper();
        [Route("")]
        [Route("index")]
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {

                    var model = new StatisticsViewModel
                    {
                        TotalUsers = database.Customers.Count(),
                        TotalOrders = database.Orders.Count(),
                        TotalRevenue = (decimal)database.OrderDetails.Sum(d => d.Quantity * d.UnitPrice),
                        TotalProducts = database.Products.Count()
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }
                
            }
           


        }
        [Route("product")]
        public ActionResult Product()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {

                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var lstproduct = database.Products.Include(p => p.Category).ToList();
                    return View(lstproduct);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }
                
            }
        }
        public ActionResult CustomerList()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var lstcustomer = database.Customers.ToList();
                    return View(lstcustomer);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }
        }
        public ActionResult CategoryList()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var lstcategory = database.Categories.ToList();
                    return View(lstcategory);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }
        }
        public ActionResult OrderList()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var lstorders = database.OrderDetails.ToList();
                    return View(lstorders);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }
        }
        [Route("createnew")]
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var category = database.Categories.Select(c => new SelectListItem { Value = c.IDCate.ToString(), Text = c.NameCate }).ToList();
                    ViewBag.CategoryList = category;
                    ViewBag.Category = new SelectList(database.Categories.ToList(), "IDCate", "NameCate");
                    return View();
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }


            }
        }
        [Route("createnew")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product pro)
        {
            if (ModelState.IsValid)
            {
                if (pro.UploadImage != null && pro.UploadImage.ContentLength > 0)
                {
                    var filename = Path.GetFileName(pro.UploadImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/source/img/product/"), filename);
                    pro.UploadImage.SaveAs(path);
                    pro.ImagePro = filename;
                    database.Products.Add(pro);
                    database.SaveChanges();
                    return RedirectToAction("Product");
                }
            }
            return View(pro);
        }

        
        [HttpGet]
        public ActionResult CreateCategory()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var userEmail = Session["user"];
                    if (userEmail == "admin")
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("LoginAdmin");
                    }
                }
                return View();


            }
        }
        [Route("createnew")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category cate)
        {
            database.Categories.Add(cate);
            database.SaveChanges();
            return RedirectToAction("CategoryList");
            
        }




        [Route("edit")]
        [HttpGet]
        public ActionResult Edit(int IDPro)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var pro = database.Products.Find(IDPro);
                    if (pro == null)
                    {
                        return HttpNotFound();
                    }

                    var category = database.Categories.Select(c => new SelectListItem { Value = c.IDCate.ToString(), Text = c.NameCate }).ToList();
                    ViewBag.CategoryList = category;
                    ViewBag.Category = new SelectList(database.Categories.ToList(), "IDCate", "NameCate", pro.Category);

                    return View(pro);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }
            
        }
        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product pro)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = database.Products.Find(pro.ProductID);
                if (existingProduct != null)
                {
                    existingProduct.NamePro = pro.NamePro;
                    existingProduct.Price = pro.Price;
                    existingProduct.DescriptionPro = pro.DescriptionPro;
                    existingProduct.IDCate = pro.IDCate;
                    if (pro.UploadImage != null && pro.UploadImage.ContentLength > 0)
                    {
                        var filename = Path.GetFileName(pro.UploadImage.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/source/img/product/"), filename);
                        pro.UploadImage.SaveAs(path);
                        pro.ImagePro = filename;
                        existingProduct.ImagePro = filename;
                    }
                    database.Entry(existingProduct).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("Product", "Admin");
                }
            }
            return View(pro);
        }
       
        [HttpGet]
        public ActionResult EditCustomer(int CusID,string name,string phonenum, string email, string address)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var customer = database.Customers.Find(CusID, name, phonenum, email, address);

                    if (customer == null)
                    {
                        return Content("Không tìm thấy tài khoản");
                    }

                    return View(customer);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {

                var existingCustomer = database.Customers.Find(customer.CustomerID);
                if (existingCustomer != null)
                {
                    
                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.CustomerPhoneNum = customer.CustomerPhoneNum;
                    existingCustomer.CustomerEmail = customer.CustomerEmail;
                    existingCustomer.CustomerAddress = customer.CustomerAddress;

                    

                    database.Entry(existingCustomer).State = EntityState.Modified;
                    database.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("CustomerList", "Admin");
                }
                else
                {
                    ViewBag.Error = "Không tìm thấy khách hàng để cập nhật.";
                }
            }

            return View(customer);
        }
        [HttpGet]
        public ActionResult EditCategory(int IDCate)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var cate = database.Categories.Find(IDCate);
                    if (cate == null)
                    {
                        return HttpNotFound();
                    }

                    
                    return View(cate);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category cate)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = database.Categories.Find(cate.IDCate);
                if (existingCategory != null)
                {
                    existingCategory.NameCate = cate.NameCate;
                    
                   
                    database.Entry(existingCategory).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("CategoryList", "Admin");
                }
            }
            return View(cate);
        }

        [HttpGet]
        public ActionResult EditOrder(int ordID)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LoginAdmin");
            }
            else
            {
                var userEmail = Session["user"];
                if (userEmail == "admin")
                {
                    var ord = database.OrderDetails.Find(ordID);
                    if (ord == null)
                    {
                        return HttpNotFound();
                    }


                    return View(ord);
                }
                else
                {
                    return RedirectToAction("LoginAdmin");
                }

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(OrderDetail ord)
        {
            if (ModelState.IsValid)
            {
                var existingOrder = database.OrderDetails.Find(ord.OrderDetailID);
                if (existingOrder != null)
                {
                    existingOrder.OrderID = ord.OrderID;
                    existingOrder.NamePro = ord.NamePro;
                    existingOrder.Quantity = ord.Quantity;
                    existingOrder.UnitPrice = ord.UnitPrice;
                    existingOrder.OrderDate = ord.OrderDate;

                    database.Entry(existingOrder).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("OrderList", "Admin");
                }
            }
            return View(ord);
        }



        [Route("details")]
        public ActionResult Details(int IDPro)
        {
            
                    var lstproduct = database.Products.Include(p => p.Category).ToList();
                    var obj = database.Products.Where(x => x.ProductID == IDPro).FirstOrDefault();
                    return View(obj);
                
        }
        public ActionResult DetailsCategory(int IDCate)
        {
                    var obj = database.Categories.Where(x => x.IDCate == IDCate).FirstOrDefault();
                    return View(obj); 
        }
        public ActionResult DetailsOrder(int ordID)
        {
            var obj = database.OrderDetails.Where(x => x.OrderDetailID == ordID).FirstOrDefault();
            return View(obj);
        }
        public ActionResult DetailsCustomer(int CusID, string name, string phonenum, string email, string address)
        {
            var obj = database.Customers.Where(x => x.CustomerID == CusID).FirstOrDefault();
            return View(obj);
        }
        [Route("delete")]
        [HttpGet]
        public ActionResult Delete(int IDPro)
        {
            //TempData để thông báo vì dữ liệu bị chuyển hướng
            TempData["Message"] = "";
            var orderdetail = database.OrderDetails.Where(x => x.ProductID == IDPro);
            if (orderdetail.Count() > 0)
            {
                TempData["Message"] = "Không xóa được sản phẩm này";
                return RedirectToAction("Product", "Admin");
            }
            database.Products.Remove(database.Products.Find(IDPro));
            database.SaveChanges();
            TempData["Message"] = "Sản phẩm đã được xóa";
            return RedirectToAction("Product", "Admin");
        }
        public ActionResult DeleteCustomer(int CusID, string name, string phonenum, string email, string address)
        {
            //TempData để thông báo vì dữ liệu bị chuyển hướng
            TempData["Message"] = "";
           
            database.Customers.Remove(database.Customers.Find(CusID, name, phonenum, email, address));
            database.SaveChanges();
            TempData["MessageCus"] = "Người dùng đã được xóa";
            return RedirectToAction("CustomerList", "Admin");
        }
        public ActionResult DeleteCategory(int IDCate)
        {
            //TempData để thông báo vì dữ liệu bị chuyển hướng
            TempData["Message"] = "";

            using (var transaction = database.Database.BeginTransaction())
            {
                try
                {
                    // Tìm và xóa các sản phẩm liên quan đến danh mục này
                    var products = database.Products.Where(p => p.IDCate == IDCate).ToList();
                    database.Products.RemoveRange(products);
                    database.SaveChanges();

                    // Tìm và xóa danh mục
                    var category = database.Categories.Find(IDCate);
                    if (category != null)
                    {
                        database.Categories.Remove(category);
                        database.SaveChanges();
                    }

                    // Xác nhận các thay đổi
                    transaction.Commit();
                    TempData["MessageCate"] = "Danh mục và tất cả sản phẩm liên quan đã được xóa.";
                }
                catch (Exception ex)
                {
                    // Khôi phục lại nếu có lỗi
                    transaction.Rollback();
                    TempData["MessageCate"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }

            return RedirectToAction("CategoryList", "Admin");
        }
        public ActionResult DeleteOrder(int ordID)
        {
            //TempData để thông báo vì dữ liệu bị chuyển hướng
            TempData["Message"] = "";

            database.OrderDetails.Remove(database.OrderDetails.Find(ordID));
            database.SaveChanges();
            TempData["MessageCate"] = "Đơn hàng đã được xóa";
            return RedirectToAction("OrderList", "Admin");
        }
        [HttpGet]
        public ActionResult LoginAdmin()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(Customer user)
        {
            var passwordHash = passwordHelper.GetMD5(user.CustomerPassword);
            

            if (user.CustomerEmail == "admin@gmail.com" && user.CustomerPassword == "123456")
            {
                //Lưu Session 
                Session["user"] = "admin";

                    return RedirectToAction("Index");
                
            }
            else
            {
                
                TempData["Error"] = "Tài khoản không tồn tại!          liuliu :P";
                return RedirectToAction("LoginAdmin");
            }


        }
        public ActionResult LogoutAdmin()
        {
            Session.Clear();//Xóa session
            return RedirectToAction("LoginAdmin", "Admin");
        }
    }
}
