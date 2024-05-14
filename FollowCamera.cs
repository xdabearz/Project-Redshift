using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gala
{
    internal class FollowCamera
    {
        private Vector2 position;
        private Vector2 desiredPosition;
        private float smoothSpeed = 0.1f;
        private int targetId;
        private EntityManager entityManager;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public FollowCamera(Viewport viewport, int targetId, EntityManager entityManager)
        {
            this.targetId = targetId;
            this.entityManager = entityManager;
            Projection = Matrix.CreateTranslation(viewport.Width/2, viewport.Height/2, 0);
            UpdateView();
        }

        public void Update()
        {
            desiredPosition = entityManager.GetTransformComponent(targetId).position;
            position = Vector2.Lerp(position, desiredPosition, smoothSpeed);
            UpdateView();
        }

        public void UpdateView()
        {
            View = Matrix.CreateTranslation(-position.X - 64, -position.Y - 64, 0);
        }
    }
}
