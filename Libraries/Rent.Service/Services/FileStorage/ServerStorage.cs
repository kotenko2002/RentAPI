namespace Rent.Service.Services.FileStorage
{
    public class ServerStorage : IFileStorageService
    {
        private readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public void Test()
        {
            Console.WriteLine(basePath);
        }
    }
}
