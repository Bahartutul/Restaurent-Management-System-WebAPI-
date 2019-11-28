using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RestaurentApi.Models;

namespace RestaurentApi.Controllers
{
    public class OrdersController : ApiController
    {
        private ResContext db = new ResContext();

        // GET: api/Orders
        public System.Object GetOrders()
        {
            var result = (from a in db.Orders
                          join b in db.CustomerDetails on a.CustomerId equals b.CustomerId
                          select new
                          {
                              a.OrderId,
                              //a.OrderItems,
                              CustomaeName = b.Name,
                              a.OrderNo,
                              a.GTotal,
                              a.PmMethod
                          });
            return result;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            var order = (from a in db.Orders
                         where a.OrderId == id
                         select new
                         {
                             a.OrderId,
                             a.OrderNo,
                             a.PmMethod,
                             a.CustomerId,
                              a.GTotal,
                              DeletedItems=""
                         }).FirstOrDefault();
            var orders = (from a in db.OrderItems
                          join b in db.Items on a.ItemId equals b.ItemId
                          where a.OrderId == id
                          select new
                          {
                              a.OrderId,
                              a.OrderItemId,
                              a.ItemId,
                              b.Name,
                              b.Price,
                              a.Quantity,
                              Total = a.Quantity * b.Price
                          }).ToList();

            return Ok(new{ order,orders});
        }

     
      

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            try
            {
                if (order.OrderId == 0) { 
                db.Orders.Add(order);
                }
                else
                {
                    db.Entry(order).State = EntityState.Modified;
                }
                foreach (var row in order.OrderItems)
                {
                    if (row.OrderItemId == 0)
                    {
                        db.OrderItems.Add(row);
                    }
                    else
                    {
                        db.Entry(row).State = EntityState.Modified;
                    }
                }

                foreach(var id in order.DeletedItems.Split(',').Where(x => x != ""))
                {
                    OrderItem x = db.OrderItems.Find(Convert.ToInt32(id));
                    db.OrderItems.Remove(x);
                }
                db.SaveChanges();
                return Ok();
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}