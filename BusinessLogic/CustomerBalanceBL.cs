using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EarthSkyTime.ViewModels;
using EarthSkyTime.Models;
using System.Web.Mvc;
using System.Data;

namespace EarthSkyTime.BusinessLogic
{
    public class CustomerBalanceBL
    {
        public List<CustomerBalanceVM> GetCustomerBalances(FormCollection collection = null)
        {
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            CustomerBalanceVM oCustInfo = new CustomerBalanceVM();

            List<CustomerBalanceVM> lCustBal = new List<CustomerBalanceVM>();


            var oBal = from b in estEnt.CustomerBalances
                       join c in estEnt.Customers on b.CustomerID equals c.CustomerID
                       select new { c.FirstName, c.LastName, b.Balance, b.DateUpdated, c.Email, c.Phone };

            if (collection != null)
            {
                if (collection["filters"].ToString() == "Balance Ascending")
                {
                    oBal = oBal.OrderBy(m => m.Balance);
                }

                if (collection["filters"].ToString() == "Balance Descending")
                {
                    oBal = oBal.OrderByDescending(m => m.Balance);
                }

                if (collection["filters"].ToString() == "Date Ascending")
                {
                    oBal = oBal.OrderBy(m => m.DateUpdated);
                }

                if (collection["filters"].ToString() == "Date Descending")
                {
                    oBal = oBal.OrderByDescending(m => m.DateUpdated);
                }

                if (collection["filters"].ToString() == "Name Ascending")
                {
                    oBal = oBal.OrderBy(m => m.LastName).ThenBy(m => m.FirstName);
                }

                if (collection["filters"].ToString() == "Name Descending")
                {
                    oBal = oBal.OrderByDescending(m => m.LastName).ThenByDescending(m => m.FirstName);
                }
            }
            else
            {
                oBal = oBal.OrderByDescending(m => m.DateUpdated);
            }


            foreach (var b1 in oBal)
            {
                lCustBal.Add(new CustomerBalanceVM { Name = b1.LastName + ", " + b1.FirstName, Amount = Convert.ToDecimal(b1.Balance), Email = b1.Email, LastTransactionDate = Convert.ToDateTime(b1.DateUpdated), Phone = b1.Phone });
            }

            return lCustBal; 
        }



        // Export the transactions as csv
        public void ExportDataTableToCsv(string sType, string filter)
        {

            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Name", typeof(string));//not a typo, sadly.
            dt1.Columns.Add("Balance", typeof(int));
            dt1.Columns.Add("TransactionDate", typeof(string));
            dt1.Columns.Add("Email", typeof(string));//not a typo, sadly.
            dt1.Columns.Add("Phone", typeof(string));//not a typo, sadly.

            var oBal = from b in estEnt.CustomerBalances
                       join c in estEnt.Customers on b.CustomerID equals c.CustomerID
                       select new { c.FirstName, c.LastName, b.Balance, b.DateUpdated, c.Email, c.Phone };

            if (filter == "Balance Ascending")
            {
                oBal = oBal.OrderBy(m => m.Balance);
            }

            if (filter == "Balance Descending")
            {
                oBal = oBal.OrderByDescending(m => m.Balance);
            }

            if (filter == "Date Ascending")
            {
                oBal = oBal.OrderBy(m => m.DateUpdated);
            }

            if (filter == "Date Descending")
            {
                oBal = oBal.OrderByDescending(m => m.DateUpdated);
            }

            if (filter == "Name Ascending")
            {
                oBal = oBal.OrderBy(m => m.LastName).ThenBy(m => m.FirstName);
            }

            if (filter == "Name Descending")
            {
                oBal = oBal.OrderByDescending(m => m.LastName).ThenByDescending(m => m.FirstName);
            }

            foreach (var b1 in oBal)
            {
                DataRow row = dt1.NewRow();
                row["Name"] = b1.LastName + ", " + b1.FirstName;
                row["Balance"] = Convert.ToDecimal(b1.Balance);
                row["TransactionDate"] = Convert.ToDateTime(b1.DateUpdated).ToString();
                row["Email"] = b1.Email;
                row["Phone"] = b1.Phone;
             
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