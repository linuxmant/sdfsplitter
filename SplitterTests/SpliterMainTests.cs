using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using SDFSplitter.ViewModel;
using System.Text.RegularExpressions;

namespace SplitterTests {
    [TestClass]
    public class SpliterMainTests {
        MainViewModel mv;
        string baseDir;
        string outFolder;
        int suffix = 0;

        [TestInitialize]
        public void setUp() {
            mv = new MainViewModel();
            baseDir = AppDomain.CurrentDomain.BaseDirectory + @"\..\..";
            Console.WriteLine("init test... (" + baseDir + ")");
        }

        [TestCleanup]
        public void cleanUp() { }

        [TestMethod]
        public void testFileWithCompleteFile() {
            Console.WriteLine("Calling complete set... (" + Path.Combine(baseDir, @"\Resources\complete.sdf") + ")");
            mv.Suffix = 0;
            mv.InFile = baseDir + @"\Resources\complete.sdf";
            mv.OutDir = baseDir + @"\Resources\complete";

            mv.SplitCommand.Execute("Test class");
            Debug.WriteLine(mv.Results);

            var files = Directory.GetFiles(mv.OutDir);

            Assert.AreEqual(5, Directory.GetFiles(mv.OutDir).Length);
            Assert.IsTrue(files[0].Contains(string.Format("{0:d7}", suffix)));
            Assert.IsTrue(files[files.Length - 1].Contains(string.Format("{0:d7}", suffix + files.Length - 1)));
        }

        [TestMethod]
        public void testFileWithMissingBlocks() {
            Console.WriteLine("Calling missing set...");
            mv.Suffix = 0;
            mv.InFile = baseDir + @"\Resources\missing.sdf";
            mv.OutDir = baseDir + @"\Resources\missing";

            mv.SplitCommand.Execute("Test method missing");
            Debug.WriteLine(mv.Results);

            var files = Directory.GetFiles(mv.OutDir);

            Assert.AreEqual(3, Directory.GetFiles(mv.OutDir).Length);
            Assert.IsTrue(files[0].Contains(string.Format("{0:d7}", suffix)));
            Assert.IsTrue(files[files.Length - 1].Contains(string.Format("{0:d7}", suffix + files.Length - 1)));
        }

        [TestMethod]
        public void testFileWithAlternateSuffix() {
            Console.WriteLine("Calling alt suffix set...");
            mv.Suffix = 3;
            mv.InFile = baseDir + @"\Resources\complete.sdf";
            mv.OutDir = baseDir + @"\Resources\suffix";

            mv.SplitCommand.Execute("Test method suffix");
            Debug.WriteLine(mv.Results + "\nsuffix:" + suffix);

            var files = Directory.GetFiles(mv.OutDir);
            Assert.AreEqual(5, files.Length);

            int i = mv.Suffix;
            foreach (var file in files) {
                var pattern = ".*?\\d{6}" + i++ + ".*";
                StringAssert.Matches(file, new Regex(pattern));
            }
        }
    }
}
