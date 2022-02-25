using Newtonsoft.Json;
using PowerArgs;
using Spectre.Console;

namespace CPack
{
    public static class Program
    {
        public static Package Package { get; set; }

        public static void Main(string[] args)
        {
            Package = new Package();

            if (args[0] == "init")
            {
                Init();
            }

            if (args[0] == "info")
            {
                Load();
            }
        }

        public static void Load()
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));

            AnsiConsole.MarkupLine($"[bold]{Package.Name}[/]");
            Console.WriteLine(Package.Description);
        }

        public static void Init()
        {
            Console.WriteLine("Package Name:");
            Package.Name = Console.ReadLine();

            Console.WriteLine("Package Description:");
            Package.Description = Console.ReadLine();

            Console.WriteLine("Package Include Directory:");

            DirectoryInfo info = new DirectoryInfo(Console.ReadLine());
            Package.IncludePath = info.FullName;

            Console.WriteLine("Binary Directory:");
            string binPath = Console.ReadLine();
            string[] dlls = Directory.GetFiles(binPath);

            for (int i = 0; i < dlls.Length; i++)
            {
                if (dlls[i].EndsWith(".dll"))
                {
                    Package.Dlls.Add(dlls[i]);
                }
            }

            Console.WriteLine("Library Directory:");
            string path = Console.ReadLine();
            Package.LibPath = new FileInfo(path).FullName;

            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".lib"))
                {
                    Package.LibraryFiles.Add(files[1]);
                }
            }

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Formatting.Indented));
        }

        public static void Install(string package, string projName)
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(package + "\\CPack.json"));

            
        }
    }
}