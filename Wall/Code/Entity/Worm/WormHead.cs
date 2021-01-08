using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class WormHead : WormSegment {
        
        public int segmentCount;
        public WormSegment[] segments;

        public bool inGround;
        public float groundTime, chargeTime;

        public WormHead(Vector2 pos) : base(pos) {
            
            isHead = true;
            initHealth(100);

            initTexture(findIdentifier() + "Head");

            setup();

            genSegments();
        }

        public override void die() {
            base.die();

            foreach (var segment in segments) {
                segment.die();
            }
        }

        public override void wormUpdate(float deltaTime) {

            inGround = collidesAt(pos);
            hasGravity = !inGround;
            

            if (inGround) {
                groundTime += deltaTime;
                
                Vector2 desiredVel = (groundTime > chargeTime) ? Util.polar(40, angleToPlayer()) : Util.polar(30, Util.angle(player.pos + 25 * Vector2.UnitY - pos));
                
                vel += (desiredVel - vel) * deltaTime * 2;
            } else {
                groundTime = 0;
                chargeTime = Util.random(0.75F, 2F);
            }
        }

        public virtual void setup() {
            segmentCount = 15;
            segmentOffset = 1.5F;
        }

        public override float findRotation() {
            return Util.angle(vel) + Maths.halfPI;
        }

        public virtual string findIdentifier() {
            return GetType().Name;
        }

        public void genSegments() {

            segments = new WormSegment[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                segments[i] = genSegment(pos + Vector2.UnitY * segmentOffset);
                segments[i].inFront = (i == 0) ? this : segments[i - 1];
                segments[i].head = this;
                segments[i].segmentOffset = this.segmentOffset;
                segments[i].initTexture(findIdentifier() + "Body");
                editSegment(segments[i]);
                Wall.entities.Add(segments[i]);
            }
        }

        public virtual void editSegment(WormSegment segment) {}

        public WormSegment genSegment(Vector2 pos) {
            return new WormSegment(pos);
        }
    }
}