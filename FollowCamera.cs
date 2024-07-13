using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Redshift
{
    // Should this just be renamed to Camera and have different modes?
    internal class FollowCamera
    {
        private Vector2 position;
        private Vector2 desiredPosition;
        private float smoothSpeed = 0.0001f;
        private Entity target;
        private EntityManager entityManager;

        private int screenWidth;
        private int screenHeight;
        private int boundaryWidth;
        private int boundaryHeight;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public FollowCamera(Viewport viewport, Entity entity, EntityManager entityManager)
        {
            this.target = entity;
            this.entityManager = entityManager;
            
            screenWidth = viewport.Width;
            screenHeight = viewport.Height;
            boundaryWidth = (int)(screenWidth * 0.8f);
            boundaryHeight = screenHeight / 3;

            Projection = Matrix.CreateTranslation(screenWidth / 2, screenHeight / 2, 0);
            UpdateView();
        }

        public void Update()
        {
            desiredPosition = entityManager.GetComponent<TransformComponent>(target).Position;

            // Calculate the boundaries
            float leftBoundary = position.X - boundaryWidth / 2;
            float rightBoundary = position.X + boundaryWidth / 2;
            float topBoundary = position.Y - boundaryHeight / 2;
            float bottomBoundary = position.Y + boundaryHeight / 2;

            // Update the camera position if the player is out of the boundaries
            if (desiredPosition.X < leftBoundary)
            {
                position.X = desiredPosition.X + boundaryWidth / 2;
                position = Vector2.Lerp(position, desiredPosition, smoothSpeed);
            }
            else if (desiredPosition.X > rightBoundary)
            {
                position.X = desiredPosition.X - boundaryWidth / 2;
                position = Vector2.Lerp(position, desiredPosition, smoothSpeed);
            }

            if (desiredPosition.Y - 250 < topBoundary)
            {
                position.Y = desiredPosition.Y - 250 + boundaryHeight / 2;
                position = Vector2.Lerp(position, desiredPosition, smoothSpeed);
            } 
            else if (desiredPosition.Y - 250 > bottomBoundary)
            {
                position.Y = desiredPosition.Y - 250 - boundaryHeight / 2;
                position = Vector2.Lerp(position, desiredPosition, smoothSpeed);
            }

            UpdateView();
        }

        public void UpdateView()
        {
            View = Matrix.CreateTranslation(-position.X - 64, -position.Y - 64, 0);
        }
    }
}
