using System;
using System.IO;
using UnityEngine;

namespace Uuvr.VrUi
{
    /// <summary>
    /// Renders a custom mouse cursor for the VR UI, ensuring visibility on the VR UI plane.
    /// </summary>
    public class VrUiCursor : UuvrBehaviour
    {
        private Texture2D? _texture;
        private readonly Vector2 _offset = new(22, 2);

        public VrUiCursor(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Unity's Start method. Initializes the custom cursor texture.
        /// </summary>
        private void Start()
        {
            // Load cursor bitmap
            var cursorPath = Path.Combine(UuvrPlugin.ModFolderPath, @"Assets\cursor.bmp");
            if (!File.Exists(cursorPath))
            {
                Debug.LogError($"Cursor file not found: {cursorPath}");
                return;
            }

            var bytes = File.ReadAllBytes(cursorPath);

            // Read dimensions from BMP header
            var width = bytes[18] + (bytes[19] << 8);
            var height = bytes[22] + (bytes[23] << 8);

            if (width <= 0 || height <= 0)
            {
                Debug.LogError("Invalid cursor dimensions.");
                return;
            }

            // Create the texture and populate it with pixel data
            _texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
            var colors = new Color32[width * height];

            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(
                    bytes[i * 4 + 54],
                    bytes[i * 4 + 55],
                    bytes[i * 4 + 56],
                    bytes[i * 4 + 57]
                );
            }

            _texture.SetPixels32(colors);
            _texture.Apply();

            Debug.Log("Custom cursor texture initialized.");
        }

        /// <summary>
        /// Unity's Update method. Ensures the custom cursor is applied every frame.
        /// </summary>
        private void Update()
        {
            if (_texture == null) return;

            // Set the custom cursor texture
            Cursor.SetCursor(_texture, _offset, CursorMode.ForceSoftware);
        }
    }
}
