using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace Wall
{

    public class MusicPlayer
    {
        public Dictionary<string, Song> songs;
        public Song curSong;
        public MusicPlayer(Dictionary<string, Song> songs)
        {
            this.songs = songs;
        }

        public void play(string songName) {
            curSong = songs[songName];
            MediaPlayer.Play(curSong);
            //  MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            // 0.0f is silent, 1.0f is full volume
            MediaPlayer.Volume -= 0.1f;
            MediaPlayer.Play(curSong);
        }
    }
}
