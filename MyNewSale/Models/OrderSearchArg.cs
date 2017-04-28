using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewSale.Models
{
    public class OrderSearchArg
    {
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        public string OrderDate { get; set; }

        /// <summary>
        /// 業務(員工)代號
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 出貨公司代號
        /// </summary>
        public string ShipperID { get; set; }

        /// <summary>
        /// 需要日期
        /// </summary>
        public string RequireDdate { get; set; }

        /// <summary>
        /// 出貨日期
        /// </summary>
        public string ShippedDate { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderID { get; set; }

        public string DeleteOrderId { get; set; }

        public string UpdateOrderId { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public string ProductID { get; set; }
    }
}