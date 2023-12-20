using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tiktokTH
{
    public class VideoDateParser
    {
        public DateTime TranslateVideoDate(string videoDate)
        {
            if (videoDate.Contains("atrás"))
            {
                // Parse relative date formats like "1w atrás", "3d atrás"
                return ParseRelativeDate(videoDate);
            }
            else if (DateTime.TryParseExact(videoDate, "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime specificDate))
            {
                // Parse specific date format like "2021-7-16"
                return specificDate;
            }
            else
            {
                // Parse specific date formats like "12-7", "6-2"
                return ParseSpecificDate(videoDate);
            }
        }

        private DateTime ParseRelativeDate(string relativeDate)
        {
            var match = Regex.Match(relativeDate, @"(\d+)\s*(w|d)\s*atrás");

            if (match.Success)
            {
                int value = int.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value;

                // Calculate the relative date
                if (unit == "w")
                {
                    return DateTime.Now.AddDays(-7 * value);
                }
                else // unit == "d"
                {
                    return DateTime.Now.AddDays(-value);
                }
            }

            // Default to the current date if parsing fails
            return DateTime.Now;
        }

        private DateTime ParseSpecificDate(string specificDate)
        {
            // Parse specific date formats like "12-7", "6-2"
            if (DateTime.TryParseExact(specificDate, "M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                // If parsing succeeds, return the parsed date
                return result;
            }

            // Default to the current date if parsing fails
            return DateTime.Now;
        }
    }
}
