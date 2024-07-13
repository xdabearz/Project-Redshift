using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Redshift.Systems
{
    internal class InputCaptureSystem : ECSSystem
    {
        public InputCaptureSystem(World world) : base(world) { }

        public override void Update(GameTime gameTime)
        {
            List<Entity> entities = World.EntityManager.GetEntitiesByFlag(ComponentFlag.InputState);

            foreach (Entity entity in entities)
            {
                var input = World.EntityManager.GetComponent<InputState>(entity);

                input.PreviousMouseState = input.CurrentMouseState;
                input.CurrentMouseState = Mouse.GetState();

                input.PreviousKeyboardState = input.CurrentKeyboardState;
                input.CurrentKeyboardState = Keyboard.GetState();

                input.Movement = calculateMovementVector(input);

                if (input.Movement.Length() > 0f)
                {
                    if (entity.ActiveComponents.HasFlag(ComponentFlag.CommandComponent))
                    {
                        var movementComponent = World.EntityManager.GetComponent<MovementComponent>(entity);
                        movementComponent.Velocity = input.Movement;
                    }
                    else
                    {
                        World.EntityManager.AddComponent(entity, new MovementComponent
                        {
                            Velocity = input.Movement
                        });
                    }
                }

                if (input.CurrentKeyboardState.IsKeyDown(getKeybind(input, "Shoot")))
                {
                    if (entity.ActiveComponents.HasFlag(ComponentFlag.CommandComponent))
                    {
                        var commandComponent = World.EntityManager.GetComponent<CommandComponent>(entity);
                        commandComponent.Type = CommandType.Shoot;
                    }
                    else
                    {
                        World.EntityManager.AddComponent(entity, new CommandComponent
                        {
                            Type = CommandType.Shoot
                        });
                    }
                }
            }
        }

        #region Private Methods
        private Vector2 calculateMovementVector(InputState input)
        {
            Vector2 moveDirection = Vector2.Zero;

            if (input.CurrentKeyboardState.IsKeyDown(getKeybind(input, "MoveLeft")))
            {
                moveDirection.X -= 1;
            }

            if (input.CurrentKeyboardState.IsKeyDown(getKeybind(input, "MoveRight")))
            {
                moveDirection.X += 1;
            }

            if (input.CurrentKeyboardState.IsKeyDown(getKeybind(input, "MoveUp")))
            {
                moveDirection.Y -= 1;
            }

            if (input.CurrentKeyboardState.IsKeyDown(getKeybind(input, "MoveDown")))
            {
                moveDirection.Y += 1;
            }

            if (moveDirection.Length() > 1)
            {
                moveDirection.Normalize();
            }

            return moveDirection;
        }

        public Keys getKeybind(InputState input, string action)
        {
            if (input.Keybinds.ContainsKey(action))
            {
                return input.Keybinds[action];
            }
            else
            {
                return Keys.None;
            }
        }
        #endregion
    }

}