﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        
        private Player player;
        private Camera camera;
        public static ChunkMap map;
        public static List<Entity> entities = new List<Entity>();

        public Wall()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
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

            map = new ChunkMap();
            player = new Player(new Vector2(25, 400));
            camera = new Camera(new Vector2(0, 0), 24F);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
        }

        private float delta(GameTime gameTime) {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void Update(GameTime gameTime) {

            float deltaTime = delta(gameTime);
            fpsCounter.update(deltaTime);
            if (gameTime.TotalGameTime.Seconds > secondsPassed) {
                secondsPassed = gameTime.TotalGameTime.Seconds;
                Window.Title = "FPS: " + (int) fpsCounter.AverageFramesPerSecond;
            }

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            keyInput(keyState);
            player.keyInput(keyState, deltaTime);
            player.mouseInput(mouseState, deltaTime);
            player.update(deltaTime);
            
            for (int i = entities.Count - 1; i >= 0; i--) {
                Entity entity = entities[i];
                
                if (entity.deleteFlag) {
                    entities.RemoveAt(i);
                    continue;
                }
                
                entity.update(deltaTime);
            }

            camera.pos = player.pos;
            
            base.Update(gameTime);
        }

        private void keyInput(KeyboardState state) {
            if (state.IsKeyDown(Keys.Escape))
                closeGame();
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, null);

            map.render(camera, spriteBatch);

            foreach (var entity in entities) {
                entity.render(camera, spriteBatch);
            }
            
            player.render(camera, spriteBatch);
            
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
