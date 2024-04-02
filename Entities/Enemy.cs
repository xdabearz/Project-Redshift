using Gala.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;

namespace Gala.Entities
{
    internal class Enemy
    {
        //initialize
        private Vector2 position;
        private Texture2D texture;


        //dynamic
        private float time;
        private static float maxTime = 2.0f;
        private int direction;


        public Enemy(Texture2D texture) {
            this.texture = texture;
            position = Vector2.Zero;
        }

        public void SpawnEnemy(Vector2 position)
        {
            this.position = position;
            time = 0f;
            direction = 0;
        } 

        public void UpdateEnemy(GameTime gameTime)
        {
            position.X += 2*direction;
            time += gameTime.ElapsedGameTime.Milliseconds/1000f;
            if (time > maxTime)
            {
                time = 0;
                direction = (direction > 0 ? -1 : 1);
            }
            //Enemy Movement: Start at 0, every 2 seconds check and swap between moving left, idle, or right. it can select the same directional for movement. If it hits the edge of the screen, it has to move the other direction

        }

        public void DrawEnemy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}