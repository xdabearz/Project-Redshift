using Gala.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gala.Manager 
{
    internal class EnemyManager
    {
        private List<Enemy> enemies;
        private int maxEnemySpawn;
        private float time;
        private static float maxTime = 2.0f;
        private Texture2D texture;



        public EnemyManager(Texture2D texture) { 
            enemies = new List<Enemy>();
            this.texture = texture;
        }

        public void Update(GameTime gameTime)
        {
            
            if (time > maxTime || enemies.Count == 0)
            {
                time = 0;
                Enemy temp = new Enemy(texture);
                temp.SpawnEnemy(new Vector2(400 - 64, 100 - 64));

                enemies.Add(temp);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.UpdateEnemy(gameTime);
            }

            time += gameTime.ElapsedGameTime.Milliseconds / 1000f;
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.DrawEnemy(spriteBatch);
            }
        }
    }



}
