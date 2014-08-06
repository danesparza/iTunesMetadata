using System;
using iTunesTVMetadata.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTunesMetadata.Library.Tests
{
    [TestClass]
    public class MetadataManagerTests
    {
        [TestMethod]
        public void SetTVMetaData_ValidMetaData_Success()
        {
            //  Arrange
            string filepath = @"C:\Temp\MFD4-7.m4v";
            TVMetadata meta = new TVMetadata()
            {
                ShowName = "Modern Family",
                ShowSeason = 1,
                EpisodeNumber = 24,
                EpisodeTitle = "Family Portrait",
                EpisodeDescription = "Everything seems to be working against Claire's plans for a new family portrait; Cameron gets a gig as a wedding singer, leaving Mitchell home alone with Lily and a wayward pigeon.",
                ShowRating = "TV-PG",
                ShowArtworkPath = @"C:\Temp\artwork\ModernFamilyS01.jpg"
            };
            
            //  Act
            MetadataManager.SetTVMetaData(filepath, meta);

            //  Assert


        }
    }
}
