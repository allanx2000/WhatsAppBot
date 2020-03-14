using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppBot.Client
{
    public class AZLyricsClient : SeleniumClient
    {
        private readonly Properties.Settings Settings = Properties.Settings.Default;

        private readonly string LyricsFolder;
        private readonly string BaseUrl = "https://www.azlyrics.com/";

        public AZLyricsClient() : base(false)
        {
            LyricsFolder = Settings.LyricsStore;

            var existing = from fi
                           in new DirectoryInfo(LyricsFolder).GetFiles()
                           select fi.FullName;

            lyricsList.AddRange(existing);

        }

        private List<string> currentLyric = null;
        private List<string> lyricsList = new List<string>();

        private Random random = new Random();

        public string GetRandom()
        {
            var loadNewLyric = random.Next(0, 100) < 60;
            if (currentLyric == null || loadNewLyric)
            {
                var downloadNew = random.Next(0, 100) < 50;
                if (lyricsList.Count == 0 || downloadNew)
                    DownloadNew();
                else
                    LoadLyrics(lyricsList[random.Next(lyricsList.Count)]);
            }

            var idx = random.Next(0, currentLyric.Count);
            return currentLyric[idx];
        }

        private Dictionary<string, List<string>> letterArtistCache = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> artistSongCache = new Dictionary<string, List<string>>();


        private void DownloadNew()
        {
            int letter = random.Next(97, 124);
            string page = letter == 123 ? "19" : ((char)letter).ToString();

            if (!letterArtistCache.ContainsKey(page))
            {
                LoadArtists(page);
            }

            var artists = letterArtistCache[page];
            string artist = artists[random.Next(0, artists.Count)];

            if (!artistSongCache.ContainsKey(artist))
            {
                LoadSongs(artist);
            }
            var songs = artistSongCache[artist];
            string song = songs[random.Next(0, songs.Count - 1)];

            //TODO: Check already downloaded
            SaveSong(artist, song);
        }

        private void SaveSong(string artist, string song)
        {
            var parts = song.Split('/');
            string name = parts[1] + "-" + parts[2];
            name = name.Split('.')[0];

            //TODO: Check exists

            Driver.Navigate().GoToUrl($"{BaseUrl}{song}");
            var lyrics = Driver.FindElementByXPath("//div[contains(@class,'col-xs-12 col-lg-8 text-center')]").Text;

            //Clean
            StringBuilder sb = new StringBuilder();
            var lines = lyrics.Split('\n');
            var start = false;
            foreach (var l in lines)
            {
                if (l == "\r" && start == false)
                {
                    start = true;
                    continue;
                }

                if (l.Contains("Submit Corrections"))
                    break;

                if (start)
                {
                    var cleaned = l.Trim();
                    if (cleaned.Length==0)
                    {
                        sb.AppendLine();
                    }
                    else
                        sb.AppendLine(cleaned);
                }
            }

            var text = sb.ToString();
            var fileName = Path.Combine(LyricsFolder, name + ".txt");

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(text);
            }

            lyricsList.Add(fileName);

            LoadLyrics(fileName);
        }

        private void LoadLyrics(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                List<string> llines = new List<string>();
                while (!sr.EndOfStream)
                {
                    var l = sr.ReadLine();
                    if (l.Length > 0)
                        llines.Add(l);
                }

                currentLyric = llines;
            }
        }

        private void LoadSongs(string artistUrl)
        {
            //Includes letter
            var artistName = artistUrl.Split('/')[1].Split('.')[0];

            Driver.Navigate().GoToUrl($"{BaseUrl}{artistUrl}");
            var links = Driver.FindElementsByXPath($"//a[contains(@href,'/lyrics/')]");

            var songs = new List<string>();
            artistSongCache[artistUrl] = songs;

            foreach (var l in links)
            {
                songs.Add(l.GetAttribute("href").Replace(BaseUrl, ""));
            }
        }

        private void LoadArtists(string page)
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}{page}.html");
            var links = Driver.FindElementsByXPath($"//a[contains(@href,'{page}/')]");

            var artists = new List<string>();
            letterArtistCache[page] = artists;

            foreach (var l in links)
            {
                artists.Add(l.GetAttribute("href").Replace(BaseUrl, ""));
            }
        }
    }
}
