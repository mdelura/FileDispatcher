using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;

namespace FileDispatcher.Core
{
    /// <summary>
    /// Specifies type of name match.
    /// </summary>
    public enum Match
    {
        [Description("Never")]
        /// <summary>
        /// Never match to similar name.
        /// </summary>
        Never,
        [Description("Only on close match")]
        /// <summary>
        /// Only if match is very high.
        /// </summary>
        HighMatchOnly,
        [Description("Always")]
        /// <summary>
        /// Always match the name to the closest match.
        /// </summary>
        Always
    }

    /// <summary>
    /// Provides methods to find target path for the Dispatch Task
    /// </summary>
    [Serializable]
    public class TargetRouter
    {
        const double highSimilarityMaxDistanceToTruncatedShare = 0.25;

        /// <summary>
        /// Gets or sets base target directory.
        /// </summary>
        public string BaseTargetPath { get; set; }
        /// <summary>
        /// Gets or sets whether the router should try to find matching subdirectory.
        /// </summary>
        public Match MatchSubdirectory { get; set; }
        /// <summary>
        /// Gets or sets whether the router should try to rename to a similar file name.
        /// </summary>
        public Match MatchToSimilarFile { get; set; }

        /// <summary>
        /// Provides full target path according to current settings.
        /// </summary>
        /// <param name="sourcePath">Source path of the item to be routed.</param>
        /// <returns>Full target path.</returns>
        public string GetTargetPath(string sourcePath)
        {
            string targetDirectory = GetTargetDirectory(sourcePath);
            return Path.Combine(targetDirectory, GetTargetPath(targetDirectory, sourcePath));
        }

        private static string GetTarget(string sourcePath, Match match, FileSystemInfo[] items, 
            Func<FileSystemInfo, string> getTargetPath, Func<FileSystemInfo, string> getStandardizedName)
        {
            //Standardize item name
            string standardizedItemName = Path.GetFileNameWithoutExtension(sourcePath).Standardize();

            //Get items to match info: potential target path, standardized named to compute distance and the distance
            var itemsToMatch = items
                .Select(i => new
                {
                    TargetPath = getTargetPath(i),
                    StandardizedName = getStandardizedName(i),
                    Distance = getStandardizedName(i).ComputeLevenshteinDistanceTo(standardizedItemName),
                })
                .ToArray();

            //If there is an exact match without truncating the names return this match
            var matched = itemsToMatch
                .FirstOrDefault(s => s.Distance == 0)
                ?.TargetPath;
            if (!string.IsNullOrEmpty(matched))
                return matched;

            //Otherwise get the acceptable  truncated distance share and return the appropriate value
            double acceptableTruncatedDistanceShare = match == Match.HighMatchOnly ?
                highSimilarityMaxDistanceToTruncatedShare : 1;


            return itemsToMatch
                .Select(i => new
                {
                    i.TargetPath,
                    TruncatedDistanceShare = Convert.ToDouble(i.StandardizedName.Truncate(standardizedItemName.Length)
                        .ComputeLevenshteinDistanceTo(standardizedItemName.Truncate(i.StandardizedName.Length))) /
                        Convert.ToDouble(Math.Min(i.StandardizedName.Length, standardizedItemName.Length))
                })
                .OrderBy(i => i.TruncatedDistanceShare)
                .FirstOrDefault(i => i.TruncatedDistanceShare <= acceptableTruncatedDistanceShare)
                ?.TargetPath;
        }

        private string GetTargetDirectory(string sourcePath)
        {
            if (MatchSubdirectory == Match.Never)
                return BaseTargetPath;

            string matchedTargetPath = GetTarget(sourcePath, MatchSubdirectory,
                new DirectoryInfo(BaseTargetPath).GetDirectories(),
                (d) => d.FullName,
                (d) => d.Name.Standardize());

            return matchedTargetPath ?? BaseTargetPath;
        }

        private string GetTargetPath(string targetDirectory, string sourcePath)
        {
            string baseTargetPath = Path.Combine(targetDirectory, Path.GetFileName(sourcePath));
            if (MatchToSimilarFile == Match.Never)
                return baseTargetPath;

            string targetExtension = Path.GetExtension(sourcePath);

            string matchedTargetPath = GetTarget(sourcePath, MatchToSimilarFile, new DirectoryInfo(targetDirectory).GetFiles(),
                (f) => $"{Path.GetFileNameWithoutExtension(f.Name)}{targetExtension}",
                (f) => Path.GetFileNameWithoutExtension(f.Name).Standardize());
            return matchedTargetPath ?? baseTargetPath;
        }
    }
}
