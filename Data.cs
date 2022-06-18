using SuRGeoNix.BitSwarmLib;
using System;
using System.Collections.Generic;

namespace SignalX
{
    [Serializable]
    public class Banners
    {
        public string B1 { get; set; }
        public string B2 { get; set; }
    }

    [Serializable]
    public class Images
    {
        public string Icon { get; set; }
        public Banners Banners { get; set; }
    }

    [Serializable]
    public class GameData
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Infohash { get; set; }
        public string LVersion { get; set; }
        public string MD5 { get; set; }
        public string Filename { get; set; }
        public Images Images { get; set; }
    }

    public class data
    {
        public static Options opt = new Options();
        public static BitSwarm bitSwarm = new BitSwarm(opt);

        public static bool paused = false;

        public static string gname = "";
        public static string format = "";
        public static string hash = "";
    }
}