using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FileDispatcher.Core.ExtensionMethods.StringExtensions
{
    /// <summary>
    /// Provides extension methods for <see cref="string"/> type
    /// </summary>
    static class StringExtensions
    {
        /// <summary>
        /// Check if <see cref="String"/> matches <paramref name="filter"/>
        /// </summary>
        /// <param name="that">File name to be checked</param>
        /// <param name="filter">Filter to be matched</param>
        /// <returns><see langword="true"/> if matches <paramref name="filter"/>, otherwise <see langword="false"/></returns>
        public static bool FitsFilter(this string that, string filter)
        {
            string regexPattern = filter
                .Replace(".", "[.]")
                .Replace("*", ".*")
                .Replace("?", ".");

            Regex mask = new Regex($"^{regexPattern}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            return mask.IsMatch(that);
        }

        /// <summary>
        /// Determine whether the specified path is a directory or a file (if exists).
        /// </summary>
        /// <param name="that"></param>
        /// <returns><see langword="true"/> if the <see cref="string"/> points to a directory, <see langword="false"/> if file
        /// or throws <see cref="IOException"/> if path does not exist.</returns>
        public static bool IsDirectory(this string that) => File.GetAttributes(that).HasFlag(FileAttributes.Directory);

        /// <summary>
        /// Compute the Levenshtein distance between strings.
        /// </summary>
        /// <param name="that">A <see cref="string"/> to calculate distance for.</param>
        /// <param name="other">Other <see cref="string"/> to calculate the distance.</param>
        /// <returns>Levenstein distance</returns>
        public static int ComputeLevenshteinDistanceTo(this string that, string other)
        {
            int[,] d = new int[that.Length + 1, other.Length + 1];

            // Step 1
            if (that.Length == 0) return other.Length;

            if (other.Length == 0) return that.Length;

            // Step 2
            for (int i = 0; i <= that.Length; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= other.Length; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= that.Length; i++)
            {
                //Step 4
                for (int j = 1; j <= other.Length; j++)
                {
                    // Step 5
                    int cost = (other[j - 1] == that[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[that.Length, other.Length];
        }

        /// <summary>
        /// Standardize the string: replace all non-word characters with a single space and make it upper-case (Invariant Culture)
        /// </summary>
        /// <param name="that"><see cref="string"/> to be standardized</param>
        /// <returns>Standardized <see cref="string"/></returns>
        public static string Standardize(this string that) => Regex.Replace(that, @"\W+", " ").Trim().ToUpperInvariant();


        /// <summary>
        /// Truncate the <see cref="string"/> it the length exceeds <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="maxLength">Maximum result length</param>
        /// <returns>Truncated <see cref="string"/> or original if it's not longer than <paramref name="maxLength"/></returns>
        public static string Truncate(this string that, int maxLength)
        {
            return (that?.Length ?? 0) <= maxLength ? that : that.Substring(0, maxLength);
        }
    }
}
