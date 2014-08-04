using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesTVMetadata.Library
{
    /// <summary>
    /// TV based meta information
    /// </summary>
    public class TVMetadata
    {
        public string ShowName { get; set; }
        public int ShowSeason { get; set; }
        public string ShowArtworkPath { get; set; }
        public string ShowRating { get; set; }

        public string EpisodeTitle { get; set; }
        public int EpisodeNumber { get; set; }
        public string EpisodeDescription { get; set; }        
    }

    /// <summary>
    /// Sets TV based meta information on a given file
    /// </summary>
    public class TVMetadataManager
    {
        //  Default constructor
        public TVMetadataManager()
        {

        }
    }
}
