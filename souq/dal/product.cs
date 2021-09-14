using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace souq.dal
{
    public class product
    {
        public int product_id { set; get; }

        public string product_name { set; get; }
        public string descr { set; get; }
        public decimal price { set; get; }
        public string picture { set; get; }

    }
}