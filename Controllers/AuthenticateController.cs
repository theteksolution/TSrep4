/*
 Authentication Controller
 LR 04122015
 * 
 * 
 * 
 * 
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.ViewModels;
using EarthSkyTime.Models;
using System.Data.SqlClient; 

namespace EarthSkyTime.Controllers
{
    public class AuthenticateController : Controller
    {
        //
        // GET: /Authenticate/


        public ActionResult Index()
        {
            return View();
        }


        // Handles login, at some point this could hook into other authentication
        [HttpPost()]
        public ActionResult Index(LoginVM loginVM)
        {
            // For now get this value from web.config
            string userName = "est123"; //System.Configuration.ConfigurationManager.AppSettings["UserNames"];
            string password = "est123"; //System.Configuration.ConfigurationManager.AppSettings["Passwords"];



            if (loginVM.UserName == userName && loginVM.Password == password)
            {
                Session["UserID"] = "1";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.IsError = "True";
                return View();
            }


            //Session["UserID"] = "1";
            //return RedirectToAction("Index", "Home");
        }

        // Handle logout
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Authenticate");
        }
    }
}
