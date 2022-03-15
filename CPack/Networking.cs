using System.Net;

namespace CPack
{
    public static class Networking
    {
        public static async Task Upload(string url)
        {
            HttpClient client = new HttpClient();

            byte[] bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync("", bytes);
        }
    }
}