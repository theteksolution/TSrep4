/*  
TransactionBL
Business logic for reporting transactions
LR 04142015
 
This will get and filter all transactions
 
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EarthSkyTime.Models;
using System.Data;
using System.Web.Mvc;
using EarthSkyTime.ViewModels; 


namespace EarthSkyTime.BusinessLogic
{
    public class TransactionsBL
    {
        // Get transactions
        public List<TransactionVM> GetTransactions(FormCollection collection = null)
        {

            List<TransactionVM> lTransactions = new List<TransactionVM>();

            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();


            // get all transactions
            var oTrans = from t in estEnt.Transactions
                         join c in estEnt.Customers on t.CustomerID equals c.CustomerID
                         join l in estEnt.Locations on t.LocationID equals l.LocationID into joinedT
                         from l in joinedT.DefaultIfEmpty()
                         orderby t.TransactionDate descending
                         select new { t.Amount, t.TransactionDate, c.FirstName, c.LastName, l.LocationName, l.City, l.State, LocationID = l.LocationID == null ? 0 : l.LocationID };

            
            // Build filters, by date and location
            if (collection != null)
            {
                if (collection["dateFrom"] != "")
                {
                    DateTime dFrom = Convert.ToDateTime(collection["dateFrom"].ToString());
                    oTrans = oTrans.Where(m => m.TransactionDate >= dFrom); 
                }

                if (collection["dateTo"] != "")
                {
                    DateTime dTo = Convert.ToDateTime(collection["dateTo"].ToString());
                    oTrans = oTrans.Where(m => m.TransactionDate <= dTo);
                }

                if (collection["selectLocation"] != "")
                {
                    int iLocation = Convert.ToInt32(collection["selectLocation"].ToString());
                    if (iLocation > 0)
                    {
                        oTrans = oTrans.Where(m => m.LocationID == iLocation);
                    }
                }
            }
          

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


                lTransactions.Add(new TransactionVM { Name = t1.LastName + ", " + t1.FirstName, Amount = Convert.ToDecimal(t1.Amount), Location = sLoc, TransactionDate = Convert.ToDateTime(t1.TransactionDate).ToString() });
            }

            return lTransactions;

        }
        
        // Get all locations for filtering
        public SelectList BuildLocationSelect(int LocationID = 0)
        {
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            List<SelectListItem> lLocations = new List<SelectListItem>();
            lLocations.Add(new SelectListItem { Text = "Select a Location", Value = "0" });

            var oLoc = from l in estEnt.Locations
                       select new { l.LocationName, l.LocationID };

            foreach (var o in oLoc)
            {
                lLocations.Add(new SelectListItem { Text = o.LocationName, Value = o.LocationID.ToString() });
            }

            if (LocationID != 0)
            {
                return new SelectList(lLocations, "Value", "Text", LocationID.ToString());
            }
            else
            {
                return new SelectList(lLocations, "Value", "Text");
            }

        }


        // Export the transactions as csv
        public void ExportDataTableToCsv(string sType, string dateFrom, string dateTo, string location)
        {

            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Amount", typeof(int));
            dt1.Columns.Add("TransactionDate", typeof(string));
            dt1.Columns.Add("CustomerID", typeof(string));//not a typo, sadly.
            dt1.Columns.Add("LocationID", typeof(string));//not a typo, sadly.



            var oTrans = from t in estEnt.Transactions
                         orderby t.TransactionDate descending
                         join c in estEnt.Customers on t.CustomerID equals c.CustomerID
                         join l in estEnt.Locations on t.LocationID equals l.LocationID into joinedT
                         from l in joinedT.DefaultIfEmpty()
                         select new { t.Amount, t.TransactionDate, c.FirstName, c.LastName, l.LocationName, l.City, l.State, LocationID = l.LocationID == null ? 0 : l.LocationID   };

            if (dateFrom != "")
            {
                DateTime dtFrom = Convert.ToDateTime(dateFrom);
                oTrans = oTrans.Where(m => m.TransactionDate >= dtFrom);
            }

            if (dateTo != "")
            {
                DateTime dtTo = Convert.ToDateTime(dateTo);
                oTrans = oTrans.Where(m => m.TransactionDate <= dtTo);
            }

            if (location != "0")
            {
                int iLocation = Convert.ToInt32(location);
                oTrans = oTrans.Where(m => m.LocationID == iLocation);
            }

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