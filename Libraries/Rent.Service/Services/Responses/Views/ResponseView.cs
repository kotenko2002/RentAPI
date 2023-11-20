using Rent.Entities.Responses;

namespace Rent.Service.Services.Responses.Views
{
    public class ResponseView
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
