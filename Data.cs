using Newtonsoft.Json;
using SuRGeoNix.BitSwarmLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;

namespace Index
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
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Infohash { get; set; }
        public string LVersion { get; set; }
        public string MD5 { get; set; }
        public string Filename { get; set; }
        public Images Images { get; set; }
    }

    public class Data
    {
        public static Options opt = new();
        public static BitSwarm bitSwarm = new(opt);

        public static List<GameData> games = Methods.GetData();

        public static bool[] paused = { false, false, false };
        public static bool[] dls = { false, false, false };
    }

    public class Methods
    {
        public static List<GameData> GetData()
        {
            try
            {
                WebClient wc = new WebClient();

                string json = wc.DownloadString(new Uri("https://raw.githubusercontent.com/OmarHopman/index/info/database/database.json"));

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var list = JsonConvert.DeserializeObject<List<GameData>>(json);

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        return JsonConvert.DeserializeObject<List<GameData>>(json);
                    }
                    else
                    {
                        CheckConnection();
                        return null;
                    }
                }
                else
                {
                    CheckConnection();
                    return null;
                }
            }
            catch
            {
                CheckConnection();
                return null;
            }
        }

        public static List<GameData> CheckConnection()
        {
            var result = new Ping().Send("https://github.com/");

            if (result.Status != IPStatus.Success)
            {
                MessageBox.Show("Unreachable database error", "Index", MessageBoxButton.OK);
                Application.Current.Shutdown();
                return null;
            }
            else
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show("Unknown error, please report at Discord", "Index", MessageBoxButton.OK);
                    return null;
                }
                else
                {
                    MessageBox.Show("Failed to connect to network", "Index", MessageBoxButton.OK);
                    return null;
                }
            }
        }
    }
}