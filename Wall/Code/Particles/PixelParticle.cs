using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class PixelParticle : Particle {

        private static readonly Texture2D pixelTexture = Textures.get("pixel");
        
        public PixelParticle(Vector2 pos, Vector2 vel, Color tint) : base(pos, vel, pixelTexture) {
            this.tint = tint;
        }
    }
}