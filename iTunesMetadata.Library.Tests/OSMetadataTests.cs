using System;
using iTunesTVMetadata.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAPICodePack.Shell;

namespace iTunesMetadata.Library.Tests
{
    [TestClass]
    public class OSMetadataTests
    {
        [TestMethod]
        public void GetVideoLengthInSeconds_ValidPath_Success()
        {
            //  Arrange
            string videoFilePath = @"C:\Temp\JohnOliverTest.mp4";
            double seconds = 0;

            //  Act
            seconds = MetadataManager.GetVideoLengthInSeconds(videoFilePath);
            
            //  Assert
            Assert.AreNotEqual<double>(0, seconds);
        }
    }
}
