using System.IO;
using CPack;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
        public void TestDeserialization()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("SFML\\CPack.json"))!;

            Assert.IsTrue(package.IncludePath == "C:\\Users\\Wesma\\source\\repos\\CPack\\Tests\\bin\\Debug\\net6.0\\SFML\\include");
        }

        [TestMethod]
        public void TestDeserialization2()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("SFML\\CPack.json"))!;

            Assert.IsTrue(package.LibPath == "C:\\Users\\Wesma\\source\\repos\\CPack\\Tests\\bin\\Debug\\net6.0\\SFML\\lib");
        }

        [TestMethod]
        public void TestDeserialization3()
        {
            Package package = JsonConvert.DeserializeObject<Package>(File.ReadAllText("SFML\\CPack.json"))!;

            Assert.IsTrue(package.BinPath == "C:\\Users\\Wesma\\source\\repos\\CPack\\Tests\\bin\\Debug\\net6.0\\SFML\\bin");
        }
    }
}