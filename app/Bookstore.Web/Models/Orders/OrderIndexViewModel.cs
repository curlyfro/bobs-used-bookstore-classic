using System;
using System.Collections.Generic;
using System.Linq;
using Bookstore.Domain;
using Bookstore.Domain.Orders;

namespace Bookstore.Web.Models.Orders
{
    public class OrderIndexViewModel
    {
        public List<OrderIndexItemViewModel> OrderItems { get; set; }

        public OrderIndexViewModel(IEnumerable<Order> orders)
        {
            OrderItems = orders.Select(x => new OrderIndexItemViewModel
            {
                Id = x.Id,
                DeliveryDate = x.DeliveryDate,
                OrderStatus = x.OrderStatus.GetDescription(),
                SubTotal = x.SubTotal
            }).ToList();
        }
    }

    public class OrderIndexItemViewModel
    {
        public int Id { get; set; }

        public decimal SubTotal { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string OrderStatus { get; set; }
    }
}