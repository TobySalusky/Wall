using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Wall
{

    public class MusicPlayer
    {
        public static Dictionary<string, Song> songs = new Dictionary<string, Song>();
        public Song curSong;
        public MusicPlayer() { }

        public void play(string songName) {
            curSong = getSong(songName);
            MediaPlayer.Play(curSong);
            //  MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            // 0.0f is silent, 1.0f is full volume
            //MediaPlayer.Volume -= 0.1f;
            MediaPlayer.Play(curSong);
        }
        
        public static Song getSong(string identifier) {
            
            songs.TryGetValue(identifier, out var song);
            
            return (song == null) ? songs["null"] : song;
        }
        
        public static void loadSongs() {
            
            processFolder(Paths.musicPath);
        }
        
        private static void processFile(string path) { // assumes a png file...
            int start = path.LastIndexOf("\\") + 1;
            int pngIndex = path.LastIndexOf(".ogg");
            
            if (pngIndex != -1) {
                string filename = path.Substring(start, pngIndex - start);
                loadSong(filename, path);
            }
        }

        private static void processFolder(string dirPath) {
            string [] files = Directory.GetFiles(dirPath);
            foreach (string file in files)
                processFile(file);

            // recursive calls
            string [] subDirs = Directory.GetDirectories(dirPath);
            foreach(string subDir in subDirs)
                processFolder(subDir);
        }

        private static void loadSong(string identifier, string absolutePath) {
            var uri = new Uri(absolutePath, UriKind.Absolute);
            Song song = Song.FromUri(identifier, uri);
            songs[identifier] = song;
        }
    }
}
