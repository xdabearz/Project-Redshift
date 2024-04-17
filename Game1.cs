using Gala.Components;
using Gala.Manager;
using Gala.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private PhysicsSystem physics;
        private int pk; // player physics key
        private Graphics playerGraphics;
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

            // This is relative to where the .exe runs from. Will be created on runtime if it doesn't exist
            inputSystem = new InputSystem("Data/keybinds.json");
            physics = new PhysicsSystem();
            // Setting initial position when we add it
            pk = physics.Add(new Vector2(400, 400));

            enemyManager = new EnemyManager(Content.Load<Texture2D>("enemy"));
            
            base.Initialize();
        }

        // Load initial resources
        protected override void LoadContent()
        {
            playerGraphics = new Graphics(0, Content.Load<Texture2D>("ship"));
            playerGraphics.Offset = new Vector2(-64, -64);

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 moveDirection = inputSystem.GetMovement();
            // This will be more automated with a way to handle entities as groups of components
            physics.ApplyMovement(pk, moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            enemyManager.Update(gameTime);

            base.Update(gameTime);
        }

        // Draws the initial scene
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            playerGraphics.Draw(spriteBatch, physics.GetTransform(pk).Position);
            enemyManager.DrawEnemies(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}