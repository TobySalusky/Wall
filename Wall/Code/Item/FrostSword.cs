namespace Wall {
    
    public class FrostSword : Sword {
        
        public FrostSword(int count) : base(count) {
            chunkCount = 4;
            chunkSize = 1;
            
            offset = 1;
            damage = 6;
            knockback = 15;

            useDelay = 0.5F;
            swingTime = useDelay;
        }
    }
}