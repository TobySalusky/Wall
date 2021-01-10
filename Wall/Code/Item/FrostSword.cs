using System;
using Microsoft.Xna.Framework;

namespace Wall {
    
    public class FrostSword : Sword {
        
        public FrostSword(int count) : base(count) {
            chunkCount = 4;
            chunkSize = 1;
            
            offset = 1;
            damage = 6;
            knockback = 15;

            useDelay = 0.5F;
            swingTime = useDelay;
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);

            if (specialUse && isUsing()) {
                if (Util.chance(deltaTime * 300 * specialChargeAmount())) {
                    float c = Util.random(0.9F, 1F);
                    Color col = new Color(c * c, c, 1);
                    Wall.projectiles.Add(new PixelFlame(chunks[Util.randInt(1, chunks.Length)].pos,
                        Util.polar(Util.random(25, 27), angle), true) {tint = col});
                }
            }
        }
    }
}