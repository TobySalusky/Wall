using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class IceSnake : WormHead {
        public IceSnake(Vector2 pos) : base(pos) {
            dimen = Vector2.One * 8;
            markBoss();
            
            initHealth(500);
        }

        public override void die() {
            base.die();
            dropItem(new YotsugiHat(1));
        }

        public override string findIdentifier() {
            return "SnowWorm";
        }

        public override void setup() {
            segmentCount = 15;
            segmentOffset = 6.5F;
        }

        public override void editSegment(WormSegment segment) {
            segment.dimen = Vector2.One * 7;
        }
    }
}