using Microsoft.Xna.Framework;

namespace Wall {
    public class Enemy : Entity {

        public Player player;

        protected float meleeDelay = 1F, meleeTimer;
        
        
        public Enemy(Vector2 pos) : base(pos) {
            player = Wall.player;
        }

        public override void update(float deltaTime) {
            
            attackTick(deltaTime);
            meleeTimer -= deltaTime;
            
            base.update(deltaTime);
        }

        public virtual void attackTick(float deltaTime) {
            
        }

        public void meleeAttack(Entity target, float damage, float knockBack) {
            if (collidesWith(target) && meleeTimer <= 0) {
                meleeTimer = meleeDelay;
                target.damaged(damage);
                target.knockedBack(knockBack, pos);
            }
        }
    }
}