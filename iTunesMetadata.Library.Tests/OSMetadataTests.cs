using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAPICodePack.Shell;

namespace iTunesMetadata.Library.Tests
{
    [TestClass]
    public class OSMetadataTests
    {
        [TestMethod]
        public void GetVideoData_FromOS_Success()
        {
            //  Arrange
            string videoFilePath = @"C:\Temp\ATOH_S09E01.mp4";
            double nanoseconds = 0;
            double seconds = 0;

            //  Act
            ShellFile so = ShellFile.FromFilePath(videoFilePath);
            if(double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds))
            {
                // One million nanoseconds in 1 millisecond, 
                // but we are passing in 100ns units...
                seconds = (nanoseconds * 0.0001) / 1000;
            }

            //  Assert
            Assert.AreNotEqual<double>(0, seconds);
        }
    }
}
