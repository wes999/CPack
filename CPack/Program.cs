using Newtonsoft.Json;
using PowerArgs;
using Spectre.Console;

namespace CPack
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
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

            Console.WriteLine("Package Binary Directory:");
            Package.GetDllFiles(Console.ReadLine()!);

            Console.WriteLine("Package Library Directory:");
            Package.GetLibFiles(Console.ReadLine()!);

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(Package, Formatting.Indented));
        }

        [ArgActionMethod]
        public void Localize()
        {
            if (!File.Exists("CPack.json"))
            {
                AnsiConsole.MarkupLine("[red]ERROR[/] Directory Is Not A Package");
                return;
            }

            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));
            List<string> dlls = package.DllFiles;

            package.DllFiles.Clear();
            package.IncludePath = new FileInfo(package.IncludePath).FullName;
            package.LibPath = new FileInfo(package.LibPath).FullName;

            for (int i = 0; i < dlls.Count; i++)
            {
                package.DllFiles.Add(new FileInfo(dlls[i]).FullName);
            }

            File.WriteAllText("CPack.json", JsonConvert.SerializeObject(package, Formatting.Indented));
        }

        [ArgActionMethod]
        public void Info()
        {
            if (!File.Exists("CPack.json"))
            {
                AnsiConsole.MarkupLine("[red]ERROR[/] Directory Is Not A Package");
                return;
            }

            Package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("CPack.json"));

            AnsiConsole.MarkupLine($"[bold]{Package.Name}[/]");
            AnsiConsole.MarkupLine(Package.Description);

            string choice = AnsiConsole.Prompt<string>(new SelectionPrompt<string>()
                .AddChoices("Install", "Localize"));
        }

        [ArgActionMethod]
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