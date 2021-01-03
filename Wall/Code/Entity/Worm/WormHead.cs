﻿using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class WormHead : WormSegment {
        
        public int segmentCount;
        public WormSegment[] segments;

        public bool inGround;

        public WormHead(Vector2 pos) : base(pos) {
            
            initHealth(100);

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
                Vector2 desiredVel = Util.polar(40, angleToPlayer());
                vel += (desiredVel - vel) * deltaTime * 2;
            }
        }

        public virtual void setup() {
            segmentCount = 15;
            segmentOffset = 2;
        }

        public void genSegments() {

            segments = new WormSegment[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                segments[i] = genSegment(pos + Vector2.UnitY * segmentOffset);
                segments[i].inFront = (i == 0) ? this : segments[i - 1];
                segments[i].head = this;
                segments[i].segmentOffset = this.segmentOffset;
                Wall.entities.Add(segments[i]);
            }
        }

        public WormSegment genSegment(Vector2 pos) {
            return new WormSegment(pos);
        }
    }
}