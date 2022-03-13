using System.Runtime.CompilerServices;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using Spectre.Console;

namespace CPack
{
    public class Package
    {
        public string Name { get; set; }
        public string? Version { get; set; }
        public string IncludePath { get; set; }
        public string Description { get; set; }
        public string LibPath { get; set; }

        public List<string> DocFiles { get; set; }
        public List<string> ExampleFiles { get; set; }
        public List<string> LibraryFiles { get; set; }
        public List<string> DllFiles { get; set; }

        public Package()
        {
            LibraryFiles = new List<string>();
            DllFiles = new List<string>();
            DocFiles = new List<string>();
            ExampleFiles = new List<string>();
        }

        public void GetExampleFiles(string exampleDir)
        {
            string[] files = Directory.GetFiles(exampleDir);

            for (int i = 0; i < files.Length; i++)
            {
                ExampleFiles.Add(files[i]);
            }
        }

        public void GetLibFiles(string libPath)
        {
            LibPath = libPath;
            string[] files = Directory.GetFiles(libPath);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".lib"))
                {
                    LibraryFiles.Add(new FileInfo(files[i]).Name);
                }
            }
        }

        public void GetDllFiles(string binPath)
        {
            string[] dlls = Directory.GetFiles(binPath);

            for (int i = 0; i < dlls.Length; i++)
            {
                if (dlls[i].EndsWith(".dll"))
                {
                    FileInfo dll = new FileInfo(dlls[i]);
                    DllFiles.Add(dll.Name);
                }
            }
        }

        public void Pack()
        {
            var archive = ZipArchive.Create();
            archive.AddAllFromDirectory(Directory.GetCurrentDirectory());
            archive.SaveTo(Name + ".cpack", CompressionType.Deflate);
        }

        public void Info()
        {
            AnsiConsole.MarkupLine($"[bold]{Name}[/]");
            Console.WriteLine(Description);
            Console.WriteLine(Version);
            Console.WriteLine(Version);
        }

        public void Install(string projName)
        {
            CopyDlls(projName);
            MakeIncludesAndDependancies(projName + "\\" + projName + ".vcxproj");
        }

        public void CopyDlls(string destination)
        {
            AnsiConsole.Progress()
                .Columns(
                    new ElapsedTimeColumn(),
                    new ProgressBarColumn(),
                    new SpinnerColumn(Spinner.Known.Ascii)
                    )
                
                .Start(ctx =>
            {
                ProgressTask task = ctx.AddTask("Copying DllFiles");

                for (int i = 0; i < DllFiles.Count; i++)
                {
                    FileInfo info = new FileInfo(DllFiles[i]);

                    File.Copy(info.FullName, destination + "\\" + info.Name);
                    task.Increment(DllFiles.Count / 100);
                }
            });
        }

        public void MakeIncludesAndDependancies(string projName)
        {
            string dependencies = String.Empty;
            string[] strings = File.ReadAllLines(projName);
            List<string> lines = new List<string>();

            for (int i = 0; i < LibraryFiles.Count; i++)
            {
                dependencies += LibraryFiles[i] + ";";
            }

            for (int i = 0; i < strings.Length; i++)
            {
                lines.Add(strings[i]);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.Contains("<ClCompile>"))
                {
                    lines.Insert(i + 1, $"\t\t\t<AdditionalIncludeDirectories>{IncludePath}</AdditionalIncludeDirectories>");
                }

                if (line.Contains("<link>"))
                {
                    lines.Insert(i + 1, $"\t\t\t<AdditionalLibraryDirectories>{LibPath}</AdditionalLibraryDirectories>");
                    lines.Insert(i + 2, $"\t\t\t<AdditionalDependencies>{dependencies}</AdditionalDependencies>");
                }
            }

            File.WriteAllLines(projName, lines);
        }
    }
}