///
/// CharacterHistogram
/// main.cs
/// by tkp
///
/// Copyright (C) 2018-2020 Thane K. Plummer.
/// Released under MIT License.
///

using System;
using System.Collections.Generic;
using System.Linq;


namespace CharacterHistogram
{
    class Program
    {

        static void Main(string[] args)
        {
            var s1 = "NOE TIMMY HEATH";
            var s2 = "NOEL TIMOTHY HEATH";

            var mash1 = Histogram(s1);
            var mash2 = Histogram(s2);
            Console.WriteLine($"{s1,-20} {mash1:X16} {mash1}");
            Console.WriteLine($"{s2,-20} {mash2:X16} {mash2}");

            var diff = HistogramDiffChars(mash1, mash2);
            Console.WriteLine($"Max character difference in strings (Levenshtein distance): {diff}");

            var mashList1 = s1.Split(new[] { ' ' }).Select(n => Histogram(n));
            Console.WriteLine($"words in [{s1}]");
            foreach (var word in mashList1)
            {
                Console.WriteLine($"characters histograms: {word:X16}");
            }
        }


        // NOT CURRENTLY USED
        public class WordBin
        {
            public char Key { get; set; }
            public int BinNum { get; set; }
            public int BinSize { get; set; }
            public int BinOffset { get; set; }
        }


        // Put character histogram in to 64-bit ULONG
        private static ulong Histogram(string s)
        {
            ulong retval = 0UL;
            var hist = new List<int>(16) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (var c in s.ToLower())
            {
                var i = (int)c;
                var m = i % 16;
                hist[m] += 1;
            }

            hist.Reverse();

            var idx = 0;
            foreach (var v in hist)
            {
                ulong k = (ulong)v << (4 * idx++);
                retval += k;
                //Console.WriteLine("{0} {1} {2} {3}", idx, v, k, retval);
            }
            return retval;
        }


        private static int HistogramDiffChars(ulong mash1, ulong mash2)
        {
            // Quick check for equality
            if (mash1 == mash2)
            {
                return 0;
            }

            int hist1, hist2;
            int dist1 = 0;
            int dist2 = 0;

            for (int i = 0; i < 64; i += 4)
            {
                ulong mask = (ulong)15 << i;
                hist1 = (int)((mash1 & mask) >> i);
                hist2 = (int)((mash2 & mask) >> i);
                int diff = Math.Abs(hist1 - hist2);
                if (diff > 0)
                {
                    if (hist1 == 0)
                    {
                        dist2 += diff;
                    }
                    else if (hist2 == 0)
                    {
                        dist1 += diff;
                    }
                    else
                    {
                        dist1 += diff;
                        dist2 += diff;
                    }
                }
            }
            return Math.Min(dist1, dist2);
        }
    }
}
