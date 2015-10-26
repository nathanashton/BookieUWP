using System;
using System.IO;

namespace Bookie
{
    public static class Utils
    {
        public static int CalculatePercentage(int current, int startIndex, int endIndex)
        {
            float range = endIndex - startIndex;
            if (range == 0)
            {
                range = 1;
            }
            var percentage = ((current - startIndex)/range)*100;
            return Convert.ToInt32(percentage);
        }

        public static int CalculatePercentage(long current, long startIndex, long endIndex)
        {
            var range = endIndex - startIndex;
            var percentage = ((current - startIndex)/range)*100;
            return Convert.ToInt32(percentage);
        }

        public static string GenerateRandomString()
        {
            //Guid g = Guid.NewGuid();
            //string GuidString = Convert.ToBase64String(g.ToByteArray());
            //GuidString = GuidString.Replace("=", "");
            //GuidString = GuidString.Replace("+", "");
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            random += ".png";
            return random;
        }

    }
}