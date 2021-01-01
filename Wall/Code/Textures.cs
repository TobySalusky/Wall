using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wall.File;

namespace Wall {
    public class Textures {

        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static void loadTextures() {
            
            Texture2D pixelParticle = new Texture2D(Wall.getGraphicsDevice(), 1, 1);
            pixelParticle.SetData(new[] { Color.White });
            textures["pixelParticle"] = pixelParticle;

            processFolder(Paths.texturePath);
        }

        private static void processFile(string path) { // assumes a png file...
            int start = path.LastIndexOf("\\") + 1;
            string filename = path.Substring(start, path.LastIndexOf(".png") - start);
            loadTexture(filename, path);
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

        public static Texture2D get(string identifier) {
            
            textures.TryGetValue(identifier, out var texture);

            if (texture == null) {
                return textures["null"];
            }
            return texture;
        }

    }
}