/* 
 LocationBL
 Business logic for location information
 LR 04132015 
 
 This will get and update location information
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EarthSkyTime.Models;
using EarthSkyTime.ViewModels;

namespace EarthSkyTime.BusinessLogic
{
    public class LocationsBL
    {
        // Add or update a location.
        public void UpdateLocation(FormCollection collection)
        {
            // Form variables

            int locationID = Convert.ToInt32(collection["selectLocation"].ToString());
            string sIsUpdate = collection["IsUpdate"].ToString();
            string sName = collection["LocationName"].ToString();
            string sStreet1 = collection["Street1"].ToString();
            string sStreet2 = collection["Street2"].ToString();
            string sCity = collection["City"].ToString();
            string sState = collection["State"].ToString();
            string sZip = collection["Zip"].ToString();


            // Connect to db
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();


            // Add a new location
            if (locationID == 0)
            {
                Location oLoc = new Location() { City = sCity, LocationName = sName, State = sState, Street1 = sStreet1, Street2 = sStreet2, Zip = sZip };
                estEnt.AddToLocations(oLoc); 
            }
            else
            {
                // Update existing location
                var oLoc = (from l in estEnt.Locations
                            where l.LocationID == locationID
                            select l).FirstOrDefault();
                if (oLoc != null)
                {
                    oLoc.City = sCity;
                    oLoc.LocationName = sName;
                    oLoc.State = sState;
                    oLoc.Street1 = sStreet1;
                    oLoc.Street2 = sStreet2;
                    oLoc.Zip = sZip;
                }

            }
            estEnt.SaveChanges();

        }


        // Get the location information
        public LocationVM GetLocation(int LocationID)
        {
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            LocationVM oLocVM = new LocationVM();

            var oLocs = (from l in estEnt.Locations
                         where l.LocationID == LocationID
                         select new { l.LocationName, l.Street1, l.Street2, l.City, l.State, l.Zip }).FirstOrDefault();

            if (oLocs != null)
            {
                oLocVM.City = oLocs.City;
                oLocVM.LocationName = oLocs.LocationName;
                oLocVM.State = oLocs.State;
                oLocVM.Street1 = oLocs.Street1;
                oLocVM.Street2 = oLocs.Street2;
                oLocVM.Zip = oLocs.Zip;
            }

            return oLocVM;
        }


        // Get all locations for dropdown
        public SelectList BuildLocationSelect(int LocationID = 0)
        {
            EarthSkyTimeEntities estEnt = new EarthSkyTimeEntities();

            List<SelectListItem> lLocations = new List<SelectListItem>();
            lLocations.Add(new SelectListItem { Text = "Select location or keep to add new", Value = "0" });


            // Get all locations
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
    }
}