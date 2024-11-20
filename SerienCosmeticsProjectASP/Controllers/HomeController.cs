using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;

namespace SerienCosmeticsProjectASP.Controllers
{
    public class HomeController : Controller
    {
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1 ();
        
        public ActionResult Index()
        {
            // Tạo HomeModel để chứa cả 2 model Category và Product
            HomeModel model = new HomeModel();
            model.ListProduct = database.Products.ToList();
            model.ListCategory = database.Categories.ToList();
            return View(model);
        }
        public ActionResult Index2() //page tiếng Anh chưa hoàn chỉnh
        {
            HomeModel model = new HomeModel();
            model.ListProduct = database.Products.ToList();
            model.ListCategory = database.Categories.ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        
    }
}