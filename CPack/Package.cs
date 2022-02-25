namespace CPack
{
    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string IncludePath { get; set; }
        public string Description { get; set; }
        public string LibPath { get; set; }

        public List<string> LibraryFiles { get; set; }
        public List<string> Dlls { get; set; }

        public Package()
        {
            LibraryFiles = new List<string>();
            Dlls = new List<string>();
        }

        public void CopyDlls(string destination)
        {
            for (int i = 0; i < Dlls.Count; i++)
            {
                FileInfo info = new FileInfo(Dlls[i]);

                File.Copy(info.FullName, Name + "\\" + info.Name);
            }
        }

        public void MakeIncludesAndDependancies(string projName)
        {
            string dependancies = String.Empty;

            for (int i = 0; i < LibraryFiles.Count; i++)
            {
                dependancies += LibraryFiles[i] + ";";
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
        }
    }
}