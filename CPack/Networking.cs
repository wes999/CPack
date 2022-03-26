using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Spectre.Console;

namespace CPack
{
    public static class Networking
    {
        private static readonly Func<PackageForm, string> packageFormConverter = result => result.Name;  

        public static void GetPackages()
        {
            WebClient client = new WebClient();
            
            string data = client.DownloadString("https://localhost:7100/packages");
            PackageForm[] packages = JsonConvert.DeserializeObject<List<PackageForm>>(data).ToArray();

            SelectionPrompt<PackageForm> prompt = new SelectionPrompt<PackageForm>();
            prompt.Converter = packageFormConverter;
            prompt.AddChoices(packages);

            PackageForm selectedPackage = AnsiConsole.Prompt(prompt);
            PackageInfo(selectedPackage);
        }

        public static void PackageInfo(PackageForm package)
        {
            Console.WriteLine(package.Name);
            Console.WriteLine(package.Description);
            Console.WriteLine(package.CreationTime);
        }
    }

    public class PackageForm
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