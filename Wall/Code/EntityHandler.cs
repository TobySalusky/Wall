using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wall {
    public class EntityHandler {
        
        public ChunkMap map;
        public Player player;
        public List<Entity> entities;

        public int targetCount = 8;

        public EntityHandler() {
            player = Wall.player;
            map = Wall.map;
            entities = Wall.entities;
        }

        public void update(Camera camera, float deltaTime) {

            spawn(camera, deltaTime);
            despawn(camera, deltaTime);
        }

        public int entityCount() {
            int count = 0;
            foreach (var entity in entities) {
                if (entity.useSpawnSlot) {
                    count++;
                }
            }

            return count;
        }

        public void spawn(Camera camera, float deltaTime) {
            Rectangle viewRect = camera.worldViewRect();
            Rectangle inRect = Util.center(camera.pos, new Vector2(1920, 1080) / camera.scale * 2);

            if (entityCount() < targetCount) {
                if (Util.chance(deltaTime * 2)) {
                    Vector2 pos = Util.randomInOut(inRect, viewRect);

                    Entity entity = Entity.create(getEntityType(pos), pos);

                    bool spawned = false;
                    int spawnAttempts = (entity.canSpawnInAir) ? 1 : 30;
                    for (int i = 0; i < spawnAttempts; i++) {
                        pos = (i == 0) ? entity.pos : Util.randomInOut(inRect, viewRect);
                        if (entity.mustCollideOnSpawn == entity.collidesAt(pos) && (entity.canSpawnInAir || entity.collidesAt(pos + Vector2.UnitY))) {
                            entity.pos = pos;
                            entities.Add(entity);
                            spawned = true;
                            break;
                        }
                    }

                    if (!spawned) {
                        entity.despawn();
                    }
                }
            }
        }

        public EntityType getEntityType(Vector2 pos) {
            if (Util.chance(0.7F)) {
                return EntityType.SnowSlime;
            }
            if (Util.chance(0.9F)) {
                return EntityType.LivingSnowBall;
            }

            return EntityType.SnowWorm;
        }

        public void despawn(Camera camera, float deltaTime) {

            Rectangle safeRect = Util.center(camera.pos, new Vector2(1920, 1080) / camera.scale * 2.75F);
            for (int i = entities.Count - 1; i >= 0; i--) {
                Entity entity = entities[i];
                if (entity.canDespawn && !safeRect.Contains(entity.pos)) {
                    entity.despawn();
                }
            }
        }

    }
}