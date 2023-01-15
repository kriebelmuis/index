using Newtonsoft.Json;
using SuRGeoNix.BitSwarmLib;

using System;
using System.IO;
using System.Data;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using SharpCompress.Common;
using System.Net;
using SharpCompress.Archives;
using System.Linq;
using SharpCompress.Readers;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives.Rar;
using System.Windows.Threading;
using HtmlAgilityPack;

namespace Index
{
    public class Archive
    {
        public string ArchivePassword
        {
            get; set;
        }
        public List<string> Archives
        {
            get; set;
        }
    }

    public class Download
    {
        public string InfoHash
        {
            get; set;
        }
        public int Type
        {
            get; set;
        }
        public string LatestVersion
        {
            get; set;
        }
        public string CRC32
        {
            get; set;
        }
    }

    public class Images
    {
        public string Icon
        {
            get; set;
        }
        public List<string> Banners
        {
            get; set;
        }
    }

    public class Metadata
    {
        public Int16 ID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
    }

    public class Names
    {
        public string LaunchName
        {
            get; set;
        }
        public string SetupName
        {
            get; set;
        }
    }

    public class GameData
    {
        public Metadata Metadata
        {
            get; set;
        }
        public Download Download
        {
            get; set;
        }
        public Names Names
        {
            get; set;
        }
        public Archive Archive
        {
            get; set;
        }
        public Images Images
        {
            get; set;
        }
    }

    internal enum DownloadStatus
    {
        Preparing = 0,
        Downloading= 1,
        DVerifying = 2,
        Verifying = 3,
        Uninstalling = 4,
        Paused = 5,
        Extracting = 6,
        Completed = 7
    }

    internal enum InstallationType
    {
        None = 0,
        Setup = 1,
        RAR = 2,
        ZIP = 3
    }

    internal enum DownloadTable
    {
        ID = 0,
        DLID = 1,
        Name = 2,
        Procentage = 3,
        Paused = 4,
        ETA = 5,
        Speed = 6,
        Status = 7,
        Banner = 8
    }

    internal enum GameTable
    {
        ID = 0,
        IndexID = 1,
        Name = 2,
        Description = 3,
        Banner = 4,
        InfoHash = 5
    }

    public class Data
    {
        public static Options opt = new();

        public static BitSwarm[] bitswarm = { new(opt), new(opt), new(opt) };

        public static List<GameData> games = Methods.GetData();

        public static string visible = "Home";

        public static int dls = 0;

        public static DataTable dltable = new() {
            Columns = {
                new DataColumn("id", Type.GetType("System.Int16")),
                new DataColumn("dlid", Type.GetType("System.Int16")),
                new DataColumn("name", Type.GetType("System.String")),
                new DataColumn("procentage", Type.GetType("System.Int16")),
                new DataColumn("paused", Type.GetType("System.Boolean")),
                new DataColumn("eta", Type.GetType("System.Int16")),
                new DataColumn("speed", Type.GetType("System.Int16")),
                new DataColumn("status"),
                new DataColumn("banner", Type.GetType("System.String"))
            }
        };
        public static DataTable gametable = new() {
            Columns = {
                new DataColumn("id", Type.GetType("System.Int16")),
                new DataColumn("indexid", Type.GetType("System.Int16")),
                new DataColumn("name", Type.GetType("System.String")),
                new DataColumn("description", Type.GetType("System.String")),
                new DataColumn("banner", Type.GetType("System.String")),
                new DataColumn("infohash", Type.GetType("System.String")),
            }
        };
    }

    public class Methods
    {
        // Fetches data from my game database
        public static List<GameData> GetData()
        {
            Trace.WriteLine("Fetching data");

            try
            {
                WebClient wc = new();

                var json = wc.DownloadString(new Uri("https://raw.githubusercontent.com/OmarHopman/index/info/database.json"));

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var list = JsonConvert.DeserializeObject<List<GameData>>(json);

                    Trace.WriteLine($"Successfuly fetched {list.Count} games");
                    return JsonConvert.DeserializeObject<List<GameData>>(json);
                }
                else
                {
                    CheckConnection("Whitespace error");
                    return null;
                }
            }
            catch(Exception ex)
            {
                CheckConnection($"Critical unhandled error\n\n{ex}");
                return null;
            }
        }

        // Work in progress switch from torrent to cs.rin.ru (optional setting)
        public static void Rin(string username, string password)
        {
            var pages = 1;

            foreach (var page in Enumerable.Range(1, pages))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load("https://cs.rin.ru/forum/viewforum.php?f=22&start=" + page + "00");

                var gimz = htmlDoc.DocumentNode.Descendants("tr")
                    .ToList();

                foreach (var gim in gimz)
                {
                    MessageBox.Show($"Found SCS game {gim}");
                }

                MessageBox.Show($"Indexed all games of page {page}");
            }

            MessageBox.Show($"Successfully indexed {pages} pages");
        }

        // Checks your connection with answer of status
        public static void CheckConnection(string source)
        {
            Trace.WriteLine("Checking connection");

            try
            {
                var ping = new Ping();
                var pingResult = ping.Send("https://github.com/");

                if (pingResult.Status == IPStatus.Success)
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        MessageBox.Show($"Unknown error, please report at Discord\n\n{source}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to connect to network\n\n{source}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    MessageBox.Show($"Unreachable database error\n\n{pingResult.Status}\n{source}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"An error occurred while checking network availability\n{source}\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        // Converts bytes to appropriate suffix
        private static string FormatBytes(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;

            for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, suffix[i]);
        }

        // Downloads game by ID
        public static void Download(int gameID)
        {
            var game = Data.games[gameID];

            if (game == null)
            {
                MessageBox.Show("Invalid game ID", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var directory = Properties.Settings.Default.Directory;

            if (string.IsNullOrWhiteSpace(directory))
            {
                MessageBox.Show($"Default download directory is not set for installation of {game.Metadata.Name}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Data.dls >= 3)
            {
                MessageBox.Show($"Maximum of 3 downloads reached for installation of {game.Metadata.Name}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var folder = $"{directory}/{game.Metadata.Name}";
            var download = $"{directory}/{game.Metadata.Name}/Download";

            if (Directory.Exists($"{directory}/{game.Metadata.Name}"))
            {
                var result = MessageBox.Show($"A folder already exists with the name {game.Metadata.Name}, are you sure you want to install here?", "Index", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Installation of {game.Metadata.Name} is cancelled", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Directory.Delete($"{directory}/{game.Metadata.Name}", true);
            }

            Trace.WriteLine("Starting download with ID " + gameID);

            var row = Data.dltable.Rows.Add();
            var dl = Data.dltable.Rows.IndexOf(row);
            Data.dls++;

            var bs = Data.bitswarm[dl];

            row.BeginEdit();

            row.ItemArray = new object[]{
                row[(int)DownloadTable.ID] = gameID,
                row[(int)DownloadTable.DLID] = dl,
                row[(int)DownloadTable.Name] = game.Metadata.Name,
                row[(int)DownloadTable.Banner] = game.Images.Banners[0],
                row[(int)DownloadTable.Status] = DownloadStatus.Preparing,
                row[(int)DownloadTable.Procentage] = 0
            };

            row.EndEdit();
            row.AcceptChanges();

            bs.StatsUpdated += (sender, e) => TorrentUpdated(sender, e, dl);
            bs.StatusChanged += (sender, e) => TorrentChanged(sender, e, dl);

            Directory.CreateDirectory(download);

            bs.Options.FolderComplete = download;
            bs.Options.FolderIncomplete = download;
            bs.Options.FolderTorrents = download;

            bs.Open(game.Download.InfoHash);
            bs.Start();
        }

        // Verifies game by ID (Work in progress)
        public static void Verify(int gameID)
        {
            var game = Data.games[gameID];

            if (game == null)
            {
                MessageBox.Show("Invalid game ID", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Data.dls >= 3)
            {
                MessageBox.Show($"Maximum of 3 downloads reached for installation of gameID {gameID}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var row = Data.dltable.Rows.Add();
            var dl = Data.dltable.Rows.IndexOf(row);
            Data.dls++;

            row.BeginEdit();

            row.ItemArray = new object[]{
                row[(int)DownloadTable.ID] = gameID,
                row[(int)DownloadTable.DLID] = dl,
                row[(int)DownloadTable.Name] = game.Metadata.Name,
                row[(int)DownloadTable.Banner] = game.Images.Banners[0],
                row[(int)DownloadTable.Status] = DownloadStatus.Preparing,
                row[(int)DownloadTable.Procentage] = 0
            };

            row.EndEdit();
            row.AcceptChanges();

            Trace.WriteLine("Verifiying game with ID " + gameID);

            row[(int)DownloadTable.Status] = DownloadStatus.DVerifying;

            // Verify process here

            row.Delete();
        }

        // Uninstalls game by ID
        public static void Uninstall(int gameID)
        {
            var game = Data.games[gameID];

            if (game == null)
            {
                MessageBox.Show("Invalid game ID", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Directory))
            {
                MessageBox.Show("Default download directory is not set", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Data.dls >= 3)
            {
                MessageBox.Show("Maximum of 3 downloads reached", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var row = Data.dltable.Rows.Add();
            var dl = Data.dltable.Rows.IndexOf(row);
            Data.dls++;

            row.BeginEdit();

            row.ItemArray = new object[]{
                row[(int)DownloadTable.ID] = gameID,
                row[(int)DownloadTable.DLID] = dl,
                row[(int)DownloadTable.Name] = game.Metadata.Name,
                row[(int)DownloadTable.Banner] = game.Images.Banners[0],
                row[(int)DownloadTable.Status] = DownloadStatus.Uninstalling,
                row[(int)DownloadTable.Procentage] = 0
            };

            row.EndEdit();
            row.AcceptChanges();

            Trace.WriteLine("Uninstalling game with ID " + gameID);

            if (Process.GetProcessesByName(game.Names.LaunchName).Length > 0)
            {
                var processes = Process.GetProcesses();
                var i = 0;
                for (var p = 0; i < processes.Length; i++)
                {
                    if (processes[p].ProcessName == game.Names.LaunchName)
                    {
                        processes[p].Kill();
                    }
                }
            }

            Directory.Delete($"{Properties.Settings.Default.Directory}/{game.Metadata.Name}", true);

            row.Delete();
        }

        // Updates the download status or extracts when done
        private static void TorrentChanged(object source, BitSwarm.StatusChangedArgs e, int gameID)
        {
            var row = Data.dltable.Select($"id = '{gameID}'")[0];

            if (e.Status == 0)
            {
                row[(int)DownloadTable.Status] = DownloadStatus.Downloading;
            }
            if (e.Status == 1)
            {
                row[(int)DownloadTable.Status] = DownloadStatus.Paused;
            }
            if (e.Status == 2)
            {
                row[(int)DownloadTable.Status] = DownloadStatus.Extracting;

                var game = Data.games[gameID];

                if (Data.games == null)
                {
                    MessageBox.Show($"Game not found with ID {gameID}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var directory = $"{Properties.Settings.Default.Directory}/{game.Metadata.Name}";

                foreach (var arch in game.Archive.Archives)
                {
                    var archiveDirectory = $"{directory}/Download/{arch}";

                    var type = Path.GetExtension(archiveDirectory);

                    var roptions = new ReaderOptions
                    {
                        Password = game.Archive.ArchivePassword
                    };

                    if (game.Archive.ArchivePassword == null)
                    {
                        roptions.Password = "";
                    }

                    var eoptions = new ExtractionOptions
                    {
                        Overwrite = true
                    };

                    long completed = 0;

                    if (string.Equals(type, ".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        var zip = ZipArchive.Open(new FileInfo(archiveDirectory), roptions);

                        double totalSize = zip.Entries.Where(e => !e.IsDirectory).Sum(e => e.Size);

                        foreach (var entry in zip.Entries.Where(e => !e.IsDirectory))
                        {
                            entry.WriteToDirectory(directory, eoptions);

                            completed += entry.Size;
                            Trace.WriteLine($"{completed / totalSize}%");
                        }
                    }
                    if (string.Equals(type, ".rar", StringComparison.OrdinalIgnoreCase))
                    {
                        var rar = RarArchive.Open(new FileInfo(archiveDirectory), roptions);

                        double totalSize = rar.Entries.Where(e => !e.IsDirectory).Sum(e => e.Size);

                        foreach (var entry in rar.Entries.Where(e => !e.IsDirectory))
                        {
                            entry.WriteToDirectory(directory, eoptions);

                            completed += entry.Size;
                            Trace.WriteLine($"{completed / totalSize}%");
                        }
                    }
                }

                row[(int)DownloadTable.Status] = DownloadStatus.Completed;

                row.Delete();
            }
        }

        // Updates the torrents data when updated
        private static void TorrentUpdated(object source, BitSwarm.StatsUpdatedArgs e, int id)
        {
            try
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    var window = Application.Current.MainWindow;

                    var row = Data.dltable.Select($"id = '{id}'")[0];

                    (window as Main).Taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                    (window as Main).Taskbar.ProgressValue = decimal.ToDouble((decimal)e.Stats.Progress / 100);

                    row[(int)DownloadTable.Procentage] = e.Stats.Progress;
                    row[(int)DownloadTable.ETA] = TimeSpan.FromSeconds((e.Stats.ETA + e.Stats.AvgETA) / 2).ToString(@"mm\:ss");
                    row[(int)DownloadTable.Speed] = FormatBytes(e.Stats.DownRate);
                    row[(int)DownloadTable.Status] = DownloadStatus.Downloading;
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Failed to update torrent with ID {id}\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}