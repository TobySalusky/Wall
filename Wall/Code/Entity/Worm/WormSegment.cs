using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class WormSegment : Enemy {

        public float segmentOffset = 5;
        public WormSegment inFront;
        public WormHead head;
        public bool shareHealth = true, isHead;
        
        public WormSegment(Vector2 pos) : base(pos) {

            hasCollision = false;
            hasGravity = false;
            runParticles = false;
            mustCollideOnSpawn = true;
            canDespawn = false;
            useSpawnSlot = false;
        }

        public override void die() {
            base.die();
            snowPuffDeath();
        }
        
        public override float findRotation() {
            return Util.angle(inFront.pos - pos) + Maths.halfPI;
        }

        public override void damaged(float damage) {
            if (shareHealth && !isHead) {
                float saveTime = head.timeSinceDamaged;
                head.damaged(damage);
                head.timeSinceDamaged = saveTime;
            }

            base.damaged(damage);
        }

        public override bool shouldDieDamaged() {
            return base.shouldDieDamaged() && (!shareHealth || isHead);
        }

        public virtual void wormUpdate(float deltaTime) {
            float angle = Util.angle(inFront.pos - pos);
            pos = inFront.pos - Util.polar(segmentOffset, angle);
            vel *= Math.Max(0, 1 - deltaTime);
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            
            wormUpdate(deltaTime);
            
            meleeAttack(player, 10, 10);
        }
    }
}