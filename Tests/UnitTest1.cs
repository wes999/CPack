using System.IO;
using CPack;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SharpCompress.Archives.Zip;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSerialization()
        {
            Package package = new Package();

            package.Name = "amongus";
            package.BinPath = "C:\\Bin\\ampngus";

            File.WriteAllText("test.json",
                JsonConvert.SerializeObject(package));

            Package package2 = JsonConvert.DeserializeObject<Package>(File.ReadAllText("test.json"))!;

            Assert.IsTrue(package.Name == package2.Name && package.BinPath == package2.BinPath);
        }

        [TestMethod]
        public void TestPack()
        {
            Package package = new Package();
            package.Name = "folder";

            package.Pack();
        }

        [TestMethod]
        public void TestUnPack()
        {
            var archive = ZipArchive.Open("folder.cpack");
            archive.ExtractAllEntries();
        }

        [TestMethod]
        public void TestDeserialization()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("C:\\sfml-2.5.1\\CPack.json"))!;

            Assert.IsTrue(package.IncludePath ==
                          "C:\\sfml-2.5.1\\include");
        }

        [TestMethod]
        public void TestDeserialization2()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("C:\\sfml-2.5.1\\Cpack.json"))!;

            Assert.IsTrue(package.LibPath ==
                          "C:\\sfml-2.5.1\\lib");
        }

        [TestMethod]
        public void TestDeserialization3()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("C:\\sfml-2.5.1\\Cpack.json"))!;

            Assert.IsTrue(package.BinPath ==
                          "C:\\sfml-2.5.1\\bin");
        }
    }
}