using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class RubberArrowProjectile : ArrowProjectile {

        public int bonks;
        
        public RubberArrowProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            bonkMult = -0.8F;
            
            damage = 4;
            knockback = 20;
        }

        public override void bonk(Vector2 newPos) {
            bonks++;
            if (bonks > 3) {
                getStuck();
            }
        }
    }
}