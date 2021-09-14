using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace souq.dal
{
    public class cards
    {
        public int product_id { set; get; }

        public string product_name { set; get; }
        public string picture { set; get; }
        public decimal price { set; get; }
    }
}