using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerienCosmeticsProjectASP.Models;

namespace SerienCosmeticsProjectASP.Controllers
{
    public class ProductController : Controller
    {
        SerienCosmeticsDBEntities1 database = new SerienCosmeticsDBEntities1 ();
        // GET: Product
        public ActionResult Index(string SearchString)
        {
            HomeModel model = new HomeModel();
            if (!string.IsNullOrEmpty(SearchString))
            {
                model.ListProduct = database.Products.Where(n => n.NamePro.Contains(SearchString)).ToList();
            }
            else 
            {
                model.ListProduct = database.Products.ToList();
            }
            model.ListCategory = database.Categories.ToList();
            return View(model);
        }
        
            public ActionResult Detail(int Id)
        {
            var obj = database.Products.Where(x => x.ProductID == Id).FirstOrDefault();
            return View(obj);
        }
        
       

    }
}