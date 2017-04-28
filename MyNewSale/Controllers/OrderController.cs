using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNewSale.Controllers
{
    public class OrderController : Controller
    {
        Models.CodeService codeService = new Models.CodeService();
        //
        // GET: /Order/
        public ActionResult Index()
        {
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.ShipCodeData = this.codeService.GetShipper();
            Models.OrderService orderService = new Models.OrderService();
            ViewBag.Result = orderService.GetOrder();
            return View("Index");
        }

        public ActionResult InsertOrder()
        {
            Models.ProductService ProductService = new Models.ProductService();
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.CustCodeData = this.codeService.GetCustomer();
            ViewBag.ShipCodeData = this.codeService.GetShipper();
            ViewBag.ProductCodeData = this.codeService.GetProduct();
            ViewBag.ProductPrice = ProductService.GetProduct();
            return View(new Models.Order());
            //return View("InsertOrder");
        }

        [HttpPost()]
        public ActionResult DoInsertOrder(Models.Order order)
        {
            Models.OrderService orderService = new Models.OrderService();
            int orderid = orderService.InsertOrder(order);
            orderService.InsertOrderDetail(order.OrderDetail, orderid);
            ModelState.Clear();
            return Index();
        }

        [HttpPost()]
        public ActionResult Index(Models.OrderSearchArg arg)
        {

            Models.OrderService orderService = new Models.OrderService();
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.ShipCodeData = this.codeService.GetShipper();
            if (arg.DeleteOrderId != null)
            {
                int s = orderService.DeleteOrderDetailById(arg.DeleteOrderId);
                if (s == 1)
                {
                    ViewBag.Result = orderService.GetOrder();
                }
            }
            else
            {

                ViewBag.Result = orderService.GetOrderByCondtioin(arg);
            }

            return View("index");
        }

        [HttpPost()]
        public ActionResult DoUpdateOrder(Models.Order UpdateData)
        {
            Models.OrderService orderService = new Models.OrderService();
            orderService.UpdateOrder(UpdateData);
            orderService.UpdateOrderDetail(UpdateData.OrderDetail, UpdateData.OrderID);
            ModelState.Clear();
            return Index();
        }

        [HttpGet]
        public ActionResult UpdateOrder(string Orderid)
        {
            Models.OrderService orderService = new Models.OrderService();
            Models.Order Order = orderService.GetOrderByIdForUpdate(Orderid);
            Models.ProductService ProductService = new Models.ProductService();
            //ViewBag.OrderData = Order;
            ViewBag.Sum = orderService.SumOrderDetailPrice(Order.OrderDetail);
            ViewBag.EmpCodeData = new SelectList(codeService.GetEmp(), "Value", "Text", Order.EmployeeID);
            ViewBag.ShipCodeData = new SelectList(codeService.GetShipper(), "Value", "Text", Order.ShipperID);
            ViewBag.CustCodeData = new SelectList(codeService.GetCustomer(), "Value", "Text", Order.CustomerID);
            ViewBag.ProductPrice = ProductService.GetProduct();
            ViewBag.ProductCodeData = codeService.GetProduct();


            return View(Order);
        }

        [HttpPost]
        public JsonResult DeleteOrderDetail(Models.UpdateJson orderdetail)
        {
            try
            {
                string orderid = orderdetail.OrderID;
                string productid = orderdetail.ProductID;
                Models.OrderService orderService = new Models.OrderService();
                int result = orderService.DeleteOrderDetailForUpdate(orderid, productid);
                if (result == 1)
                {
                    return this.Json(true);
                }
                else
                {
                    return this.Json(false);
                }

            }
            catch (Exception)
            {

                return this.Json(false);
            }

        }
        

    }
}