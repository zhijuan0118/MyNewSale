using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MyNewSale.Models
{
    /// <summary>
    /// 訂單明細
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// OrderDetail_訂單編號
        /// </summary>
        [DisplayName("訂單編號")]
        public string OrderID { get; set; }
        /// <summary>
        /// OrderDetail_產品編號
        /// </summary>
        [DisplayName("產品編號")]
        public string ProductID { get; set; }
        /// <summary>
        /// OrderDetail_單價
        /// </summary>
        [DisplayName("單價")]
        public int UnitPrice { get; set; }
        /// <summary>
        /// OrderDetail_數量
        /// </summary>
        [DisplayName("數量")]
        public int Qty { get; set; }
    }
}