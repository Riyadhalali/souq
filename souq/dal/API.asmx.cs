using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace souq.dal
{
    /// <summary>
    /// Summary description for API
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class API : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(MessageName = "sign_up", Description = "تابع التسجيل للزبون وادخال معلوماته لأول مره ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void sign_up( string username, string phone, string password,string conf_password, string email,string country,string city,string address,string picture)
        {
           
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
           
            string file = "";
            DataTable rt1 = new DataTable();
            int customer_i = 0;
            try
            {
                if (password == conf_password)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("select * from customer where username like '" + username + "' ", Dbc.conn);

                    adapter.Fill(rt1);

                    if (rt1 != null && rt1.Rows.Count > 0)
                    {
                        mess = "User already exist";
                        result = 0;
                    }

                    else
                    {
                        if (picture != null && picture != "" )
                        {
                            string UNid = Convert.ToString(Guid.NewGuid());
                             file = UNid + ".jpg";
                            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(picture)))
                            {
                                using (Bitmap bm2 = new Bitmap(ms))
                                {
                                    bm2.Save(Server.MapPath("~/picture/" + UNid + ".jpg"));
                                }
                            }
                        }
                        else
                             file =".jpg";
                        sql = "insert into customer (username,password,phone,email,country,city,address,photo,flag_nofication) values (N'" + username + "','" + password + "','" + phone + "','" + email + "','" + country + "','" + city + "','" + address + "','" + file + "',1)";
                            SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                            dal.Dbc.conn.Open();
                            result = cmd.ExecuteNonQuery();
                            dal.Dbc.conn.Close();
                            if (result != 0)
                            {
                                mess = "Add successful  ";

                            }
                            else { mess = "Try again"; }



                            SqlDataReader reader;

                            string sql2 = "select customer_id from customer where username='" + username + "'and password='" + password + "' ";
                            SqlCommand cmd2 = new SqlCommand(sql2, Dbc.conn);
                            // cmd2.CommandType = CommandType.Text;

                            dal.Dbc.conn.Open();

                            reader = cmd2.ExecuteReader();
                            while (reader.Read())
                            {
                                customer_i = reader.GetInt32(0);


                            }

                            reader.Close();

                            dal.Dbc.conn.Close();
                        


                    }
                }
                else
                    mess = "password does not match ";
            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess,
                customer_id = customer_i
            };
            Context.Response.Write(sr.Serialize(jsonData));
        }



        [WebMethod(MessageName = "Login", Description = "  اتصال الزبون باليزرنيم-يعيد التابع الرقم ورساله تاكيد نجاح الدخول ورقم الزبون واسمه")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Login(string username, string password)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            int customer_i = 0;
            string name = "";
            string Message = "";
            string sql = "";


            try
            {
                SqlDataReader reader;

                sql = "select customer_id,username from customer where username='" + username + "'and password='" + password + "' ";
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;
                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customer_i = reader.GetInt32(0);
                    name = reader.GetString(1);

                }
                if (customer_i == 0)
                {
                    Message = " password or username false ";
                }
                else
                    Message = "login success";
                reader.Close();

                dal.Dbc.conn.Close();


            }
            catch (Exception ex)
            {
              //  Message = " cannot access to the data";
                Message = ex.Message;

                dal.Dbc.conn.Close();
            }

            var jsonData = new
            {
                customer_id = customer_i,
                message = Message,
                username = name

            };
            Context.Response.Write(sr.Serialize(jsonData));
        }

        [WebMethod(MessageName = "getdata_user_information", Description = "  ارجاع بيانات المستخديين")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getdata_user_information(int customer_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
          
            string name = "";
            string em = "";
            string pho = "";
            string coun = "";
            string cit= "";
            string phot = "";
            string add = "";
            string Message = "";
            string sql = "";


            try
            {
                SqlDataReader reader;

                sql = "select username,email,phone,country,city,photo,address from customer where customer_id=" + customer_id + "  ";
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;
                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                   
                    name = reader.GetString(0);
                    em = reader.GetString(1);
                    pho = reader.GetString(2);
                    coun = reader.GetString(3);
                    cit = reader.GetString(4);
                    phot = reader.GetString(5);
                    add = reader.GetString(6);
                    


                }
              
                reader.Close();

                dal.Dbc.conn.Close();


            }
            catch (Exception ex)
            {
                Message = " cannot access to the data";

                dal.Dbc.conn.Close();
            }

            var jsonData = new
            {
               
                username = name,
                email=em,
                phone=pho,
                country=coun,
                city=cit,
                picture=phot,
                address=add

            };
            Context.Response.Write(sr.Serialize(jsonData));
        }
        [WebMethod(MessageName = "about_us", Description = "    اعادة نص نبذه عنا وبيانات الاتصال  ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void about_us()
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string Message = "";
            string sql = "";
            string pho = "";
            string fac = "";
            string ema = "";
            string tw = "";
            string tel = "";

            try
            {
                SqlDataReader reader;

                sql = "select text,phone,facebook,email,twitter,telegram from about_us ";
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                cmd.CommandType = CommandType.Text;
                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    Message = reader.GetString(0);
                    pho = reader.GetString(1);
                    fac = reader.GetString(2);
                    ema = reader.GetString(3);
                    tw = reader.GetString(4);
                    tel = reader.GetString(5);
                }


                reader.Close();

                dal.Dbc.conn.Close();


            }
            catch (Exception ex)
            {
                Message = " cannot access to the data";

                dal.Dbc.conn.Close();
            }

            var jsonData = new
            {

                about_us = Message,
                phone=pho,
                facebook=fac,
                email=ema,
                twitter=tw,
                telegram=tel


            };
            Context.Response.Write(sr.Serialize(jsonData));
        }


        [WebMethod(MessageName = "update_user_information", Description = "تابع تعديل معلومات الزبون ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void update_user_information(int customer_id,string username, string phone, string password, string email, string country, string city, string address, string picture)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
         
           
            string sql = "update customer set ";
           
            try
            {


                if (picture != null || picture != "")
                {
                    string UNid = Convert.ToString(Guid.NewGuid());
                    string file = UNid + ".jpg";
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(picture)))
                    {
                        using (Bitmap bm2 = new Bitmap(ms))
                        {
                            bm2.Save(Server.MapPath("~/picture/" + UNid + ".jpg"));
                        }
                    }
                    sql = sql + " picture= '" + file + "' ";
                }
                if (username !=null || username!="")
                    sql = sql + " , username= '" + username + "' ";
                if (password != null || password != "")
                    sql = sql + " , password= '" + password + "' ";
                if (phone != null || phone != "")
                    sql = sql + " , phone= '" + phone + "' ";
                if (email != null || email != "")
                    sql = sql + " , email= '" + email + "' ";
                if (country != null || country != "")
                    sql = sql + " , country= '" + country + "' ";
                if (city != null || city != "")
                    sql = sql + " , city= '" + city + "' ";
                if (address != null || address != "")
                    sql = sql + " , address= '" + address + "' ";
                sql = sql + " where customer_id =" + customer_id + "";
                SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                        dal.Dbc.conn.Open();
                        result = cmd.ExecuteNonQuery();
                        dal.Dbc.conn.Close();
                        if (result != 0)
                        {
                            mess = "update success ";

                        }
                        else { mess = "Try again"; }

            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess
                
            };
            Context.Response.Write(sr.Serialize(jsonData));
        }



        [WebMethod(MessageName = "getdata_categories", Description = "اعادة الفئات     ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_categories( )
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<cat> offer = new List<cat>();

            try
            {
                SqlDataReader reader;
                string sql = "select cat_id,cat_name from cat  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cat h = new cat();

                    h.category_id = Convert.ToInt32(reader["cat_id"]);

                    h.category_name = reader["cat_name"].ToString();
                    
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }


        [WebMethod(MessageName = "getdata_product", Description = "اعادة السلع     ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_product(int category_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product> offer = new List<product>();

            try
            {
                SqlDataReader reader;
                string sql = "select product_id,product_name,price,descr,picture from product where  cat_id=" + category_id + "  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product h = new product();

                    h.product_id = Convert.ToInt32(reader["product_id"]);

                    h.product_name = reader["product_name"].ToString();
                    h.descr = reader["descr"].ToString();
                    h.price = Convert.ToDecimal(reader["price"]);
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }

        [WebMethod(MessageName = "get_most_sales_products", Description = "most_sales_products")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void get_most_sales()
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product> offer = new List<product>();

            try
            {
                SqlDataReader reader;
                string sql = "SELECT top 20 product_id,product_name,price,descr,picture from product ORDER BY RAND() ;";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product h = new product();

                    h.product_id = Convert.ToInt32(reader["product_id"]);

                    h.product_name = reader["product_name"].ToString();
                    h.descr = reader["descr"].ToString();
                    h.price = Convert.ToDecimal(reader["price"]);
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
          
            Context.Response.Write(sr.Serialize(offer));
        }


        [WebMethod(MessageName = "get_most_visited_product", Description = "most_visited_products")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void get_most_visited_product()
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product> offer = new List<product>();

            try
            {
                SqlDataReader reader;
                string sql = "SELECT top 15 product_id,product_name,price,descr,picture from product ORDER BY RAND()  ;";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product h = new product();

                    h.product_id = Convert.ToInt32(reader["product_id"]);

                    h.product_name = reader["product_name"].ToString();
                    h.descr = reader["descr"].ToString();
                    h.price = Convert.ToDecimal(reader["price"]);
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }
        [WebMethod(MessageName = "getdata_product_image", Description = "اعادة الصور للسلع      ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_product_image(int product_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product_img> offer = new List<product_img>();

            try
            {
                SqlDataReader reader;
                string sql = "select picture_id,picture_url from picture where  product_id=" + product_id + "  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product_img h = new product_img();

                    h.id = Convert.ToInt32(reader["picture_id"]);

                    h.picture = reader["picture_url"].ToString();
                   
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }

        [WebMethod(MessageName = "getdata_related_product", Description = " get related products      ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_related_product(int category_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product> offer = new List<product>();

            try
            {
                SqlDataReader reader;
                string sql = "select top 5 product_id,product_name,price,descr,picture from product  where  cat_id=" + category_id + "  ORDER BY RAND(); ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product h = new product();

                    h.product_id = Convert.ToInt32(reader["product_id"]);

                    h.product_name = reader["product_name"].ToString();
                    h.descr = reader["descr"].ToString();
                    h.price = Convert.ToDecimal(reader["price"]);
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }

        [WebMethod(MessageName = "turn_on_off_Notifications", Description = "تابع تشغيل واطفاء الاشعارات  للتشغيل الفلاج1 وللاطفاء الفلاج 0 ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void turn_on_off_Notifications(int customer_id,int flag)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
            int id = 0;


            try
            {

                        sql = "update customer set flag_nofication=" + flag + " where customer_id=" + customer_id + "";
                        SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                        dal.Dbc.conn.Open();
                        result = cmd.ExecuteNonQuery();
                        dal.Dbc.conn.Close();
                        if (result != 0)
                        {
                    if (flag==1)
                            mess = "notfication ON  ";
                    if (flag == 0)
                        mess = "notfication OFF ";
                }
                        else { mess = "Try again"; }



                        


                
            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess
               
            };
            Context.Response.Write(sr.Serialize(jsonData));
        }

        [WebMethod(MessageName = "add_orders", Description = "طلب منتج عن طريق الوصف والصوره  ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void add_orders( string picture ,string descr)
        {

            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
            int id = 0;
            DataTable rt1 = new DataTable();
            int customer_i = 0;
            try
            {
                
                    
                        if (picture != null || picture != "")
                        {
                            string UNid = Convert.ToString(Guid.NewGuid());
                            string file = UNid + ".jpg";
                            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(picture)))
                            {
                                using (Bitmap bm2 = new Bitmap(ms))
                                {
                                    bm2.Save(Server.MapPath("~/picture/" + UNid + ".jpg"));
                                }
                            }//0type للدلاه على انه طلب وسيتم البحث عنه عكس الطلبات الجاهزه 
                            sql = "insert into orders (type,picture,descr) values (0,N'" + file + "','" + descr + "')";
                            SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                            dal.Dbc.conn.Open();
                            result = cmd.ExecuteNonQuery();
                            dal.Dbc.conn.Close();
                            if (result != 0)
                            {
                                mess = "add success ";

                            }
                            else { mess = "Try again"; }



                            


                   
                }
                
            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess,
                customer_id = customer_i
            };
            Context.Response.Write(sr.Serialize(jsonData));
        }

        [WebMethod(MessageName = "add_to_card", Description = "تابع اضافة السلعة الى السلة  ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void add_to_card(int customer_id, int product_id, int Quantity)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
            int store_id = 0;
            int card_id = 0;
            DataTable rt1 = new DataTable();

            decimal price = 0;
            try
            {

                SqlDataReader reader;

                sql = "select price from product where product_id=" + product_id + " ";
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                cmd.CommandType = CommandType.Text;
                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    
                    price = reader.GetDecimal(0);
                }

               

                reader.Close();

                dal.Dbc.conn.Close();
                string sql1 = "insert into cards (customer_id,product_id,Quantity,price) values (" + customer_id + "," + product_id + "," + Quantity + "," + price + ")";
                SqlCommand cmd1 = new SqlCommand(sql1, dal.Dbc.conn);
                dal.Dbc.conn.Open();
                result = cmd1.ExecuteNonQuery();
                dal.Dbc.conn.Close();
                if (result != 0)
                {
                    mess = "Add successful   ";

                }
                else { mess = "Try again"; }
            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess

            };
            Context.Response.Write(sr.Serialize(jsonData));
        }



        [WebMethod(MessageName = "remove_cards", Description = " حذف السلة او بعض السلع منها  من القائمة لحذف سلعه واحده تمرر رقم السلعه العائد من التابع الاعلى ولحذف الكل(السله كامله) يمرر 0 كرقم للسلعه  ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void remove_cards(int product_id, int customer_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            DateTime dt = DateTime.Now;

            string mess = null;

            int result = 0;
            string sql = "";
            if (product_id == 0)
            {

                sql = " delete from cards where customer_id=" + customer_id + " ";
            }

            else
            {

                sql = " delete from cards where customer_id=" + customer_id + " and product_id=" + product_id + "";
            }

            try
            {

                SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                dal.Dbc.conn.Open();
                result = cmd.ExecuteNonQuery();
                dal.Dbc.conn.Close();
                if (result != 0)
                {
                    mess = "remove succ ";

                }
                else { mess = "try again"; }


            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                //  mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess,

            };
            Context.Response.Write(sr.Serialize(jsonData));
        }



        [WebMethod(MessageName = "update_Quantity", Description = "تابع لاضافه الكميه او انقاصها للسله ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void update_Quantity(int customer_id, int product_id, int Quantity)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
           


            try
            {

                sql = "update cards set Quantity=" + Quantity + " where customer_id=" + customer_id + " and product_id="+product_id+"";
                SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                dal.Dbc.conn.Open();
                result = cmd.ExecuteNonQuery();
                dal.Dbc.conn.Close();
                if (result != 0)
                {
                    
                        mess = "update success ";
                }
                else { mess = "try again"; }

                            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess

            };
            Context.Response.Write(sr.Serialize(jsonData));
        }


        [WebMethod(MessageName = "getdata_cards", Description = "اعادة السلع المضافه للسلة     ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_cards(int customer_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<cards> offer = new List<cards>();

            try
            {
                
                int q = 0;
                decimal pr = 0;

                SqlDataReader reader;
                string sql = "select cards.product_id,product_name,cards.price,picture,cards.Quantity from product,cards where  customer_id=" + customer_id + " and product.product_id=cards.product_id  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cards h = new cards();

                    h.product_id = Convert.ToInt32(reader["product_id"]);
                    q = Convert.ToInt32(reader["Quantity"]);
                    pr = Convert.ToDecimal(reader["price"]);
                    pr = (int)pr;//
                    h.product_name = reader["product_name"].ToString();
                   
                    h.price = q * pr;
                 
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }



        [WebMethod(MessageName = "getdata_image", Description = "اعادة صور العرض في بداية التطبيق   ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_image( )
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product_img> offer = new List<product_img>();

            try
            {
                SqlDataReader reader;
                string sql = "select picture_id,picture_url from picture where  product_id=0  ";
                // صور العرض تكون فيها رقم السلعه صفر 
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product_img h = new product_img();

                    h.id = Convert.ToInt32(reader["picture_id"]);

                    h.picture = reader["picture_url"].ToString();

                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }



        [WebMethod(MessageName = "getdata_group", Description = "اعادة الجمعيات المشترك فيها الزبون      ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_group(int customer_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<group> offer = new List<group>();

            try
            {
                SqlDataReader reader;
                string sql = "select group_id,group_name from group,group_descr where cudtomer_id="+customer_id+" and group.grouo_id=group_descr.group_id  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    group h = new group();

                    h.group_id = Convert.ToInt32(reader["group_id"]);

                    h.group_name = reader["group_name"].ToString();

                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }



        [WebMethod(MessageName = "getdata_group_member", Description = "اعادة الاعضاء المشتركين في جمعيه ما       ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_group_member(int group_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<groupmember> offer = new List<groupmember>();

            try
            {
                SqlDataReader reader;
                string sql = "select username from customer,group_descr where group_id=" + group_id + " and customer.customer_id=group_descr.customer_id  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    groupmember h = new groupmember();

                  //  h.group_id = Convert.ToInt32(reader["group_id"]);

                    h.customer_name = reader["username"].ToString();

                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }


        [WebMethod(MessageName = "getdata_group_descr", Description = "اعادة تفاصيل المجموعه عدد المشتركين والمبلغ       ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_group_descr(int group_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List< group_descr > offer = new List<group_descr>();

            try
            {
                SqlDataReader reader;
                string sql = "select total_amount,number_of_person from group where group_id=" + group_id + "";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    group_descr h = new group_descr();

                     h.number_of_person = Convert.ToInt32(reader["number_of_person"]);

                    h.total_amount = Convert.ToDecimal(reader["total_amount"]);

                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }


        [WebMethod(MessageName = "getdata_offer", Description = "اعادة العروض     ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_offer( )
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            List<product> offer = new List<product>();

            try
            {
                SqlDataReader reader;
                string sql = "select product_id,product_name,price,descr,picture from offer  ";

                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                //  cmd.CommandType = CommandType.Text;

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    product h = new product();

                    h.product_id = Convert.ToInt32(reader["product_id"]);

                    h.product_name = reader["product_name"].ToString();
                    h.descr = reader["descr"].ToString();
                    h.price = Convert.ToDecimal(reader["price"]);
                    h.picture = reader["picture"].ToString();
                    offer.Add(h);
                }

                reader.Close();
                dal.Dbc.conn.Close();
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

            /*  var jsonData = new
              {
                  name_hotoffer = name,
                  picture = imgdata
              };*/
            Context.Response.Write(sr.Serialize(offer));
        }


        [WebMethod(MessageName = "confirm_order", Description = "تأكيد الطلب بعد اضافة المنتجات للسلة وبعد تحديد وجهة التوصيل   ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void confirm_order(int card_id,decimal latitudes,decimal longitude)
        {

            JavaScriptSerializer sr = new JavaScriptSerializer();

            string mess = null;
            int result = 0;
            string sql = "";
            
            DataTable rt1 = new DataTable();
            int customer_i = 0;
            try
            {

          //0type للدلاه على انه طلب وسيتم البحث عنه عكس الطلبات الجاهزه 
          // رقم الزبون هو نفسه رقم السلة 
                    sql = "insert into orders (type,long,late,customer_id) values (1,"+longitude+","+latitudes+","+card_id+")";
                    SqlCommand cmd = new SqlCommand(sql, dal.Dbc.conn);
                    dal.Dbc.conn.Open();
                    result = cmd.ExecuteNonQuery();
                    dal.Dbc.conn.Close();
                    if (result != 0)
                    {
                        mess = "add success ";

                    }
                    else { mess = "Try again"; }





// عمل فلاج للسله بعد اضافتها للطلب لحتى ماتطلع مره اخرى 

  // يتم خصم المبلغ واجور التوصيل من اللوحه               

            }
            catch (Exception ex)
            {
                dal.Dbc.conn.Close();
                mess = ex.Message;
                // mess = "تأكد من صحة البيانات ";
            }
            var jsonData = new
            {
                message = mess,
                customer_id = customer_i
            };
            Context.Response.Write(sr.Serialize(jsonData));
        }


        [WebMethod(MessageName = "getdata_balance", Description = "اعاده الرصيد الكلي للزبون       ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getdata_balance(int customer_id)
        {
            JavaScriptSerializer sr = new JavaScriptSerializer();
            decimal amount1 = 0;
            decimal amount2 = 0;
            decimal a = 0;
            try
            {
                SqlDataReader reader;
                SqlDataReader reader2;
                string sql = "select amount from balance where cudtomer_id=" + customer_id + " and flag=0  ";
                string sql2 = "select amount from balance where cudtomer_id=" + customer_id + " and flag=1  ";
                SqlCommand cmd = new SqlCommand(sql, Dbc.conn);
                SqlCommand cmd2 = new SqlCommand(sql2, Dbc.conn);

                dal.Dbc.conn.Open();
                reader = cmd.ExecuteReader();
                reader2 = cmd2.ExecuteReader();
                while (reader.Read())
                {
                    amount1 = reader.GetDecimal(0);
                }

                reader.Close();
                while (reader2.Read())
                {
                    amount2 = reader2.GetDecimal(0);
                }
                reader2.Close();
                dal.Dbc.conn.Close();

                a = amount1 - amount2;
            }
            catch (Exception ex)
            {

                dal.Dbc.conn.Close();
            }

              var jsonData = new
              {
                  
                  amount = a
              };
            Context.Response.Write(sr.Serialize(jsonData));
        }

    }
}
