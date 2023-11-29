using Rent.Entities.Responses;

namespace Rent.Service.Services.Responses.Views
{
    public class ResponseView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
