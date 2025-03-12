﻿using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? QuantityId { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
