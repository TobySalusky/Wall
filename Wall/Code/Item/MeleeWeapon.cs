using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class MeleeWeapon : Item {

        public MeleeAttack[] chunks;
        public float damage;
        public float knockback;
        

        public MeleeWeapon(int count) : base(count) {

        }

        public override bool canUse() {
            return base.canUse() && chunks == null;
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);

            if (isUsing()) {
                positionChunks(deltaTime);
            } else {
                if (chunks != null) {
                    foreach (var chunk in chunks) {
                        chunk.deleteFlag = true;
                    }
                    chunks = null;
                }
            }
        }

        public virtual void positionChunks(float deltaTime) {
        }
    }
}