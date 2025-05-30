﻿using System.Collections.Generic;
using Mage;
using NUnit.Framework;

namespace MageUnitTests
{
    internal class TestFileContentProcessorModule : FileContentProcessor
    {
        public string ExpectedSourceFile { get; set; }
        public string ExpectedSourcePath { get; set; }
        public string ExpectedDestPath { get; set; }

        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
            Assert.AreEqual(ExpectedSourceFile, sourceFile);
            Assert.AreEqual(ExpectedSourcePath, sourcePath);
            Assert.AreEqual(ExpectedDestPath, destPath);
        }
    }
}
