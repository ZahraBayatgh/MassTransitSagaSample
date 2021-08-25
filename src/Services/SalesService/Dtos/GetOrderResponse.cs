using SalesService.Models;
using System;
using System.Collections.Generic;

namespace SalesService.Dtos
{
    public class GetOrderResponse
    {
        public GetOrderResponse(int customerId, string customerName, DateTime orderDate, List<OrderItem> orderItems)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            OrderDate = orderDate;
            OrderItems = orderItems;
        }

        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public DateTime OrderDate { get; private set; }
        public List<OrderItem> OrderItems { get; }
    }
}
