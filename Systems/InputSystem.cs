using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Gala.Systems
{
    internal class InputSystem
    {
        private string keybindsFilePath;

        public Dictionary<string, Keys> Keybinds { get; private set; }
        
        public InputSystem(string filePath, int arraySize=32)
        {
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] paths = { exePath, filePath };
            keybindsFilePath = System.IO.Path.Combine(paths);
            LoadKeybinds();
        }

        private void LoadKeybinds()
        {
            try
            {
                // Access the keybind file
                string json = System.IO.File.ReadAllText(keybindsFilePath);
                Keybinds = JsonSerializer.Deserialize<Dictionary<string, Keys>>(json);
            }
            catch
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

        public void SaveKeybinds()
        {
            // Create the file & directory if they don't exist. Does nothing otherwise.
            System.IO.FileInfo file = new System.IO.FileInfo(keybindsFilePath);
            file.Directory.Create();

            // Write to the file, overwrites completely
            string json = JsonSerializer.Serialize(Keybinds, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(keybindsFilePath, json);
        }

        public Keys GetKeybind(string action)
        {
            if (Keybinds.ContainsKey(action))
            {
                return Keybinds[action];
            }
            else
            {
                return Keys.None;
            }
        }

        public Vector2 GetMovement()
        {
            KeyboardState currentKeystate = Keyboard.GetState();
            Vector2 moveDirection = Vector2.Zero;
            
            if (currentKeystate.IsKeyDown(GetKeybind("MoveLeft"))) {
                moveDirection.X -= 1;
            }

            if (currentKeystate.IsKeyDown(GetKeybind("MoveRight"))) {
                moveDirection.X += 1;
            }

            if (currentKeystate.IsKeyDown(GetKeybind("MoveUp"))) {
                moveDirection.Y -= 1;
            }

            if (currentKeystate.IsKeyDown(GetKeybind("MoveDown"))) {
                moveDirection.Y += 1;
            }

            if (moveDirection.Length() > 1)
            {
                moveDirection.Normalize();
            }
            return moveDirection;
        }
    }
}
