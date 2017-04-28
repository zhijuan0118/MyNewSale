using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyNewSale.Models
{
    public class ProductService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.ToString();
        }
        /// <summary>
        /// 取得產品
        /// </summary>
        /// <returns></returns>
        public List<Models.Product> GetProduct()
        {
            DataTable dt = new DataTable();
            string sql = @"Select Productid,UnitPrice From Production.Products";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return MapProductList(dt);


        }
        private List<Models.Product> MapProductList(DataTable product)
        {


            List<Models.Product> result = new List<Models.Product>();
            foreach (DataRow row in product.Rows)
            {
                result.Add(new Product()
                {
                    ProductID = row["ProductID"].ToString(),
                    UnitPrice = Convert.ToDouble(row["UnitPrice"])
                });
            }

            return result;
        }
    }
}