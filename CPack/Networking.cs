using System.Net;
using Newtonsoft.Json;

namespace CPack
{
    public static class Networking
    {
        public static async Task GetPackages()
        {
            HttpClient client = new HttpClient();

            string data = await client.GetStringAsync("localhost:7100/packages");

            List<PackageForm> packages = JsonConvert.DeserializeObject<List<PackageForm>>(data);
        }
    }

    class PackageForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Version { get; set; }
        public string IncludePath { get; set; }
        public string Description { get; set; }
        public string LibPath { get; set; }
        public string BinPath { get; set; }

        public DateTime LastUpDateTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}