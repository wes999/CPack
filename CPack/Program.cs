using System.Xml;
using Newtonsoft.Json;
using PowerArgs;
using SharpCompress.Archives.Zip;
using Spectre.Console;

namespace CPack
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class CPack
    {
        public Package Package = new Package();

        [ArgActionMethod, ArgDescription("Initializes A Package In The Current Directory")]
        public void Init()
        {
            Console.WriteLine("Package Name:");
            Package.Name = Console.ReadLine();

            Console.WriteLine("Package Description:");
            Package.Description = Console.ReadLine();

            Console.WriteLine("Package Include Directory:");
            Package.IncludePath = Console.ReadLine();

            Console.WriteLine("Package Binary Directory:");
            Package.GetDllFiles(Console.ReadLine()!);

            Console.WriteLine("Package Library Directory:");
            Package.GetLibFiles(Console.ReadLine()!);

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Newtonsoft.Json.Formatting.Indented));
        }

        [ArgActionMethod, ArgDescription("Installs The Packages Dependencies, includes and Dlls to The Specified Project")]
        public void Install(string packageName, string projName)
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText(packageName + "\\CPack.json"));
            Package!.Install(projName);
        }

        [ArgActionMethod]
        public void SetupAndInstall(string package, string projName)
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText($"{package}\\CPack.json"));
            Package!.Localize();
            File.WriteAllText($"{package}\\CPack.json", JsonConvert.SerializeObject(Package));

            Package.Install(projName);
        }

        [ArgActionMethod, ArgDescription("Sets The Packages Paths To Their Proper Local Paths")]
        public void Localize()
        {
            if (!File.Exists("CPack.json"))
            {
                AnsiConsole.MarkupLine("[red]ERROR[/] Directory Is Not A Package");
                return;
            }

            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
            Package!.Localize();

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Newtonsoft.Json.Formatting.Indented));
        }

        [ArgActionMethod, ArgDescription("Gets Info On The Current Package")]
        public void Info()
        {
            if (!File.Exists("CPack.json"))
            {
                AnsiConsole.MarkupLine("[red]ERROR[/] Directory Is Not A Package");
                return;
            }

            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
            Package!.Info();
        }

        [ArgActionMethod, ArgDescription("Extracts The Files From The Packed CPack File")]
        public void Unpack(string packageName)
        {
            var archive = ZipArchive.Open(packageName);
            archive.ExtractAllEntries();
        }

        [ArgActionMethod, ArgDescription("Packs The Current Package With The Zip Archiving System")]
        public void Pack()
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"))!;

            Package.Pack();
        }
        
        public static class Program
        {
            public static void Main(string[] args)
            {
                Args.InvokeAction<CPack>(args);
            }
        }
    }
}