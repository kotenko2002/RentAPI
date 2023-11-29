using Rent.Entities.Responses;

namespace Rent.Service.Services.Responses.Descriptors
{
    public class ProcessResponseDescriptor
    {
        public int ResponseId { get; set; }
        public string LandlordId { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
