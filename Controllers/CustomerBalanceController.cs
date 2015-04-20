using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.ViewModels;
using EarthSkyTime.Models;
using EarthSkyTime.BusinessLogic;

namespace EarthSkyTime.Controllers
{
    public class CustomerBalanceController : Controller
    {
        //
        // GET: /CustomerBalance/

        [LogFilterAttribute]
        public ActionResult Index()
        {

            CustomerBalanceBL oCustBalBL = new CustomerBalanceBL();

            ViewBag.FilterValue = "";

            return View(oCustBalBL.GetCustomerBalances());
        }

        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {

            ViewBag.FilterValue = collection["filters"].ToString();

            CustomerBalanceBL oCustBalBL = new CustomerBalanceBL();
            return View(oCustBalBL.GetCustomerBalances(collection));
        }

        // Export as csv
        [LogFilterAttribute]
        public void ExportCSV(string filter)
        {
            CustomerBalanceBL oCustBalBL = new CustomerBalanceBL();

            oCustBalBL.ExportDataTableToCsv("AllBalances", filter);
        }
    }
}
