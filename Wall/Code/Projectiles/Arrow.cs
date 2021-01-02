using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Arrow : StickableProjectile {
        
        public Arrow(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;

            damage = 5;
            knockback = 13;
        }

        public override float findRotation() {
            return base.findRotation() + Maths.halfPI;
        }

        public override void bonk(Vector2 newPos) {
            base.bonk(newPos);
            getStuck();
        }

        public override bool collidesAt(Vector2 pos, Vector2 dimen) {
            return Wall.map.pointCollide(pos + Util.polar(dimen.Y / 2, velAngle()));
        }
    }
}