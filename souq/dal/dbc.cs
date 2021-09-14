using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace souq.dal
{
    public class Dbc
    {
        public static SqlConnection conn = new SqlConnection("Data Source = WIN-HQ8EO8P8GBF; Initial Catalog = souq1;Integrated Security=True; User Id =sa; Password=61158");
      //  public static SqlConnection conn = new SqlConnection("Data Source = AMER-PC\\SQLEXPRESS; Initial Catalog = souq1;Integrated Security=True");

    }
}