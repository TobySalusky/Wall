using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wall {
    public class FryingPan : Sword {
        
        public FryingPan(int count) : base(count) {
            chunkCount = 1;
            chunkSize = 2F;
            
            offset = 4;
            damage = 4;
            knockback = 25;

            useDelay = 0.8F;
            swingTime = 0.4F;

            handOffset = Vector2.UnitX * -2.5F;
        }
        
        public override MeleeAttack createAttackChunk(List<Entity> hasHit, float mult) {
            return new FryingPanMelee(player.pos, true) {stunProb = Math.Max(0.15F, 0.75F * specialChargeAmount()), hasHit = hasHit, damage = damage * mult, knockback = knockback, dimen = Vector2.One * chunkSize};
        }
    }

    public class FryingPanMelee : MeleeAttack {
        public FryingPanMelee(Vector2 pos, bool playerOwned) : base(pos, playerOwned) { }

        public float stunProb;
        
        public override void hit(Entity entity) {
            base.hit(entity);
            if (Util.chance(stunProb))
                entity.stunTimer = 1F;
        }
    }
}