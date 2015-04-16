/*
 * 
 Transactions Controller
 LR 04142015
 * 
 * */

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
    public class TransactionsController : Controller
    {
        //
        // GET: /Transactions/


        [LogFilterAttribute]
        public ActionResult Index()
        {
            List<TransactionVM> lTransactions = new List<TransactionVM>();

            TransactionsBL oTranBL = new TransactionsBL();

            ViewBag.LocationsList = oTranBL.BuildLocationSelect();

            return View(oTranBL.GetTransactions());
        }



        // Get all transactions, with filtering
        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {
            TransactionsBL oTranBL = new TransactionsBL();

            if (collection["dateFrom"] != "")
            {
                ViewBag.FromDate = collection["dateFrom"].ToString();
            }

            if (collection["dateTo"] != "")
            {
                ViewBag.ToDate = collection["dateTo"].ToString();
            }

            ViewBag.LocationsList = oTranBL.BuildLocationSelect(Convert.ToInt32(collection["selectLocation"].ToString()));

            return View(oTranBL.GetTransactions(collection));
        }

      
        // Export as csv
        public void ExportCSV()
        {
            TransactionsBL oTranBL = new TransactionsBL();

            oTranBL.ExportDataTableToCsv("AllTransactions"); 
        }

    }
}
