using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;

namespace SerienCosmeticsProjectASP.Controllers
{
    public class CategoryController : Controller
    {
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1 ();
        // GET: Category
        public ActionResult Index()
        {
            HomeModel model = new HomeModel ();
            model.ListCategory = database.Categories.ToList();
            model.ListProduct = database.Products.ToList();
            return View(model);
        }
        public ActionResult ProductCate(int Id)
        {
            HomeModel model = new HomeModel();
            model.ListProduct = database.Products.Where(x => x.IDCate == Id).ToList();
            model.ListCategory = database.Categories.ToList();
            return View(model);
        }
    }
}