using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public string ShowRating { get; set; }
        public string ShowArtworkPath { get; set; }

        public string EpisodeTitle { get; set; }
        public int EpisodeNumber { get; set; }
        public string EpisodeDescription { get; set; }        
    }

    /// <summary>
    /// Movie meta information
    /// </summary>
    public class MovieMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Rating { get; set; }
        public string ArtworkPath { get; set; }
    }

    /// <summary>
    /// Helper to set iTunes meta information on a given video file.  Assumes AtomicParsley.exe is 
    /// in the same directory as the calling assembly
    /// </summary>
    public static class MetadataManager
    {
        private static string currentPath = string.Empty;

        /// <summary>
        /// Static constructor
        /// </summary>
        static MetadataManager()
        {
            currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Set TV based metadata in the passed file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meta"></param>
        /// <param name="timout">Timeout in milliseconds</param>
        public static void SetTVMetaData(string path, TVMetadata meta, int timout = 30000)
        {
            //  First, make sure the file exists
            if(File.Exists(path))
            {
                //  Add meta information using Atomicparsley
                ProcessStartInfo apPInfo = new ProcessStartInfo();
                apPInfo.FileName = Path.Combine(currentPath, "AtomicParsley.exe");

                /*
                apPInfo.Arguments = string.Format(
                    "\"{0}\" --genre \"TV Shows\" --stik \"TV Show\" --TVShowName \"{1}\" --TVEpisode \"{2}{3}\" --TVSeasonNum {2} --TVEpisodeNum {3} --artist \"{1}\" --title \"{4}\" --description \"{7}\" --contentRating \"{5}\" --artwork \"{6}\" --overWrite",
                    handbrakeOutput,
                    episodeInfo.ShowName,
                    episodeInfo.SeasonNumber,
                    episodeInfo.EpisodeNumber,
                    episodeInfo.EpisodeTitle,
                    metaShowInfo.Rating,
                    metaShowInfo.ArtworkLocation,
                    GetEpisodeSummary(episodeInfo.EpisodeSummary)
                    );
                */

                Process apProcess = Process.Start(apPInfo);
                apProcess.WaitForExit(timout);

                //  If it hasn't exited, but it's not responding...
                //  kill the process
                if(!apProcess.HasExited && !apProcess.Responding)
                {
                    apProcess.Kill();
                }
            }
        }

        /// <summary>
        /// Set movie based metadata in the passed file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meta"></param>
        /// <param name="timout">Timeout in milliseconds</param>
        public static void SetMovieMetaData(string path, MovieMetadata meta, int timout = 30000)
        {
            //  First, make sure the file exists
            if(File.Exists(path))
            {
                //  Add meta information using Atomicparsley
                ProcessStartInfo apPInfo = new ProcessStartInfo();
                apPInfo.FileName = Path.Combine(currentPath, "AtomicParsley.exe");

                /*
                apPInfo.Arguments = string.Format(
                    "\"{0}\" --title \"{1}\" --year \"{2}\" --genre \"{3}\" --stik \"Short Film\" --description \"Library\" --artwork \"{4}\" --contentRating \"{5}\" --overWrite",
                    handbrakeOutput,
                    movieInfo.Name,
                    movieInfo.Year,
                    movieInfo.Genre,
                    movieInfo.ArtworkLocation,
                    movieInfo.Rating
                    );
                */

                Process apProcess = Process.Start(apPInfo);
                apProcess.WaitForExit(timout);

                //  If it hasn't exited, but it's not responding...
                //  kill the process
                if(!apProcess.HasExited && !apProcess.Responding)
                {
                    apProcess.Kill();
                }
            }
        }

        /// <summary>
        /// Preps the episode summary to be used by AtomicParsley
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        private static string GetEpisodeSummary(string summary)
        {
            string retval = string.Empty;

            //  If we actually have a summary
            if(!string.IsNullOrWhiteSpace(summary))
            {
                //  If it's too long ... 
                if(summary.Length > 250)
                {
                    //  Shorten it and trim any leading / trailing whitespace
                    retval = summary.Substring(0, 250).Trim() + "...";
                }
                else
                    retval = summary;
            }

            return retval;
        }
    }
}
