/*
 Home Controller
 * 
 LR 04122015
 * 
 * 
 This will handle a $$$ update for customers
 * 
 LogFilter is used for logged in validation
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.Models;
using EarthSkyTime.ViewModels;
using EarthSkyTime.BusinessLogic;
using EarthSkyTime.Controllers;

namespace EarthSkyTime.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        [LogFilterAttribute]
        public ActionResult Index()
        {

            CustomerBL oCustBL = new CustomerBL();

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect();
            ViewBag.LocationsList = oCustBL.BuildLocationSelect();

            CustomerVM oCustVM = null; 


            // Render the view model
            return View(oCustVM);
        }



        // Update a customers balance 
        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {
            
            // Form and other variables
            int iCustomerID = Convert.ToInt32(collection["selectCustomer"].ToString());
            
           
            string sIsAmountUpdate = collection["IsAmountUpdate"].ToString();

            decimal dAmount = 0;
            decimal d;
            bool isNumeric = true;
            int iLocationID = 0;

            if (collection["selectLocation"] != null)
            {
                 iLocationID = Convert.ToInt32(collection["selectLocation"].ToString());
            }


            // Determine if this is a debit or credit
            if (collection["inputAmount"] != null)
            {
               string sPurchaseType = collection["purchaseType"].ToString();

               isNumeric = decimal.TryParse(collection["inputAmount"].ToString(), out d);
               if (isNumeric)
               {
                   dAmount  = Convert.ToDecimal(collection["inputAmount"].ToString());

                   if (sPurchaseType == "Purchase")
                   {
                       dAmount = dAmount * -1;
                   }

               } 
            }
           
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();


            // Update the amount, then add to the transactions history
            if (sIsAmountUpdate == "Y" && dAmount != 0 && isNumeric)
            {
                var oBal = (from b in estEnt.CustomerBalances
                           where b.CustomerID == iCustomerID
                           select b).FirstOrDefault();
                if (oBal != null)
                {
                    oBal.Balance += dAmount;

                    // Insert Transaction

                    Transaction oTran = new Transaction() { AddedBy = "LR", CustomerID = iCustomerID, Amount = dAmount, LocationID = iLocationID, TransactionDate = DateTime.Now };

                    estEnt.AddToTransactions(oTran);

                    estEnt.SaveChanges();

                    ViewBag.AmountAdded = "Y";
                }
            }
            if (sIsAmountUpdate == "Y" && !isNumeric)
            {
                ViewBag.Error = "True";
            }

            CustomerVM oCustInfo = (from c in estEnt.Customers 
                            where c.CustomerID == iCustomerID 
                            join b in estEnt.CustomerBalances on c.CustomerID equals b.CustomerID into joinedT
                            from b in joinedT.DefaultIfEmpty()
                            select new CustomerVM  { City = c.City, Email = c.Email, FirstName = c.FirstName, LastName = c.LastName,
                                 Phone = c.Phone, State = c.State, Street1 = c.Street1, Street2= c.Street2 ,Zip = c.Zip, Amount = b.Balance == null ? 0 : (decimal)b.Balance}).FirstOrDefault();


            CustomerBL oCustBL = new CustomerBL();

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect(iCustomerID);
            ViewBag.LocationsList = oCustBL.BuildLocationSelect();


            // Render the view model
            return View(oCustInfo);
        }

    }
}
