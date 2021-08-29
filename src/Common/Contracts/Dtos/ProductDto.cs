﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Dtos
{
    [Serializable]
    public class ProductDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int InitialOnHand { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
    public enum ProductStatus
    {
        Pending = 0,
        SalesIsOk = 2,
        InventoryIsOk = 4,
        Completed = 6,
        Failed=8
    }
}
