using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text.Json;

namespace Gala
{
    // InputState holds all the various types of input together
    // For now, this is just mouse and keyboard
    public class InputState
    {
        // Keybinds info
        // This likely would make more sense if moved out of this class into an input handling system
        public Dictionary<string, Keys> Keybinds { get; private set; }
        private string keybindsFilePath;

        // Mouse
        public MouseState CurrentMouseState { get; private set; }
        public MouseState PreviousMouseState { get; private set; }

        // Keyboard
        public KeyboardState CurrentKeyboardState { get; private set; }
        public KeyboardState PreviousKeyboardState { get; private set; }

        // There is probably a better way to store these types of joint inputs
        public Vector2 Movement { get; private set; }

        public InputState(string filePath)
        {
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] paths = { exePath, filePath };
            keybindsFilePath = System.IO.Path.Combine(paths);
            loadKeybinds();


            CurrentMouseState = new MouseState();
            PreviousMouseState = CurrentMouseState;

            CurrentKeyboardState = new KeyboardState();
            PreviousKeyboardState = CurrentKeyboardState;

            Movement = Vector2.Zero;
        }

        #region Public Methods
        public void GetCurrentInputState()
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            Movement = calculateMovementVector();
        }

        public Keys GetKeybind(string action)
        {
            if (Keybinds.ContainsKey(action))
            {
                return Keybinds[action];
            } else
            {
                return Keys.None;
            }
        }

        public void SaveKeybinds()
        {
            // Create the file & directory if they don't exist. Does nothing otherwise.
            System.IO.FileInfo file = new System.IO.FileInfo(keybindsFilePath);
            file.Directory.Create();

            // Write to the file, overwrites completely
            string json = JsonSerializer.Serialize(Keybinds, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(keybindsFilePath, json);
        }
        #endregion

        #region Private Methods
        private Vector2 calculateMovementVector()
        {
            Vector2 moveDirection = Vector2.Zero;

            if (CurrentKeyboardState.IsKeyDown(GetKeybind("MoveLeft")))
            {
                moveDirection.X -= 1;
            }

            if (CurrentKeyboardState.IsKeyDown(GetKeybind("MoveRight")))
            {
                moveDirection.X += 1;
            }

            if (CurrentKeyboardState.IsKeyDown(GetKeybind("MoveUp")))
            {
                moveDirection.Y -= 1;
            }

            if (CurrentKeyboardState.IsKeyDown(GetKeybind("MoveDown")))
            {
                moveDirection.Y += 1;
            }

            if (moveDirection.Length() > 1)
            {
                moveDirection.Normalize();
            }

            return moveDirection;
        }

        private void loadKeybinds()
        {
            try
            {
                // Access the keybind file
                string json = System.IO.File.ReadAllText(keybindsFilePath);
                Keybinds = JsonSerializer.Deserialize<Dictionary<string, Keys>>(json);
            } catch
            {
                // File read error, most likely. Set defaults, then save
                Keybinds = new Dictionary<string, Keys>
                {
                    {"MoveUp", Keys.W},
                    {"MoveLeft", Keys.A},
                    {"MoveDown", Keys.S},
                    {"MoveRight", Keys.D}
                };

                SaveKeybinds();
            }
        }
        #endregion
    }
}
