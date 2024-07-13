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
            // Special case exit for now
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
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