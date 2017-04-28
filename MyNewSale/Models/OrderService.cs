using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyNewSale.Models
{
    public class OrderService
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
		/// 新增訂單
		/// </summary>
		/// <param name="order"></param>
		/// <returns>訂單編號</returns>
		public int InsertOrder(Models.Order order)
        {
            string sql = @" Insert INTO Sales.Orders
						 (
							CustomerID,EmployeeID,Orderdate,RequireDdate,ShippedDate,ShipperID,Freight,
							ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry
						)
						VALUES
						(
							@CustomerID,@EmployeeID,@OrderDate,@RequireDdate,@ShippedDate,@ShipperID,@Freight,
							@ShipName,@ShipAddress,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipCountry
						)
						Select SCOPE_IDENTITY()
						";
            int orderId;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", order.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", order.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry));

                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
            return orderId;

        }

        /// <summary>
        /// 新增訂單明細
        /// </summary>
        /// <param name="OrderDetail"></param>
        /// <param name="orderid"></param>
        public void InsertOrderDetail(List<OrderDetail> OrderDetail, int orderid)
        {

            string sql = @"INSERT INTO [Sales].[OrderDetails]
                            ([OrderID]
                            ,[ProductID]
                            ,[UnitPrice]
                            ,[Qty])
                        VALUES
                            (@OrderID
                            ,@ProductID
                            ,@UnitPrice
                            ,@Qty)";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                try
                {
                    conn.Open();
                    for (int i = 0; i < OrderDetail.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.Add(new SqlParameter("@OrderID", orderid));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", OrderDetail[i].ProductID));
                        cmd.Parameters.Add(new SqlParameter("@UnitPrice", OrderDetail[i].UnitPrice));
                        cmd.Parameters.Add(new SqlParameter("@Qty", OrderDetail[i].Qty));

                        cmd.ExecuteScalar();
                    }
                }
                catch
                {
                    DeleteOrderById(orderid.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 透過訂單ID取得訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<Models.Order> GetOrderById(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CompanyName,
					A.EmployeeID,C.Lastname+ C.Firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where  A.OrderID Like @OrderID";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return this.MapOrderDataToList(dt);


        }

        /// <summary>
        /// 修改訂單前將該訂單資料顯示出來
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrderByIdForUpdate(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CompanyName,
					A.EmployeeID,C.Lastname+ C.Firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where  A.OrderID=@OrderID";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<Models.OrderDetail> OrderDetail = GetOrderDetailById(orderId);

            Models.Order result = new Models.Order();
            foreach (DataRow row in dt.Rows)
            {
                result.CustomerID = (int)row["CustomerID"];
                result.CompanyName = row["CompanyName"].ToString();
                result.EmployeeID = (int)row["EmployeeID"];
                result.EmployeeName = row["EmployeeName"].ToString();
                result.Freight = (decimal)row["Freight"];
                result.Orderdate = ChangeDate(row["Orderdate"].ToString());
                result.OrderID = row["OrderID"].ToString();
                result.RequireDdate = ChangeDate(row["RequireDdate"].ToString());
                result.ShipAddress = row["ShipAddress"].ToString();
                result.ShipCity = row["ShipCity"].ToString();
                result.ShipCountry = row["ShipCountry"].ToString();
                result.ShipName = row["ShipName"].ToString();
                result.ShippedDate = ChangeDate(row["ShippedDate"].ToString());
                result.ShipperID = (int)row["ShipperID"];
                result.ShipperName = row["ShipperName"].ToString();
                result.ShipPostalCode = row["ShipPostalCode"].ToString();
                result.ShipRegion = row["ShipRegion"].ToString();
            }
            result.OrderDetail = OrderDetail;

            return result;
        }

        /// <summary>
        /// 更改日期型態
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string ChangeDate(string date)
        {
            string d;
            if (date != "")
            {
                DateTime datetime = Convert.ToDateTime(date);
                d = datetime.ToString("yyyy-MM-dd");
            }
            else
            {
                d = null;
            }


            return d;
        }


        /// <summary>
        /// 取得所有訂單資料
        /// </summary>
        /// <returns></returns>
        public List<Models.Order> GetOrder()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CompanyName,
					A.EmployeeID,C.Lastname+ C.Firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapOrderDataToList(dt);
        }


        /// <summary>
        /// 依照條件取得訂單資料
        /// </summary>
        /// <returns></returns>
        public List<Models.Order> GetOrderByCondtioin(Models.OrderSearchArg arg)
        {

            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CompanyName,
					A.EmployeeID,C.Lastname+ C.Firstname As EmployeeName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where (B.Companyname Like @CompanyName Or @CompanyName='') And
                          (A.OrderId=@OrderID Or @OrderID='') And
                          (A.EmployeeID= @EmployeeID Or @EmployeeID='') And
                          (A.ShipperId = @ShipperID Or @ShipperID='') And
                          (A.Orderdate = @Orderdate Or @Orderdate='') And
                          (A.ShippedDate= @ShippedDate Or @ShippedDate='') And
                          (A.RequireDdate = @RequireDdate Or @RequireDdate='')";


            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CompanyName", arg.CompanyName == null ? string.Empty : '%' + arg.CompanyName + '%'));
                cmd.Parameters.Add(new SqlParameter("@OrderID", arg.OrderID == null ? string.Empty : arg.OrderID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", arg.EmployeeID == null ? string.Empty : arg.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", arg.ShipperID == null ? string.Empty : arg.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", arg.ShippedDate == null ? string.Empty : arg.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", arg.RequireDdate == null ? string.Empty : arg.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", arg.OrderDate == null ? string.Empty : arg.OrderDate));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }


            return this.MapOrderDataToList(dt);
        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<Models.OrderDetail> GetOrderDetailById(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT OrderID,ProductID,UnitPrice,Qty FROM Sales.OrderDetails Where OrderID  = @orderid";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapOrderDetailToList(dt);
        }

        /// <summary>
        /// 刪除訂單明細
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int DeleteOrderDetailById(string orderId)
        {
            try
            {
                int result;
                string sql = "Delete FROM Sales.OrderDetails Where OrderID=@orderid";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
                    result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                if (result == 1)
                {
                    result = DeleteOrderById(orderId);
                    return result;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int DeleteOrderById(string orderId)
        {
            try
            {
                int result;
                string sql = "Delete FROM Sales.Orders Where OrderID=@orderid";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
                    result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder(Models.Order order)
        {
            string sql = @"UPDATE [Sales].[Orders]
                            SET [CustomerID] = @CustomerID
                            ,[EmployeeID] =@EmployeeID
                            ,[OrderDate] = @Orderdate
                            ,[RequiredDate] = @RequireDdate
                            ,[ShippedDate] = @ShippedDate
                            ,[ShipperID] = @ShipperID
                            ,[Freight] =@Freight
                            ,[ShipName] = @ShipName
                            ,[ShipAddress] =@ShipAddress
                            ,[ShipCity] = @ShipCity
                            ,[ShipRegion] = @ShipRegion
                            ,[ShipPostalCode] =@ShipPostalCode
                            ,[ShipCountry] = @ShipCountry
                            WHERE OrderID=@OrderID";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", order.Orderdate == null ? string.Empty : order.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", order.RequireDdate == null ? string.Empty : order.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate == null ? string.Empty : order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName == null ? string.Empty : order.ShipName));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress == null ? string.Empty : order.ShipAddress));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity == null ? string.Empty : order.ShipCity));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion == null ? string.Empty : order.ShipRegion));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode == null ? string.Empty : order.ShipPostalCode));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry == null ? string.Empty : order.ShipCountry));
                cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                cmd.ExecuteScalar();
                conn.Close();
            }

        }

        /// <summary>
        /// 更新訂單時移除某比訂單明細
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public int DeleteOrderDetailForUpdate(string OrderID, string ProductID)
        {
            try
            {
                int result;
                string sql = "DELETE FROM [Sales].[OrderDetails]  WHERE OrderID =@OrderID AND ProductID =@ProductID ";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@OrderID", OrderID));
                    cmd.Parameters.Add(new SqlParameter("@ProductID", ProductID));
                    result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新訂單明細
        /// </summary>
        /// <param name="OrderDetail"></param>
        /// <param name="OrderID"></param>
        public void UpdateOrderDetail(List<OrderDetail> OrderDetail, string OrderID)
        {

            List<OrderDetail> oldOrderDetail = GetOrderDetailById(OrderID);
            List<OrderDetail> NewOrderDetail = new List<Models.OrderDetail>();
            for (int i = 0; i < OrderDetail.Count; i++)
            {
                int c = 0;
                for (int j = 0; j < oldOrderDetail.Count; j++)
                {
                    if (OrderDetail[i].ProductID == oldOrderDetail[j].ProductID)
                    {
                        c = 1;
                        string sql = "UPDATE [Sales].[OrderDetails] SET [UnitPrice] = @UnitPrice,[Qty] =@Qty  WHERE OrderID=@OrderID AND ProductID=@ProductID";
                        using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            cmd.Parameters.Add(new SqlParameter("@OrderID", OrderID));
                            cmd.Parameters.Add(new SqlParameter("@ProductID", OrderDetail[i].ProductID));
                            cmd.Parameters.Add(new SqlParameter("@UnitPrice", OrderDetail[i].UnitPrice));
                            cmd.Parameters.Add(new SqlParameter("@Qty", OrderDetail[i].Qty));
                            cmd.ExecuteScalar();
                            conn.Close();
                        }
                        oldOrderDetail[i].ProductID = null;
                    }
                }
                if (c == 0)
                {
                    NewOrderDetail.Add(OrderDetail[i]);
                }
            }
            if (NewOrderDetail.Count > 0)
            {
                InsertOrderDetail(NewOrderDetail, Convert.ToInt32(OrderID));
            }
            for (int i = 0; i < oldOrderDetail.Count; i++)
            {
                if (oldOrderDetail[i].ProductID != null)
                {
                    string sql = "DELETE FROM [Sales].[OrderDetails]  WHERE OrderID =@OrderID AND ProductID =@ProductID ";
                    using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.Add(new SqlParameter("@OrderID", OrderID));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", oldOrderDetail[i].ProductID));
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
        }

        private List<Models.Order> MapOrderDataToList(DataTable orderData)
        {
            List<Models.Order> result = new List<Order>();


            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new Order()
                {
                    CustomerID = (int)row["CustomerID"],
                    CompanyName = row["CompanyName"].ToString(),
                    EmployeeID = (int)row["EmployeeID"],
                    EmployeeName = row["EmployeeName"].ToString(),
                    Freight = (decimal)row["Freight"],
                    Orderdate = row["Orderdate"].ToString(),
                    OrderID = row["OrderID"].ToString(),
                    RequireDdate = row["RequireDdate"].ToString(),
                    ShipAddress = row["ShipAddress"].ToString(),
                    ShipCity = row["ShipCity"].ToString(),
                    ShipCountry = row["ShipCountry"].ToString(),
                    ShipName = row["ShipName"].ToString(),
                    ShippedDate = row["ShippedDate"].ToString(),
                    ShipperID = (int)row["ShipperID"],
                    ShipperName = row["ShipperName"].ToString(),
                    ShipPostalCode = row["ShipPostalCode"].ToString(),
                    ShipRegion = row["ShipRegion"].ToString()
                });
            }
            return result;
        }

        private List<Models.OrderDetail> MapOrderDetailToList(DataTable orderData)
        {
            List<Models.OrderDetail> result = new List<Models.OrderDetail>();
            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new OrderDetail()
                {
                    OrderID = row["OrderID"].ToString(),
                    ProductID = row["ProductID"].ToString(),
                    UnitPrice = Convert.ToInt32(row["UnitPrice"]),
                    Qty = Convert.ToInt32(row["Qty"])
                });
            }
            return result;
        }

        public int SumOrderDetailPrice(List<Models.OrderDetail> OrderDetail)
        {
            int sum = 0;
            foreach (var row in OrderDetail)
            {
                sum += row.UnitPrice * row.Qty;
            }
            return sum;
        }
    }
}