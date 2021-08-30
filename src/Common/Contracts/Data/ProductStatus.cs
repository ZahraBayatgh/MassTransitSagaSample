using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Data
{
    public enum ProductStatus
    {
        Pending = 0,
        SalesIsOk = 2,
        InventoryIsOk = 4,
        Completed = 6,
    }
}
