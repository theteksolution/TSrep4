/*
 * 
 Locations Controller
 LR 04142015
 * 
 * 
 Applies the LogFilter for logged in validation
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.ViewModels;
using EarthSkyTime.BusinessLogic;
using EarthSkyTime.Models;

namespace EarthSkyTime.Controllers
{
    public class LocationsController : Controller
    {
        //
        // GET: /Locations/

        [LogFilterAttribute]
        public ActionResult Index()
        {
            LocationVM oLocVM = new LocationVM();

            LocationsBL oLocBL = new LocationsBL();

            ViewBag.LocationsList = oLocBL.BuildLocationSelect();

            // Render view model
            return View(oLocVM);
        }

        // Locations are added/updated here
        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {
            int locationID = Convert.ToInt32(collection["selectLocation"].ToString());
            string sIsUpdate = collection["IsUpdate"].ToString();

            LocationsBL oLocBL = new LocationsBL();
            
            if (sIsUpdate == "Y")
            {
                oLocBL.UpdateLocation(collection);
                ViewBag.LocationAdded = "True";
            }


            ViewBag.LocationsList = oLocBL.BuildLocationSelect(locationID);

            if (locationID > 0)
            {
                ViewBag.IsEdit = "True";
            }
            else
            {
                ViewBag.IsEdit = "False";
            }
            return View(oLocBL.GetLocation(locationID));
            
        }

    }
}
