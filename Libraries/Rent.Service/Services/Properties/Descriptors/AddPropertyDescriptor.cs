﻿using Microsoft.AspNetCore.Http;
using Rent.Entities.Properties;

namespace Rent.Service.Services.Properties.Descriptors
{
    public class AddPropertyDescriptor
    {
        public string LandlordId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public PropertyStatus Status { get; set; }

        public IFormFile[] Photos { get; set; }
    }
}
