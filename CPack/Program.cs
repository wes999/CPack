using System.Diagnostics;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using PowerArgs;
using SharpCompress.Archives.Zip;
using Spectre.Console;
using Formatting = Newtonsoft.Json.Formatting;

namespace CPack
{
    public class InstallArgs
    {
        [ArgRequired, ArgDescription("The Name Of The Directory Containing The CPack.json Package File"), ArgPosition(1)]
        public string PackageName { get; set; }

        [ArgRequired, ArgDescription("The Name Of The Directory Containing The Target vcxproj File"), ArgPosition(2)]
        public string ProjName { get; set; }
    }

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

            Package.CreationTime = DateTime.Now;
            Package.LastUpDateTime = DateTime.Now;

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Newtonsoft.Json.Formatting.Indented));
        }

        [ArgActionMethod, ArgDescription("Updates The Current Package")]
        public void Update(string message)
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
            
            Package!.Update(message);
            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Formatting.Indented));
        }

        [ArgActionMethod, ArgDescription("Installs The Packages Dependencies, includes and Dlls to The Specified Project")]
        public void Install(InstallArgs args)
        {
            if (File.Exists($"{args.PackageName}\\CPack.json"))
            {
                AnsiConsole.MarkupLine($"[red]ERROR:[/] Could Not Find CPack.json File In {args.PackageName}");
            }

            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText( $"{args.PackageName}\\CPack.json"));
            Package!.Install(args.ProjName);
        }

        [ArgActionMethod]
        public void SetupAndInstall(InstallArgs args)
        {
            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText($"{args.PackageName}\\CPack.json"));
            Package!.Localize();
            File.WriteAllText($"{args.PackageName}\\CPack.json", JsonConvert.SerializeObject(Package));

            Package.Install(args.ProjName);
        }

        [ArgActionMethod]
        public void Test()
        {
            AnsiConsole.Progress().Columns(
                    new ProgressColumn[]
                    {
                        new SpinnerColumn(),
                        new ElapsedTimeColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn()
                    })
                .Start(ctx =>
                {
                    ProgressTask task = ctx.AddTask("a");

                    while (!task.IsFinished)
                    {
                        Thread.Sleep(100);
                        task.Increment(1);
                    }
                });
        }

        [ArgActionMethod, ArgDescription("Downloads The Package From The Url, Localizes It And Installs It To The Specified Project")]
        public async Task SetupAndInstallFromUrl(string url, string destination, string projName)
        {
            HttpClient client = new HttpClient();
            byte[] bytes = await client.GetByteArrayAsync(url);

            ZipArchive archive = ZipArchive.Open(new MemoryStream(bytes));
            archive.ExtractAllEntries();

            Package = JsonConvert.DeserializeObject<Package>(await File.ReadAllTextAsync($"{destination}\\CPack.json"));
            Package!.Localize();
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