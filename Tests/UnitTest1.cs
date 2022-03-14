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
    }
}