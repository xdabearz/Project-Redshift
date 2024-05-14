using Gala.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gala
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private EnemyManager enemyManager;
        private World world;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            // This is relative to where the .exe runs from. Will be created on runtime if it doesn't exist
            world = new World("Data/keybinds.json");
            world.AddCamera(GraphicsDevice.Viewport);

            enemyManager = new EnemyManager(Content.Load<Texture2D>("enemy"));
            
            base.Initialize();
        }

        // Load initial resources
        protected override void LoadContent()
        {
            world.CreatePlayer(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.GetInput();
            world.Update(gameTime);

            enemyManager.Update(gameTime);

            base.Update(gameTime);
        }

        // Draws the initial scene
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var camera = world.Camera;
            spriteBatch.Begin(transformMatrix: camera.View * camera.Projection);

            world.Draw(gameTime, spriteBatch);
            enemyManager.DrawEnemies(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}