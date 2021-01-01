using Microsoft.Xna.Framework;

namespace Wall {
    public class Enemy : Entity {

        public Player player;
        
        public Enemy(Vector2 pos) : base(pos) {
            player = Wall.player;
        }
    }
}