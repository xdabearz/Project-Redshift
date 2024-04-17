using Gala.Manager;
using Gala.Systems;
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
        private Texture2D player;
        private Vector2 playerPosition;
        private EnemyManager enemyManager;
        private InputSystem inputSystem;
        private  float moveSpeed = 300; // pixels per second

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

            inputSystem = new InputSystem("Data/keybinds.json");

            enemyManager = new EnemyManager(Content.Load<Texture2D>("enemy"));

            //First is X, second is Y. We subtract 64 from the location because that is the size of the ship (default top left)
            playerPosition = new Vector2(400-64, 400-64);
            
            base.Initialize();
        }

        // Load initial resources
        protected override void LoadContent()
        {
            player = Content.Load<Texture2D>("ship");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 moveDirection = inputSystem.GetMovement();
            playerPosition += moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            enemyManager.Update(gameTime);

            base.Update(gameTime);
        }

        //Draws the initial scene
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(player, playerPosition, Color.White);
            enemyManager.DrawEnemies(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}