/*
 Customer Controller
 LR 04122015
 * 
 Applies LogFilter to validate logged in
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
using EarthSkyTime.BusinessLogic;

namespace EarthSkyTime.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        [LogFilterAttribute]
        public ActionResult Index(int ID = 0)
        {
            // Customer business logic
            CustomerBL oCustBL = new CustomerBL();

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect(ID,false);

            ViewBag.Error = false;
            
            // Render the view model
            return View(oCustBL.GetCustomerInfo(ID));  
        }


        // Handles the post
        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {
            int customerID = Convert.ToInt32(collection["selectCustomer"].ToString());

            // Customer business logic
            CustomerBL oCustBL = new CustomerBL();

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect(customerID,false);

            // Render the view model
            return View(oCustBL.GetCustomerInfo(customerID));  
        }


        // Add customer
        [LogFilterAttribute]
        public ActionResult AddCustomer()
        {
            // View model
            CustomerVM oCustVM = new CustomerVM();

            // Business logic
            CustomerBL oCustBL = new CustomerBL();

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect(0,true);

            return View(oCustVM);
        }



        // Handle add/update customer post
        [LogFilterAttribute]
        [HttpPost()]
        public ActionResult AddCustomer(FormCollection collection)
        {

            int CustomerID = Convert.ToInt32(collection["selectCustomer"].ToString());
            string sIsUpdate = collection["IsUpdate"].ToString();
            int iReturnCustID = 0;


            CustomerBL oCustBL = new CustomerBL();

            if (sIsUpdate == "Y")
            {
                iReturnCustID = oCustBL.UpdateCustomer(collection);
                ViewBag.CustomerAdded = true;
            }
            else
            {
                iReturnCustID = CustomerID; 
            }

            if (CustomerID > 0)
            {
                ViewBag.IsEdit = "True";
            }
            else
            {
                ViewBag.IsEdit = "False";
            }

            ViewBag.Error = false;

            ViewBag.CustomerList = oCustBL.BuildCustomerSelect(iReturnCustID,true );


            // Render view model
            return View(oCustBL.GetCustomerInfo(iReturnCustID));
        }

        
        // Handle csv export
        public void ExportCSV(int ID)
        {
            
            CustomerBL oCustBL = new CustomerBL();

            oCustBL.ExportDataTableToCsv(ID, "CustomerTransactions"); 

        }
    }
}
