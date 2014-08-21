using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;

namespace iTunesTVMetadata.Library
{
    /// <summary>
    /// TV based meta information
    /// </summary>
    public class TVMetadata
    {
        /// <summary>
        /// The name of the TV show
        /// </summary>
        public string ShowName { get; set; }
        
        /// <summary>
        /// The season number of the TV show
        /// </summary>
        public int ShowSeason { get; set; }

        /// <summary>
        /// The TV show rating.  Should be blank, or one of: 'TV-Y', 'TV-Y7', 'TV-G', 'TV-PG', 'TV-14', 'TV-MA' 
        /// </summary>
        public string ShowRating { get; set; }

        /// <summary>
        /// Full local path to artwork to set in the file
        /// </summary>
        public string ShowArtworkPath { get; set; }

        /// <summary>
        /// The episode name / title
        /// </summary>
        public string EpisodeTitle { get; set; }

        /// <summary>
        /// The number of the episode within the TV season
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// The episode description / summary
        /// </summary>
        public string EpisodeDescription { get; set; }        
    }

    /// <summary>
    /// Movie meta information
    /// </summary>
    public class MovieMetadata
    {
        /// <summary>
        /// The name of the movie
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An description/summary of the movie
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The genre for the movie.  Should be blank, or one of: Action, Classics, Comedy, 
        /// Documentary, DramaHorror, Independent, Kids, Music, 
        /// Romance, SciFi, ShortFilms, Sports, Thriller, Western
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// The 4 digit year in which the movie was released
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The movie rating.  Should be blank, or one of: 'G', 'PG', 'PG-13', 'R', 'NC-17'
        /// </summary>
        public string Rating { get; set; }

        /// <summary>
        /// Full local path to artwork to set in the file
        /// </summary>
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
        /// Gets the video runtime length in seconds.  Returns 0 if there was a problem getting the length 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double GetVideoLengthInSeconds(string path)
        {
            double nanoseconds = 0;
            double retval = 0;

            try
            {
                //  Get the information from the OS:
                ShellFile so = ShellFile.FromFilePath(path);
                if(double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds))
                {
                    // One million nanoseconds in 1 millisecond, 
                    // but we are passing in 100ns units...
                    retval = (nanoseconds * 0.0001) / 1000;
                }
            }
            catch(Exception)
            {
                /* Eat the exception for now */
            }

            return retval;
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

                //  Spit out our finished args 
                apPInfo.Arguments = args.ToString();

                //  Set our process options:
                apPInfo.CreateNoWindow = true;
                apPInfo.UseShellExecute = false;
                apPInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //  Start the process
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

                //  Spit out our finished args 
                apPInfo.Arguments = args.ToString();

                //  Set our process options:
                apPInfo.CreateNoWindow = false;
                apPInfo.UseShellExecute = false;
                apPInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //  Start the process
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
