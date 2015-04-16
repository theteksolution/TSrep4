/*
 Error Controller
 LR 04142015
 * 
 This will be called if there is an application error
 * 
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarthSkyTime.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

    }
}
