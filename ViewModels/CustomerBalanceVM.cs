using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthSkyTime.ViewModels
{
    public class CustomerBalanceVM
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Amount { get; set; }
        public DateTime LastTransactionDate { get; set; }
    }
}