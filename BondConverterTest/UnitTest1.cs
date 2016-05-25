using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

namespace BondConverterTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleTypes()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(Foo)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }

        [TestMethod]
        public void ContainsComplexTypes()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(Bar)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }

        [TestMethod]
        public void InheritanceTypes()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(FooBar)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }

        [TestMethod]
        public void TestEnums()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(FooNum)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }

        [TestMethod]
        public void CollectionTypes()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(FooL)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }


        [TestMethod]
        public void TestMaps()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(Doo)
            });

            var str = converter.GenerateBondFile("test");

            CompileBond(str);
        }

        [TestMethod]
        public void TestDateTimes()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(Noo)
            });

            var str = converter.GenerateBondFile("test");

            //We expect this to fail. Time is a funky thing and we want schemas to be careful and mindful here
            CompileBond(str);
        }

        [TestMethod]
        public void TestGenerics()
        {
            var converter = new BondConverter.BondConverter(new Type[] { typeof(PGen)
            });

            var str = converter.GenerateBondFile("test");

            //We don't compile this because we expect it to file compilation. Currently a gap in bond gen
            CompileBond(str);
        }

        private void CompileBond(string bond)
        {
            var tempBondFilePath = "test.bond";
            if (File.Exists(tempBondFilePath))
                File.Delete(tempBondFilePath);

            File.WriteAllText(tempBondFilePath, bond);

            //Invoke gbc
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = "gbc.exe";
            process.StartInfo.Arguments = string.Format("c# {0}", tempBondFilePath);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.

            if (!File.Exists("test_types.cs"))
                throw new Exception("gbc failure");

            File.Delete("test_types.cs");
        }
    }
}
