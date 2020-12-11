using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BLL
{
    public class Ordermaster
    {
        public string sno { get; set; }
        public string Order_ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public string Mobile { get; set; }
        public string QRcode { get; set; }
        public string Condition { get; set; }

    }

    public class Usermanagement
    {
        public string Loginid { get; set; }
        public string Password { get; set; }
        public string usertype { get; set; }
    }


    public class Response
    {
        public string Status_cd { get; set; }
        public DataSet ds { get; set; }
        public string ErrorMsg { get; set; }
    }
}
