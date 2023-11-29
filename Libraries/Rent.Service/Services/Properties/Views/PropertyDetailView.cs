﻿namespace Rent.Service.Services.Properties.Views
{
    public class PropertyDetailView
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public PhotoView[] Photos { get; set; }
    }
}
