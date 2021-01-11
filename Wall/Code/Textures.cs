using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wall.File;

namespace Wall {
    public class Textures {

        private static Dictionary<string, Texture2D> textures;

        public static void loadTextures() {

            textures = new Dictionary<string, Texture2D>();

            textures["pixel"] = genRect(Color.White);
            textures["PixelFlame"] = genRect(Color.White);
            textures["ItemSlot"] = genRect(Color.Black);
            textures["ItemSlotSelect"] = genRect(Color.DarkGray);
            textures["HealthBar"] = genRect(Color.Red);
            textures["SpecialBar"] = genRect(Color.Purple);

            processFolder(Paths.texturePath);
        }

        public static Dictionary<string, Texture2D> debugTexturesGrab() {
            return textures;
        }

        private static Texture2D genRect(Color rectColor) {
            Texture2D rect = new Texture2D(Wall.getGraphicsDevice(), 1, 1);
            rect.SetData(new[] {rectColor});
            return rect;
        }

        private static void processFile(string path) { // assumes a png file...
            int start = path.LastIndexOf("\\") + 1;
            int pngIndex = path.LastIndexOf(".png");
            
            if (pngIndex != -1) {
                string filename = path.Substring(start, pngIndex - start);
                loadTexture(filename, path);
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

        private static void loadTexture(string identifier, string path) {
            Texture2D texture = Texture2D.FromFile(Wall.getGraphicsDevice(), path);
            textures[identifier] = texture;
        }

        public static bool has(string identifier) {
            return textures.ContainsKey(identifier);
        }

        public static Texture2D get(string identifier) {
            
            textures.TryGetValue(identifier, out var texture);

            if (texture == null) {
                return textures["null"];
            }
            return texture;
        }

    }
}