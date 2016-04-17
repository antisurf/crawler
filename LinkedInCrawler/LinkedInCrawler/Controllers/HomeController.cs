using LinkedInCrawler.BL;
using LinkedInCrawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkedInCrawler.Controllers
{
    public class HomeController : Controller
    {
        private CrawlerBL _bl;

        public HomeController() :base()
        {
            _bl = new CrawlerBL();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "LinkedIn Crawler";

            return View();
        }
            
        public ActionResult ParseHtml(string url)
        {
            JsonResult response = new JsonResult();
            response.Data = _bl.ParseURL(url);
            _bl.SaveUser(response.Data as User);
            return response;
        }


        //What if there are a couple of people with the same name ????
        public ActionResult FindUser(string name)
        {
            JsonResult response = new JsonResult();
            response.Data = _bl.GetUser(name);
            return response;
        }

        //what if there are a couple of people with the same name ????
        public ActionResult GetTopSkillsCounter(string name)
        {
            JsonResult response = new JsonResult();
            response.Data = _bl.GetUser(name).TopSkillsCounter;
            return response;
        }
    }
}
