/*
 CustomerBL
 Business logic for Customer information. It gets information, updates and adds a customer, creates a balance record on add, and exports csv.
 LR 04142015
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.Models; 
using EarthSkyTime.ViewModels;
using System.Data;


namespace EarthSkyTime.BusinessLogic
{
    public class CustomerBL
    {

        // Build the dropdown for all customers
        public SelectList BuildCustomerSelect(int CustomerID = 0, bool bInfoUpdate = false)
        {
            
            //Connet to db
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();


            List<SelectListItem> lCustomers = new List<SelectListItem>();
            if (bInfoUpdate == true)
            {
                lCustomers.Add(new SelectListItem { Text = "Select customer or keep to add new", Value = "0" });
            }
            else
            {
                lCustomers.Add(new SelectListItem { Text = "Select customer", Value = "0" });
            }
           

            // Get the customers
            var oCust = from c in estEnt.Customers
                        orderby c.LastName 
                        select new { c.LastName, c.FirstName, c.CustomerID };

            foreach (var o in oCust)
            {
                lCustomers.Add(new SelectListItem { Text = o.LastName + ", " + o.FirstName, Value = o.CustomerID.ToString() });
            }


            // Use the current customer if there is one
            if (CustomerID != 0)
            {
                return new SelectList(lCustomers, "Value", "Text", CustomerID.ToString());
            }
            else
            {
                return new SelectList(lCustomers, "Value", "Text");
            }

        }


        // Get all the locations
        public SelectList BuildLocationSelect(int LocationID = 0)
        {
            
            // Connect to the db
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            List<SelectListItem> lLocations = new List<SelectListItem>();
            lLocations.Add(new SelectListItem { Text = "Add a Location", Value = "0" });


            // Get all the locations
            var oLoc = from l in estEnt.Locations
                       select new { l.LocationName, l.LocationID };

            foreach (var o in oLoc)
            {
                lLocations.Add(new SelectListItem { Text = o.LocationName, Value = o.LocationID.ToString() });
            }


            // If one is previously selected use it
            if (LocationID != 0)
            {
                return new SelectList(lLocations, "Value", "Text", LocationID.ToString());
            }
            else
            {
                return new SelectList(lLocations, "Value", "Text");
            }

        }


        // Update the customer
        public int UpdateCustomer(FormCollection collection)
        {
            // Form variables
            int customerID = Convert.ToInt32(collection["selectCustomer"].ToString());
            string sIsUpdate = collection["IsUpdate"].ToString();
            string sFirstName = collection["FirstName"].ToString();
            string sLastName = collection["LastName"].ToString();
            string sPhone = collection["Phone"].ToString();
            string sEmail = collection["Email"].ToString();
            string sStreet1 = collection["Street1"].ToString();
            string sStreet2 = collection["Street2"].ToString();
            string sCity = collection["City"].ToString();
            string sState = collection["State"].ToString();
            string sZip = collection["Zip"].ToString();

            int iReturnCustID = 0;



            // Connect to db
            EarthSkyTimeEntities estEntity = new EarthSkyTimeEntities();


            // Add a new customer
            if (customerID == 0)
            {
                Customer oCust = new Customer()
                {
                    AddedBy = "Admin",
                    City = sCity,
                    FirstName = sFirstName,
                    LastName = sLastName,
                    Phone = sPhone,
                    State = sState,
                    Street1 = sStreet1,
                    Street2 = sStreet2,
                    Zip = sZip,
                    Email = sEmail
                };


                estEntity.AddToCustomers(oCust);
                estEntity.SaveChanges();

                iReturnCustID = oCust.CustomerID; 

                // Setup their Balance information
                CustomerBalance oBal = new CustomerBalance() { Balance = 0, CustomerID = oCust.CustomerID, DateUpdated = DateTime.Now, UpdatedBy = "Admin" };
                estEntity.AddToCustomerBalances(oBal);
                estEntity.SaveChanges();
            }
            else
            {
                // Update the customer
                var oC = (from c in estEntity.Customers 
                         where c.CustomerID == customerID 
                         select c).FirstOrDefault();

                if (oC != null)
                {
                    oC.City = sCity;
                    oC.FirstName = sFirstName;
                    oC.LastName = sLastName;
                    oC.Phone = sPhone;
                    oC.State = sState;
                    oC.Street1 = sStreet1;
                    oC.Street2 = sStreet2;
                    oC.Zip = sZip;
                    oC.Email = sEmail;

                    estEntity.SaveChanges();

                    iReturnCustID = customerID;
                }
            }

            return iReturnCustID;

        }


        // Get a customers information
        public CustomerVM GetCustomerInfo(int CustomerID)
        {
            // Connect db
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            CustomerVM oCustInfo = new CustomerVM();


            // Get the customer info
               var oC = (from c in estEnt.Customers
                             where c.CustomerID == CustomerID
                             join b in estEnt.CustomerBalances on c.CustomerID equals b.CustomerID into joinedT
                             from b in joinedT.DefaultIfEmpty()
                             select new CustomerVM
                             {
                                 City = c.City,
                                 Email = c.Email,
                                 FirstName = c.FirstName,
                                 LastName = c.LastName,
                                 Phone = c.Phone,
                                 State = c.State,
                                 Street1 = c.Street1,
                                 Street2 = c.Street2,
                                 Zip = c.Zip,
                                 Amount = b.Balance == null ? 0 : (decimal)b.Balance,
                                 CustomerID = c.CustomerID 
                             }).FirstOrDefault();

               if (oC != null)
               {
                   oCustInfo.City = oC.City;
                   oCustInfo.Email = oC.Email;
                   oCustInfo.FirstName = oC.FirstName;
                   oCustInfo.LastName = oC.LastName;
                   oCustInfo.Phone = oC.Phone;
                   oCustInfo.State = oC.State;
                   oCustInfo.Street1 = oC.Street1;
                   oCustInfo.Street2 = oC.Street2;
                   oCustInfo.Zip = oC.Zip;
                   oCustInfo.Amount = oC.Amount;
                   oCustInfo.CustomerID = oC.CustomerID; 
               }

                // Get the customers transactions
                List<TransactionVM> lTransactions = new List<TransactionVM>();

                var oTrans = from t in estEnt.Transactions
                                join c in estEnt.Customers on t.CustomerID equals c.CustomerID 
                                join l in estEnt.Locations on t.LocationID equals l.LocationID into joinedT
                                from l in joinedT.DefaultIfEmpty()
                                orderby t.TransactionDate descending
                                where t.CustomerID == CustomerID
                                select new { t.Amount, t.TransactionDate, c.FirstName, c.LastName, l.LocationName, l.City, l.State };

                foreach (var t1 in oTrans)
                {
                    string sLoc = t1.LocationName + " - " + t1.City + ", " + t1.State;
                    if (t1.LocationName != null)
                    {
                        sLoc = t1.LocationName;

                        if (t1.City != "")
                        {
                            sLoc += " - " + t1.City;
                        }

                        if (t1.State != "")
                        {
                            sLoc += ", " + t1.State;
                        }
                    }
                    else
                    {
                        sLoc = "None";
                    }

                    lTransactions.Add(new TransactionVM { Name = t1.LastName + ", " + t1.FirstName, Amount = Convert.ToDecimal(t1.Amount), Location = sLoc , TransactionDate = Convert.ToDateTime(t1.TransactionDate).ToString() });
                }

                if (oCustInfo != null)
                {
                    oCustInfo.CustomerTransactions = lTransactions;
                }

                return oCustInfo;

        }


        // Export customer transactions to csv
        public void ExportDataTableToCsv(int CustomerID, string sType)
        {
            // Connect to db
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            // Set up datatable
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Amount", typeof(int));
            dt1.Columns.Add("TransactionDate", typeof(string));
            dt1.Columns.Add("CustomerID", typeof(string));//not a typo, sadly.
            dt1.Columns.Add("LocationID", typeof(string));//not a typo, sadly.


            // Get the transactions
            var oTrans = from t in estEnt.Transactions
                         join c in estEnt.Customers on t.CustomerID equals c.CustomerID
                         join l in estEnt.Locations on t.LocationID equals l.LocationID into joinedT
                         from l in joinedT.DefaultIfEmpty()
                         orderby t.TransactionDate descending
                         where t.CustomerID == CustomerID
                         select new { t.Amount, t.TransactionDate, c.FirstName, c.LastName, l.LocationName, l.City, l.State };


            // Populate table
            foreach (var t1 in oTrans)
            {

                string sLoc = t1.LocationName + " - " + t1.City + ", " + t1.State;
                if (t1.LocationName != null)
                {
                    sLoc = t1.LocationName;

                    if (t1.City != "")
                    {
                        sLoc += " - " + t1.City;
                    }

                    if (t1.State != "")
                    {
                        sLoc += ", " + t1.State;
                    }
                }
                else
                {
                    sLoc = "None";
                }

                DataRow row = dt1.NewRow();
                row["Amount"] = Convert.ToDecimal(t1.Amount);
                row["TransactionDate"] = Convert.ToDateTime(t1.TransactionDate).ToString();
                row["CustomerID"] = t1.LastName + ", " + t1.FirstName;
                row["LocationID"] = sLoc;
              
                dt1.Rows.Add(row);
            }

          
            //* This function will take a databable and render it as a CSV [Excel] file

            //* Declare vars
            DataTable toExcel = dt1.Copy();
            HttpContext context = HttpContext.Current; 
            int i = 0;
            context.Response.Clear();

            //* Process columns
            foreach (DataColumn column in toExcel.Columns)
            {
                context.Response.Write(column.ColumnName + ",");
            }

            context.Response.Write(Environment.NewLine);

            //* Process rows

            foreach (DataRow row in toExcel.Rows)
            {
                for (i = 0; i <= toExcel.Columns.Count - 1; i++)
                {
                    context.Response.Write(row[i].ToString().Replace(",", string.Empty) + ",");
                }
                context.Response.Write(Environment.NewLine);
            }

            //* Render the file
            context.Response.ContentType = "text/csv";
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + sType + "_" + DateTime.Now.ToShortDateString() + ".csv");
            context.Response.End();

        }

    }
}