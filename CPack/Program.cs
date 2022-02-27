using Newtonsoft.Json;
using Spectre.Console;

namespace CPack
{
    public static class Program
    {
        public static Package Package { get; set; }

        public static void Main(string[] args)
        {
            Package = new Package();


            if (args.Length >= 1)
            {
                if (args[0] == "init")
                {
                    Init();
                }

                if (args[0] == "info")
                {
                    Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
                    Package.Info();
                }

                if (args[0] == "install")
                {
                    Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(args[1] + "\\CPack.json"));
                    Package.Install(args[2]);
                }

                if (args[0] == "pack")
                {
                    Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(args[1] + "\\CPack.json"));
                    Package.Pack();
                }
            }

            else
            {
                Menu();
            }
        }

        public static void Menu()
        {

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
                    FileInfo dll = new FileInfo(dlls[i]);

                    Package.Dlls.Add(dll.FullName);
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
                    Package.LibraryFiles.Add(new FileInfo(files[i]).Name);
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