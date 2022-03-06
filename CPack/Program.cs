using Newtonsoft.Json;
using PowerArgs;
using Spectre.Console;

namespace CPack
{
    class CPack
    {
        public Package Package = new Package();

        [ArgActionMethod]
        public void Init()
        {
            Console.WriteLine("Package Name:");
            Package.Name = Console.ReadLine();

            Console.WriteLine("Package Description:");
            Package.Description = Console.ReadLine();

            Console.WriteLine("Package Include Directory:");

            Package.IncludePath = Console.ReadLine();

            Console.WriteLine("Binary Directory:");
            string binPath = Console.ReadLine();
            string[] dlls = Directory.GetFiles(binPath);

            for (int i = 0; i < dlls.Length; i++)
            {
                if (dlls[i].EndsWith(".dll"))
                {
                    FileInfo dll = new FileInfo(dlls[i]);

                    Package.Dlls.Add(dll.Name);
                }
            }

            Console.WriteLine("Library Directory:");
            string path = Console.ReadLine();
            Package.LibPath = path;

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

        [ArgActionMethod]
        public void Localize()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
            List<string> dlls = package.Dlls;

            package.Dlls.Clear();
            package.IncludePath = new FileInfo(package.IncludePath).FullName;
            package.LibPath = new FileInfo(package.LibPath).FullName;

            for (int i = 0; i < dlls.Count; i++)
            {
                package.Dlls.Add(new FileInfo(dlls[i]).FullName);
            }

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(package, Formatting.Indented));
        }

        [ArgActionMethod]
        public void Info()
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
        }
    }

    public static class Program
    {
        public static Package Package { get; set; }

        public static void Main(string[] args)
        {
            Args.InvokeAction<CPack>(args);
        }

        public static void Normalize()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));

            package.IncludePath = new FileInfo(package.IncludePath).FullName;
            package.LibPath = new FileInfo(package.LibPath).FullName;
            
            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(package, Formatting.Indented));
        }

        public static void Init()
        {
            Console.WriteLine("Package Name:");
            Package.Name = Console.ReadLine();

            Console.WriteLine("Package Description:");
            Package.Description = Console.ReadLine();

            Console.WriteLine("Package Include Directory:");

            Package.IncludePath = Console.ReadLine();

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