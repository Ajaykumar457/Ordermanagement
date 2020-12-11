using BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Ordermanagement.Controllers
{
    public class OrderController : ApiController
    {
       
            DAL.DAL objbll = new DAL.DAL();
            Response res = new Response();
            DataSet ds = new DataSet();

        [HttpPost]
        public HttpResponseMessage Usermanagement(Usermanagement userdata)
        {
            try
            {
               
                        ds = objbll.Db_Bll(userdata, "login_Master");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["Result"].ToString().ToLower() == "true")
                        {
                            res.Status_cd = "1";
                            res.ds = ds;
                        }
                        else
                        {
                            res.Status_cd = "0";
                            res.ds = ds;
                        }


                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.Status_cd = "2";
                res.ErrorMsg = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
        }
        [HttpPost]
            public HttpResponseMessage Ordertransaction(Ordermaster orderinfo)
            {
                try
                {
                if (orderinfo.Condition != null && orderinfo.Condition != "")
                {
                    if (orderinfo.Condition == "AddOrders")
                    {
                        ds = objbll.Db_Bll(orderinfo, "Order_Transactions");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["Result"].ToString().ToLower() == "true")
                        {
                            Random generator = new Random();
                            String OTP = generator.Next(0, 1000000).ToString("D6");

                            string file_Name = "Q" + OTP;


                            string code = ds.Tables[0].Rows[0]["Order_ID"].ToString();
                            QRCoder.QRCodeGenerator qrcodegenerator = new QRCodeGenerator();
                            string url = code;
                            var qrData = qrcodegenerator.CreateQrCode(url, QRCoder.QRCodeGenerator.ECCLevel.H);
                            var qrcode = new QRCoder.QRCode(qrData);
                            using (Bitmap bitMap = qrcode.GetGraphic(20))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    byte[] byteImage = ms.ToArray();
                                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                                    var filePath = System.Web.HttpContext.Current.Server.MapPath("~/UploadDocuments/Qrcodes/") + Convert.ToString(DateTime.UtcNow.ToString("dd/MM/yyyy").Replace('/', '_') + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_") + "_" + file_Name;
                                    img.Save(filePath + ".Jpeg", ImageFormat.Jpeg);
                                }
                            }
                            string qimage = "~/UploadDocuments/Qrcodes/" + Convert.ToString(DateTime.UtcNow.ToString("dd/MM/yyyy").Replace('/', '_') + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_") + "_" + file_Name + ".Jpeg";

                            orderinfo.Condition = "GenQRcode";
                            orderinfo.QRcode = qimage;
                            orderinfo.Order_ID = code;
                            ds = objbll.Db_Bll(orderinfo, "Order_Transactions");

                            res.Status_cd = "1";
                            res.ds = ds;
                        }
                        else
                        {
                            res.Status_cd = "0";
                            res.ds = ds;
                        }
                    }
                    else
                    {
                        ds = objbll.Db_Bll(orderinfo, "Order_Transactions");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["Result"].ToString().ToLower() == "true")
                        {
                            res.Status_cd = "1";
                            res.ds = ds;
                        }
                        else
                        {
                            res.Status_cd = "0";
                            res.ds = ds;
                        }

                    }
                }
                else
                {
                    res.Status_cd = "3";
                    res.ErrorMsg = "Condition missing";
                }

                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
                catch (Exception ex)
                {
                    res.Status_cd = "2";
                    res.ErrorMsg = ex.Message.ToString();
                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
            }
        
    }
}
