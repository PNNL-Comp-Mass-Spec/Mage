using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MageUnitTests {

    class TestFileContentProcessorModule : FileContentProcessor {

        public string ExpectedSourceFile { get; set; }
        public string ExpectedSourcePath { get; set; }
        public string ExpectedDestPath { get; set; }


        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context) {
            Assert.AreEqual(ExpectedSourceFile, sourceFile);
            Assert.AreEqual(ExpectedSourcePath, sourcePath);
            Assert.AreEqual(ExpectedDestPath, destPath);
        }

    }
}
