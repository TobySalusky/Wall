using Microsoft.Xna.Framework;

namespace Wall {
    public class WormSegment : Enemy {

        public float segmentOffset = 5;
        public WormSegment inFront;
        public WormHead head;
        public bool shareHealth = true;
        
        public WormSegment(Vector2 pos) : base(pos) {

            hasCollision = false;
            hasGravity = false;
            
            initTexture("bush");
        }

        public override void die() {
            base.die();
            snowPuffDeath();
        }

        public override void damaged(float damage) {
            if (shareHealth && !Util.isClassOrSub(this, typeof(WormHead))) {
                float saveTime = head.timeSinceDamaged;
                head.damaged(damage);
                head.timeSinceDamaged = saveTime;
            }

            base.damaged(damage);
        }

        public override bool shouldDieDamaged() {
            return base.shouldDieDamaged() && (!shareHealth || Util.isClassOrSub(this, typeof(WormHead)));
        }

        public virtual void wormUpdate(float deltaTime) {
            float angle = Util.angle(inFront.pos - pos);
            pos = inFront.pos - Util.polar(segmentOffset, angle);
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            
            wormUpdate(deltaTime);
            
            meleeAttack(player, 10, 10);
        }
    }
}