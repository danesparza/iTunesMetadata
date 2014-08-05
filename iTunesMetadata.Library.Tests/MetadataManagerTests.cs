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
            string filepath = "";
            TVMetadata meta = new TVMetadata()
            {
                ShowName = "Modern Family",
                ShowSeason = 1,
                EpisodeNumber = 4,
                EpisodeTitle = "Some title",
                EpisodeDescription = "",
                ShowRating = "",
                ShowArtworkPath = ""
            };
            
            //  Act
            MetadataManager.SetTVMetaData(filepath, meta);

            //  Assert


        }
    }
}
