using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Wall.OutsideSamples;

namespace Wall
{
    public class Wall : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private static Wall instance;

        private FrameCounter fpsCounter = new FrameCounter();
        private int secondsPassed = 0;
        
        public static Player player;
        public static Camera camera;
        public static ChunkMap map;
        public static List<Entity> entities = new List<Entity>();
        public static List<Entity> bosses = new List<Entity>();
        public static List<Projectile> projectiles = new List<Projectile>();
        public static List<Particle> particles = new List<Particle>();
        public static List<Entity> playerList = new List<Entity>();
        public static List<GroundItem> items = new List<GroundItem>();

        public static ButtonState lastLeft, lastMiddle, lastRight;
        public static int lastScroll;
        public static KeyboardState lastKeyState;
        public static MouseInfo lastMouseInfo;

        public static bool F3Enabled;
        public static bool entitySpawning = true;
        public static bool paused;
        public static int deaths;

        public static EntityHandler entityHandler;

        public static Texture2D cursor;
        public static Vector2 cursorDimen;

        public static MusicPlayer musicPlayer;

        public static Effect testShader;
        public static Effect otherShader;

        public static bool contentLoaded = false;

        public static float currentDeltaTime;
        
        public Wall()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            
            // These remove the framerate limit
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            
            instance = this;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;

            graphics.ApplyChanges();
            
            base.Initialize();
            
            Textures.loadTextures();
            Chunk.loadMapData();
            Item.loadItems();
            Entity.loadEntities();
            MusicPlayer.loadSongs();
            
            musicPlayer = new MusicPlayer();

            map = new ChunkMap();
            player = new Player(new Vector2(25, 400));
            playerList.Add(player);
            camera = new Camera(new Vector2(0, 0), 24F);

            entityHandler = new EntityHandler();
            
            cursor = Textures.get("Cursor");
            cursorDimen = Vector2.One * 30;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            Fonts.arial = Content.Load<SpriteFont>("BaseFont");

            testShader = Content.Load<Effect>("Gaussian");

            float pix = 0.05F;
            testShader.Parameters["sampleOffsets"].SetValue(new Vector2[] {
                new Vector2(-pix * 2, -pix * 2),
                new Vector2(-pix, -pix * 2),
                new Vector2(0, -pix * 2),
                new Vector2(pix, -pix * 2),
                new Vector2(pix * 2, -pix * 2),
                
                new Vector2(-pix * 2, -pix),
                new Vector2(-pix, -pix),
                new Vector2(0, -pix),
                new Vector2(pix, -pix),
                new Vector2(pix * 2, -pix),
                
                new Vector2(-pix * 2, 0),
                new Vector2(-pix, 0),
                new Vector2(0, 0),
                new Vector2(pix, 0),
                new Vector2(pix * 2, 0),
                
                new Vector2(-pix * 2, pix),
                new Vector2(-pix, pix),
                new Vector2(0, pix),
                new Vector2(pix, pix),
                new Vector2(pix * 2, pix),
                
                new Vector2(-pix * 2, pix * 2),
                new Vector2(-pix, pix * 2),
                new Vector2(0, pix * 2),
                new Vector2(pix, pix * 2),
                new Vector2(pix * 2, pix * 2),
            });
            
            testShader.Parameters["sampleWeights"].SetValue(new float[] {
                0.003765F,
                0.015019F,
                0.023792F,
                0.015019F,
                0.003765F,
                
                0.015019F,
                0.059912F,
                0.094907F,
                0.059912F,
                0.015019F,
                
                0.023792F,
                0.094907F,
                0.150342F,
                0.094907F,
                0.023792F,
                
                0.015019F,
                0.059912F,
                0.094907F,
                0.059912F,
                0.015019F,
                
                0.003765F,
                0.015019F,
                0.023792F,
                0.015019F,
                0.003765F,
            });

            otherShader = Content.Load<Effect>("OffPixel");

            contentLoaded = true;
        }

        public void reloadTextures() {
            Textures.loadTextures();
            Tile.genAtlas();
            foreach (var chunk in map.chunks.Values) {
                chunk.loaded = false;
            }
        }

        private float delta(GameTime gameTime) {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        public string F3Info() {
            const string tab = "   ";
            
            StringBuilder str = new StringBuilder();
            str.AppendLine("Entities: " + entityHandler.entityCount());
            str.AppendLine("All Entities: " + entities.Count);
            str.AppendLine("Particles: " + particles.Count);
            str.AppendLine("Projectiles: " + projectiles.Count);
            str.AppendLine("Items: " + items.Count);
            str.AppendLine("");
            str.AppendLine("Player:");
            str.AppendLine(tab + "Pos: <" + Math.Round(player.pos.X) + " " + Math.Round(player.pos.Y) + ">");
            str.AppendLine(tab + "Vel: <" + Math.Round(player.vel.X) + " " + Math.Round(player.vel.Y) + ">");
            str.AppendLine(tab + "Deaths: " + deaths);
            str.AppendLine("");
            str.AppendLine("Mouse Angle: " + Util.angle(lastMouseInfo.pos - camera.toScreen(player.pos)));
            str.AppendLine("TerrariaMode: " + player.terrariaMode);
            str.AppendLine("Entity Spawning: " + entitySpawning);
            return str.ToString();
        }

        protected override void Update(GameTime gameTime) {

            
            float deltaTime = delta(gameTime);
            currentDeltaTime = deltaTime;
            fpsCounter.update(deltaTime);
            if (gameTime.TotalGameTime.Seconds > secondsPassed) {
                secondsPassed = gameTime.TotalGameTime.Seconds;
                Window.Title = "FPS: " + (int) fpsCounter.AverageFramesPerSecond;
            }

            if (lastKeyState == null) { 
                lastKeyState = Keyboard.GetState();
            }

            KeyboardState keyState = Keyboard.GetState();
            
            KeyInfo keys = new KeyInfo(keyState, lastKeyState);
            lastKeyState = keyState;

            MouseState mouseState = Mouse.GetState();

            bool leftChange = lastLeft != mouseState.LeftButton;
            lastLeft = mouseState.LeftButton;
            
            bool middleChange = lastMiddle != mouseState.MiddleButton;
            lastMiddle = mouseState.MiddleButton;
            
            bool rightChange = lastRight != mouseState.RightButton;
            lastRight = mouseState.RightButton;

            int scroll = -Math.Sign(mouseState.ScrollWheelValue - lastScroll); // TODO: fix: WARNING: can only scroll in intervals of one per update
            lastScroll = mouseState.ScrollWheelValue;

            MouseInfo mouseInfo = new MouseInfo(mouseState, leftChange, middleChange, rightChange, scroll);
            lastMouseInfo = mouseInfo;
            
            keyInput(keys);
            
            if (!paused) {
            
                if (entitySpawning)
                    entityHandler.update(camera, deltaTime);
                
                player.keyInput(mouseState, keys, deltaTime);
                player.mouseInput(mouseInfo, deltaTime);
                player.update(deltaTime);
                
            
                for (int i = entities.Count - 1; i >= 0; i--) {
                    Entity entity = entities[i];

                    if (entity.deleteFlag) {
                        entities.RemoveAt(i);
                        continue;
                    }

                    entity.update(deltaTime);
                }
                
                for (int i = bosses.Count - 1; i >= 0; i--) {
                    Entity boss = bosses[i];

                    if (boss.deleteFlag) {
                        bosses.RemoveAt(i);
                    }
                }

                for (int i = projectiles.Count - 1; i >= 0; i--) {
                    Projectile projectile = projectiles[i];

                    if (projectile.deleteFlag) {
                        projectiles.RemoveAt(i);
                        continue;
                    }

                    projectile.update(deltaTime);
                }

                for (int i = particles.Count - 1; i >= 0; i--) {
                    Particle particle = particles[i];

                    if (particle.deleteFlag) {
                        particles.RemoveAt(i);
                        continue;
                    }

                    particle.update(deltaTime);
                }

                for (int i = items.Count - 1; i >= 0; i--) {
                    GroundItem item = items[i];

                    if (item.deleteFlag) {
                        items.RemoveAt(i);
                        continue;
                    }

                    item.update(deltaTime);
                }

                camera.pos = player.pos;
            } else if (player.inventoryOpen) {
                player.inventoryMouseInput(mouseInfo, deltaTime);
            }

            base.Update(gameTime);
        }

        private void keyInput(KeyInfo keys) {
            if (keys.down(Keys.Escape))
                closeGame();

            if (keys.pressed(Keys.F3))
                F3Enabled = !F3Enabled;
            if (keys.pressed(Keys.P))
                paused = !paused;
            
            if (keys.pressed(Keys.Tab)) {
                player.inventoryOpen = !player.inventoryOpen;
                paused = player.inventoryOpen;
            }

            if (keys.pressed(Keys.K)) {
                Effect temp = testShader;
                testShader = otherShader;
                otherShader = temp;
            }

            if (keys.pressed(Keys.R) && keys.down(Keys.LeftShift)) {
                reloadTextures();
            }
            
            if (keys.pressed(Keys.S) && keys.down(Keys.LeftShift) && keys.down(Keys.LeftControl)) {
                Chunk.shadeMap();
            }

            if (keys.pressed(Keys.L)) {
                entitySpawning = !entitySpawning;
                foreach (var entity in entities) {
                    if (entity.canDespawn)
                        entity.despawn();
                }
            }
            
            // camera controls
            if (keys.down(Keys.OemMinus) && keys.down(Keys.LeftControl)) {
                camera.scale -= 1F;
            }

            if (keys.down(Keys.OemPlus) && keys.down(Keys.LeftControl)) {
                camera.scale += 1F;
            }

            // song stuff
            if (keys.pressed(Keys.NumPad1))
                musicPlayer.play("Cave");
            if (keys.pressed(Keys.NumPad2))
                musicPlayer.play("Snake");
            if (keys.pressed(Keys.NumPad3))
                musicPlayer.play("Lab");
        }

        public void renderCursor(Vector2 pos) {
            spriteBatch.Draw(cursor, Util.center(pos, cursorDimen), Color.White);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!contentLoaded) {
                return;
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);

            map.render(camera, spriteBatch);

            foreach (var item in items) {
                item.render(camera, spriteBatch);
            }
            
            foreach (var projectile in projectiles) {
                projectile.render(camera, spriteBatch);
            }
            
            foreach (var entity in entities) {
                entity.render(camera, spriteBatch);
            }

            player.render(camera, spriteBatch);
            
            foreach (var particle in particles) {
                particle.render(camera, spriteBatch);
            }

            foreach (var boss in bosses) {
                Bosses.renderHealthBar(boss, 980, spriteBatch);
            }

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

            spriteBatch.End();

            // Shading
            map.renderShading(camera, spriteBatch);

            // UI
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);


            if (F3Enabled) { 
                spriteBatch.DrawString(Fonts.arial, F3Info(), new Vector2(50, 200), Color.White);
            }

            player.renderUI(camera, spriteBatch);
            renderCursor(mousePos);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public static GraphicsDevice getGraphicsDevice() {
            return instance.GraphicsDevice;
        }

        public static void closeGame() {
            instance.Exit();
        }
    }
}
