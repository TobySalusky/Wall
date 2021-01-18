using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class HighTechBow : Bow {
        
        public HighTechBow(int count) : base(count) {
        }
        
        public void renderLazer(Camera camera, SpriteBatch spriteBatch) {
            Vector2 posInit = player.pos + Util.polar((offset), angle);
            Vector2 vel = Util.polar(findVelocity(), angle);
            Vector2 accel = new Vector2(0, Entity.gravity);
            
            Vector2 last = posInit;
            bool collides = false;
            for (int i = 0; i < 300; i++) {
                float time = i * 0.015F;
                Vector2 newPos = posInit + vel * time + 1/2F * accel * time * time;
                
                if (Wall.map.pointCollide(newPos)) {
                    collides = true;
                    for (int j = 10; j > 0; j--) {
                        time -= 0.001F;
                        newPos = posInit + vel * time + 1/2F * accel * time * time;
                        if (!Wall.map.pointCollide(newPos)) {
                            break;
                        }
                    }
                }
                
                if (i > 0) {
                    renderLine(last, newPos, camera, spriteBatch);
                }

                last = newPos;

                if (collides) break;
            }
        }

        public void renderLine(Vector2 from, Vector2 to, Camera camera, SpriteBatch spriteBatch) {
            Vector2 diff = Util.setMag(to - from, Tile.pixelSize);
            Vector2 size = Vector2.One * Tile.pixelSize;
            int pointCount = (int) (Util.mag(from - to) / Tile.pixelSize) + 1;
            Color col = new Color(1F, 0F, 0F, 0.3F);
            Texture2D point = Textures.get("pixel");
            for (int i = 0; i < pointCount; i++) {
                Vector2 pos = from + diff * i;
                pos /= Tile.pixelSize;
                pos.X = (float) Math.Round(pos.X);
                pos.Y = (float) Math.Round(pos.Y);
                pos *= Tile.pixelSize;
                
                spriteBatch.Draw(point, camera.toScreen(pos, size), col);
            }
        }

        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {
            base.renderInHand(camera, spriteBatch);
            
            if (hasArrow())
                renderLazer(camera, spriteBatch);
        }
    }
}