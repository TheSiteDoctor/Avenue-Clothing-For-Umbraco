﻿using System.Collections.Generic;

namespace AvenueClothing.Api.Model
{
    public class GetVariantSkuFromSelectionRequest
    {
        public string ProductSku { get; set; }
        public IDictionary<string, string> VariantProperties { get; set; }
    }
}