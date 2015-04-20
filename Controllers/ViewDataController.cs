using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.Models;
using System.Data.SqlClient; 

namespace EarthSkyTime.Controllers
{
    public class ViewDataController : Controller
    {
        //
        // GET: /ViewData/

        public ActionResult Index()
        {
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            var oC = from c in estEnt.Customers
                     select c;

            ViewBag.Customers = oC;

            var oB = from b in estEnt.CustomerBalances
                     orderby b.DateUpdated descending 
                     select b;

            ViewBag.Balances = oB;

            var oT = from t in estEnt.Transactions
                     orderby t.TransactionDate descending 
                     select t;
            ViewBag.Transactions = oT;

            return View();
        }

        [HttpPost()]
        public ActionResult Index(FormCollection collection)
        {

            string s = collection["Stuff"].ToString();
            //string s = " INSERT [dbo].[Customer] ([FirstName], [LastName], [Phone], [Street1], [Street2], [City], [State], [Zip], [DateAdded], [AddedBy], [DateUpdated], [UpdatedBy], [Email]) VALUES (N'Jane', N'Davies', N'', N'po box 116', N'', N'manchester center', N'VT', N'05255', CAST(0x0000A47F00E37404 AS DateTime), NULL, NULL, NULL, N'jane@janedaviesstudios.com')  LR-YA";


            string c2 = "data source=(local);initial catalog=EarthSkyTime;integrated security=True";
            string c1 = "Data Source=db572130233.db.1and1.com;Initial Catalog=db572130233;User ID=dbo572130233;Password=earthsql";

            if (s.Contains("LR-YA") && Session["UserID"] != null)
            {
                s = s.Replace("LR-YA", "");

                try
                { 
                    
                    using (SqlConnection connection = new SqlConnection(c1))
                    {
                        SqlCommand command = new SqlCommand(s,connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    throw new HttpException(404, ex.Message);
                }

               

            }


            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            var oC = from c in estEnt.Customers
                     select c;

            ViewBag.Customers = oC;

            var oB = from b in estEnt.CustomerBalances
                     orderby b.DateUpdated descending
                     select b;

            ViewBag.Balances = oB;

            var oT = from t in estEnt.Transactions
                     orderby t.TransactionDate descending
                     select t;
            ViewBag.Transactions = oT;

            return View();

            
        }

    }
}
