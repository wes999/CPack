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
            MakeIncludesAndDependancies(projName);
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
            AnsiConsole.Progress()
                .Columns(
                    new ElapsedTimeColumn(),
                    new ProgressBarColumn(),
                    new SpinnerColumn(Spinner.Known.Ascii)
                )

                .Start(ctx =>
                {
                    ProgressTask task = ctx.AddTask("Making Includes And Dependencies");

                    string dependancies = String.Empty;

                    for (int i = 0; i < LibraryFiles.Count; i++)
                    {
                        dependancies += LibraryFiles[i] + ";";
                        task.Increment(100 / LibraryFiles.Count);
                    }

                    string[] lines = File.ReadAllLines(projName + "\\" + projName + ".vcxproj");

                    string settings =
                        $"   <ItemDefinitionGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|Win32'\">\r\n    <ClCompile>\r\n      <WarningLevel>Level3</WarningLevel>\r\n      <SDLCheck>true</SDLCheck>\r\n      <PreprocessorDefinitions>WIN32;_DEBUG;_CONSOLE;%(PreprocessorDefinitions)</PreprocessorDefinitions>\r\n      <ConformanceMode>true</ConformanceMode>\r\n      <AdditionalIncludeDirectories>{IncludePath}</AdditionalIncludeDirectories>\r\n    </ClCompile>\r\n    <Link>\r\n      <SubSystem>Console</SubSystem>\r\n      <GenerateDebugInformation>true</GenerateDebugInformation>\r\n      <AdditionalLibraryDirectories>{LibPath}</AdditionalLibraryDirectories>\r\n      <AdditionalDependencies>{dependancies}%(AdditionalDependencies)</AdditionalDependencies>\r\n    </Link>\r\n  </ItemDefinitionGroup>";

                    string done = String.Empty;

                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        done += lines[i] + "\n";
                    }

                    done += settings + "\n";
                    done += "</Project>";

                    File.Delete(projName + "\\" + projName + ".vcxproj");

                    FileStream stream = new FileStream(projName + "\\" + projName + ".vcxproj", FileMode.Create);
                    StreamWriter writer = new StreamWriter(stream);

                    writer.Write(done);
                    writer.Close();
                }); 
        }
    }
}