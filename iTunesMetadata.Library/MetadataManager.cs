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
        public string Genre { get; set; }
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
        /// <param name="timeout">Timeout in milliseconds</param>
        public static void SetTVMetaData(string path, TVMetadata meta, int timeout = 30000)
        {
            //  First, make sure the file exists
            if(File.Exists(path))
            {
                //  Add meta information using Atomicparsley
                ProcessStartInfo apPInfo = new ProcessStartInfo();
                apPInfo.FileName = Path.Combine(currentPath, "AtomicParsley.exe");

                //  Start building our command line:
                StringBuilder args = new StringBuilder();

                //  Add the file path to the video:
                args.AppendFormat("\"{0}\" --genre \"TV Shows\" --stik \"TV Show\" --overWrite", path);

                //  If we have the show name, include it
                if(!string.IsNullOrWhiteSpace(meta.ShowName))
                {
                    args.AppendFormat(" --TVShowName \"{0}\" --artist \"{0}\"", meta.ShowName);
                }

                //  If we have the episode information, include it
                if(meta.EpisodeNumber > 0)
                {
                    args.AppendFormat(" --TVEpisode \"{0}{1}\" --TVEpisodeNum {1}", meta.ShowSeason, meta.EpisodeNumber);
                }

                //  If we have the season number, include it
                if(meta.ShowSeason > 0)
                {
                    args.AppendFormat(" --TVSeasonNum {0}", meta.ShowSeason);
                }

                //  If we have the title, include it
                if(!string.IsNullOrWhiteSpace(meta.EpisodeTitle))
                {
                    args.AppendFormat(" --title \"{0}\"", meta.EpisodeTitle);
                }

                //  If we have the description, include it
                if(!string.IsNullOrWhiteSpace(meta.EpisodeDescription))
                {
                    args.AppendFormat(" --description \"{0}\"", GetEpisodeSummary(meta.EpisodeDescription));
                }

                //  If we have the artwork, include it
                if(!string.IsNullOrWhiteSpace(meta.ShowArtworkPath))
                {
                    args.AppendFormat(" --artwork \"{0}\"", meta.ShowArtworkPath);
                }

                //  If we have the content rating, include it
                if(!string.IsNullOrWhiteSpace(meta.ShowRating))
                {
                    args.AppendFormat(" --contentRating \"{0}\"", meta.ShowRating);
                }

                //  Spit out our finished args and start the process
                apPInfo.Arguments = args.ToString();
                Process apProcess = Process.Start(apPInfo);
                apProcess.WaitForExit(timeout);

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
        /// <param name="timeout">Timeout in milliseconds</param>
        public static void SetMovieMetaData(string path, MovieMetadata meta, int timeout = 30000)
        {
            //  First, make sure the file exists
            if(File.Exists(path))
            {
                //  Add meta information using Atomicparsley
                ProcessStartInfo apPInfo = new ProcessStartInfo();
                apPInfo.FileName = Path.Combine(currentPath, "AtomicParsley.exe");

                //  Start building our command line:
                StringBuilder args = new StringBuilder();

                //  Add the file path to the video:
                args.AppendFormat("\"{0}\" --stik \"Short Film\" --overWrite", path);

                //  If we have the title, include it
                if(!string.IsNullOrWhiteSpace(meta.Name))
                {
                    args.AppendFormat(" --title \"{0}\"", meta.Name);
                }

                //  If we have the year, include it
                if(meta.Year > 0)
                {
                    args.AppendFormat(" --year \"{0}\"", meta.Year);
                }

                //  If we have the genre, include it
                if(!string.IsNullOrWhiteSpace(meta.Genre))
                {
                    args.AppendFormat(" --genre \"{0}\"", meta.Genre);
                }

                //  If we have the description, include it
                if(!string.IsNullOrWhiteSpace(meta.Description))
                {
                    args.AppendFormat(" --description \"{0}\"", meta.Description);
                }

                //  If we have the artwork, include it
                if(!string.IsNullOrWhiteSpace(meta.ArtworkPath))
                {
                    args.AppendFormat(" --artwork \"{0}\"", meta.ArtworkPath);
                }

                //  If we have the content rating, include it
                if(!string.IsNullOrWhiteSpace(meta.Rating))
                {
                    args.AppendFormat(" --contentRating \"{0}\"", meta.Rating);
                }

                //  Spit out our finished args and start the process
                apPInfo.Arguments = args.ToString();
                Process apProcess = Process.Start(apPInfo);
                apProcess.WaitForExit(timeout);

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
