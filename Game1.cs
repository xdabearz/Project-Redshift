using Redshift.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Redshift
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private World world;
        private InputState input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            world = new World();

            // This is relative to where the .exe runs from. Will be created on runtime if it doesn't exist
            input = new InputState("Data/keybinds.json");

            base.Initialize();
        }

        // Load initial resources
        protected override void LoadContent()
        {
            world.CreatePlayer(Content);
            world.SpawnEnemy(Content);
            world.AddCamera(GraphicsDevice.Viewport);
            world.WeaponSystem.LoadBulletTexture(Content.Load<Texture2D>("bullet"));

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Capture the input
            input.GetCurrentInputState();

            // Special case exit for now
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Create actions based on the input. This is sent to all entities with an input component
            // where inputEnabled is true
            if (input.Movement.Length() > 0.0f)
            {
                world.QueueInputCommand(input.Movement, "Move", gameTime);
            }

            if (input.CurrentKeyboardState.IsKeyDown(input.GetKeybind("FireWeapon")))
            {
                // Yeah, this needs to change. Having to pass the movement even for non-movement commands is bad
                world.QueueInputCommand(Vector2.Zero, "Shoot", gameTime);
            }

            world.Update(gameTime);

            base.Update(gameTime);
        }

        // Draws the initial scene
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var camera = world.Camera;
            spriteBatch.Begin(transformMatrix: camera.View * camera.Projection);

            world.Draw(gameTime, spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}