using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthSkyTime.ViewModels
{
    public class TransactionVM
    {
        public string TransactionDate { get; set; }
        public string Location { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
    }

}