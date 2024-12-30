using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Uuvr
{
    public class UuvrInput : UuvrBehaviour
    {
        private enum XboxButton
        {
            DpadUp = 0x0001,
            DpadDown = 0x0002,
            DpadLeft = 0x0004,
            DpadRight = 0x0008,
            Start = 0x0010,
            Back = 0x0020,
            LeftThumb = 0x0040,
            RightThumb = 0x0080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000,
        }

        // Native DLL calls for XInput emulation
        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetButtonState")]
        private static extern void XInputSetButtonState(ushort button, bool pressed);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetTriggerState")]
        private static extern void XInputSetTriggerState(bool isLeft, byte value);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetThumbState")]
        private static extern void XInputSetThumbState(bool isLeft, short x, short y);

        /// <summary>
        /// Unity's Awake method. Logs initialization.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("UUVR Input initialized.");
        }

        /// <summary>
        /// Unity's Update method. Handles input emulation.
        /// </summary>
        private void Update()
        {
            EmulateInput();
        }

        /// <summary>
        /// Emulates Xbox controller input based on Unity's Input system.
        /// </summary>
        private static void EmulateInput()
        {
            // Map buttons to Unity Input keys
            SetButtonState(XboxButton.A, Input.GetKey(KeyCode.Space)); // A -> Space
            SetButtonState(XboxButton.B, Input.GetKey(KeyCode.B)); // B -> B key
            SetButtonState(XboxButton.Start, Input.GetKey(KeyCode.Return)); // Start -> Enter
            SetButtonState(XboxButton.Back, Input.GetKey(KeyCode.Escape)); // Back -> Escape

            // Emulate triggers
            XInputSetTriggerState(true, (byte)(Mathf.Clamp01(Input.GetAxis("Fire1")) * 255)); // Left trigger
            XInputSetTriggerState(false, (byte)(Mathf.Clamp01(Input.GetAxis("Fire2")) * 255)); // Right trigger

            // Emulate thumbsticks
            XInputSetThumbState(true,
                (short)(Input.GetAxis("Horizontal") * short.MaxValue),
                (short)(Input.GetAxis("Vertical") * short.MaxValue)); // Left thumbstick

            XInputSetThumbState(false,
                (short)(Input.GetAxis("Mouse X") * short.MaxValue),
                (short)(Input.GetAxis("Mouse Y") * short.MaxValue)); // Right thumbstick
        }

        /// <summary>
        /// Sets the button state for a virtual Xbox controller.
        /// </summary>
        /// <param name="button">The button to set.</param>
        /// <param name="pressed">Whether the button is pressed.</param>
        private static void SetButtonState(XboxButton button, bool pressed)
        {
            XInputSetButtonState((ushort)button, pressed);
        }
    }
}
