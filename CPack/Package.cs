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
        public string BinPath { get; set; }

        public DateTime LastUpDateTime { get; set; }
        public DateTime CreationTime { get; set; }

        public List<Commit> Commits { get; set; }
        public List<string> DocFiles { get; set; }
        public List<string> ExampleFiles { get; set; }
        public List<string> LibraryFiles { get; set; }
        public List<string> DllFiles { get; set; }

        public Package()
        {
            Commits = new List<Commit>();
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
            BinPath = binPath;
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

        public void Update(string message)
        {
            GetDllFiles(BinPath);
            GetLibFiles(LibPath);

            Commits.Add(new Commit(message, DateTime.Now));
            LastUpDateTime = DateTime.Now;
        }

        public void Pack()
        {
            var archive = ZipArchive.Create();
            archive.AddAllFromDirectory(Directory.GetCurrentDirectory());
            archive.SaveTo(Name + ".cpack", CompressionType.Deflate);
        }

        public void Info()
        {
            Tree tree = new Tree("Commits");

            for (int i = 0; i < Commits.Count; i++)
            {
                Commit commit = Commits[i];

                tree.AddNode($"{commit.Message} at {commit.Time}");
            }

            AnsiConsole.MarkupLine($"[bold]{Name}[/]");
            AnsiConsole.MarkupLine(Description);
            AnsiConsole.MarkupLine($"Creation Time: {CreationTime}");
            AnsiConsole.Write(tree);
        }

        public void Localize()
        {
            string[] dlls = Directory.GetFiles(BinPath);

            DllFiles.Clear();
            IncludePath = new FileInfo(IncludePath).FullName;
            LibPath = new FileInfo(LibPath).FullName;
            BinPath = new FileInfo(BinPath).FullName;

            for (int i = 0; i < dlls.Length; i++)
            {
                DllFiles.Add(new FileInfo(dlls[i]).FullName);
            }
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
                    ProgressTask task = ctx.AddTask("Copying DllFiles", true, DllFiles.Count);

                    for (int i = 0; i < DllFiles.Count; i++)
                    {
                        FileInfo info = new FileInfo(DllFiles[i]);

                        File.Copy(info.FullName, destination + "\\" + info.Name);
                        task.Increment(1);
                    }
                });
        }

        public void MakeIncludesAndDependancies(string projName)
        {
            AnsiConsole.Progress().Columns(
                    new ElapsedTimeColumn(),
                    new ProgressBarColumn(),
                    new SpinnerColumn(Spinner.Known.Ascii))

                .Start(ctx =>
                {
                    ProgressTask task = ctx.AddTask("MakeIncludesAndDependancies", true, 60d);

                    string dependencies = String.Empty;
                    string[] strings = File.ReadAllLines(projName);
                    List<string> lines = new List<string>();
                    task.Increment(10);

                    for (int i = 0; i < LibraryFiles.Count; i++)
                    {
                        dependencies += LibraryFiles[i] + ";";
                    }

                    task.Increment(10);

                    for (int i = 0; i < strings.Length; i++)
                    {
                        lines.Add(strings[i]);
                    }

                    task.Increment(10);

                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];

                        if (line.Contains("<ClCompile>"))
                        {
                            lines.Insert(i + 1,
                                $"\t\t\t<AdditionalIncludeDirectories>{IncludePath}</AdditionalIncludeDirectories>");
                            task.Increment(10);
                        }

                        if (line.Contains("<Link>"))
                        {
                            lines.Insert(i + 1,
                            $"\t\t\t<AdditionalLibraryDirectories>{LibPath}</AdditionalLibraryDirectories>");
                            task.Increment(10);

                            lines.Insert(i + 2,
                                $"\t\t\t<AdditionalDependencies>{dependencies}</AdditionalDependencies>");
                            task.Increment(10);
                        }
                    }

                    File.WriteAllLines(projName, lines);
                    task.Increment(10);
                }); 
        }
    }
}