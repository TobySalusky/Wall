using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class ExplosionProjectile : Projectile {
        
        public ExplosionProjectile(Vector2 pos, float damage, float knockBack, Vector2 dimen, bool playerOwned) : base(pos, Vector2.Zero, playerOwned) {
            texture = null;
            this.dimen = dimen;
            this.damage = damage;
            this.knockback = knockBack;
            timeLeft = 0.1F;
            hitsLeft = -1;
        }

        public override float knockbackDir(Entity entity) {
            return Util.angle(entity.pos - pos);
        }
    }

    public class StunExplosion : ExplosionProjectile {
        public StunExplosion(Vector2 pos, bool playerOwned) : base(pos, 4, 20, Vector2.One * 2, playerOwned) {
        }

        public override void hit(Entity entity) {
            base.hit(entity);
            entity.stunTimer = 2;
        }
    }
}