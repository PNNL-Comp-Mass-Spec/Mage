using System.Collections.Generic;
using Mage;
using NUnit.Framework;

namespace MageUnitTests
{
    internal class TestFileContentProcessorModule : FileContentProcessor
    {
        // Ignore Spelling: Dest

        public string ExpectedSourceFile { get; set; }
        public string ExpectedSourcePath { get; set; }
        public string ExpectedDestPath { get; set; }

        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(sourceFile, Is.EqualTo(ExpectedSourceFile));
                Assert.That(sourcePath, Is.EqualTo(ExpectedSourcePath));
                Assert.That(destPath, Is.EqualTo(ExpectedDestPath));
            }
        }
    }
}
