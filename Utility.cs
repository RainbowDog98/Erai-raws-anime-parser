﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace AnimeDownloader
{
    static class Utility
    {

        public const string TorrentClientPathWindows = @"C:\Program Files\qBittorrent\qbittorrent.exe";
        public const string TorrentClientPathLinux = @"/usr/bin/qbittorrent";
        public static WebClient WClient = new WebClient();

        public enum WindowHelp
        {
            MainWindowHelp,
            WatchlistWindowHelp
        }

        public static void DownloadAnime(string torrentLink)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(TorrentClientPathWindows, torrentLink);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start(TorrentClientPathLinux, torrentLink);
            else
                DisplayError("ERROR: can't download the torrent file");

        }
        /// <summary>
        /// Gets rid of episode number and quality tag at the beggining
        /// </summary>
        /// <param name="title">Anime title to clear</param>
        /// <returns>Cleared title</returns>
        public static string GetCleanName(ReadOnlySpan<char> title)
        {
            int sliceStart = 0;
            int sliceLength = title.Length;
            bool startedSlice = false;
            for (int i = 0; i < title.Length; i++)
            {
                if(!startedSlice && title[i] == '[')
                {
                    startedSlice = true;
                }
                else if(startedSlice && title[i] == ']')
                {
                    sliceStart = i + 2; // exclude ']' and ' ' characters
                    break;
                }
            }
            for (int i = title.Length - 1; i != 0; i--)
            {
                if (title[i] == '-')
                {
                    sliceLength = i - sliceStart;
                }
            }

            return title.Slice(sliceStart, sliceLength).ToString();
        }
        /// <summary>
        /// Get the episode number from the anime title
        /// </summary>
        /// <param name="title">Anime title</param>
        /// <returns>If successful returns the episode number, otherwise returns -1</returns>
        public static int GetAnimeEpisodeNumber(string title)
        {
            for (int i = title.Length - 1; i != 0; i--)
            {
                if (title[i] == '-')
                {
                    var newSplit = title.Substring(i).Split(' ');// FORMAT: [-][episodeNumber][othershit]...
                    if (int.TryParse(newSplit[1], out int number))
                        return number;
                    else
                        return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Display an error message on the screen
        /// </summary>
        /// <param name="infoText">Error message</param>
        public static void DisplayError(string infoText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(infoText);
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Set program setting from a "Settings.txt" file
        /// </summary>
        /// <param name="linkIndex">Index that is used for selecting anime quality</param>
        public static void SetSettingsFromFile(out int linkIndex)
        {
            if (!File.Exists("Settings.txt"))
            {
                linkIndex = 0; // Set default quality as highest
                return;
            }
            using var fs = new StreamReader("Settings.txt");
            var line = fs.ReadLine();
            var sizes = line.Split();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && sizes.Length == 3 && int.TryParse(sizes[0], out int Width) && int.TryParse(sizes[1], out int Height) && int.TryParse(sizes[2], out int LinkIndex))
            {
                Console.SetWindowSize(Width, Height);
                linkIndex = LinkIndex;
                return;
            }
            linkIndex = 0; // Set default quality as highest
        }
        /// <summary>
        /// Save program setting to a "Settings.txt" file
        /// </summary>
        /// <param name="linkIndex">Index that is used for selecting anime quality</param>
        public static void SaveSettingsToFile(ref int linkIndex)
        {
            using var fs = new StreamWriter("Settings.txt", false);
            fs.WriteLine($"{Console.WindowWidth} {Console.WindowHeight} {linkIndex}");
        }

        private static string GetHelpString(WindowHelp specificWindow) => specificWindow switch
        {
            WindowHelp.MainWindowHelp => "[Any other key - quit]\n[0-... - anime to be downloaded]\n[w - add to watch list (eg. 0 11 43 ...)]\n[dw - display watchlist]\n[sq - switch quality]\n[r - refresh]",
            WindowHelp.WatchlistWindowHelp => "[q - go back to main window]\n[0-... - download anime]\n[x - mark as downloaded (eg. 1 5 10 30 ...)]\n[r - multiple removal (eg. 1 5 10 30 ...)]"
        };
        /// <summary>
        /// Display the available commands
        /// </summary>
        /// <param name="specificWindow">Window whose commands should be displayed</param>
        public static void DisplayHelp(WindowHelp specificWindow)
        {
            Console.WriteLine("\nAvailable options:");
            Console.WriteLine(GetHelpString(specificWindow));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
