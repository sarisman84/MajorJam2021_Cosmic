using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MajorJam.System
{
    public class InputManager
    {
        public static void SetInputActive(bool value, params InputActionReference[] references)
        {
            foreach (var reference in references)
            {
                if (reference)
                {
                    if (value)
                        reference.action.Enable();
                    else
                        reference.action.Disable();
                }
                else
                {
                    Debug.LogWarning("Couldn't find input reference! Skipping.");
                }
            }
        }
    }


    public static class InputExtension
    {
        public enum Axis
        {
            Horizontal,
            Vertical,
            Both
        }

        public static Vector2 GetAxisRaw(this InputActionReference reference, Axis inputAxis = Axis.Both)
        {
            InputAction correspondingAction =
                (reference.action.name.ToLower().Contains("movement")) ? reference.action : null;


            if (correspondingAction != null)
                switch (inputAxis)
                {
                    case Axis.Horizontal:
                        return new Vector2(correspondingAction.ReadValue<Vector2>().x, 0);
                    case Axis.Vertical:
                        return new Vector2(0, correspondingAction.ReadValue<Vector2>().y);
                    case Axis.Both:
                        return correspondingAction.ReadValue<Vector2>();
                }

            return Vector2.zero;
        }


        public static bool GetButton(this InputActionReference reference)
        {
            InputAction correspondingAction = GetButtonWrap(reference);
            if (correspondingAction != null)
            {
                return correspondingAction.ReadValue<float>() > 0;
            }

            return false;
        }

        public static bool GetButtonDown(this InputActionReference reference)
        {
            InputAction correspondingAction = GetButtonWrap(reference);
            if (correspondingAction != null)
            {
                return correspondingAction.ReadValue<float>() > 0 && correspondingAction.triggered;
            }

            return false;
        }

        private static bool Contains(this string text, params string[] elements)
        {
            foreach (var element in elements)
            {
                if (text.Contains(element))
                    return true;
            }

            return false;
        }

        private static InputAction GetButtonWrap(this InputActionReference reference)
        {
            return (reference.action.name.ToLower().Contains("button", "jump")) ? reference.action : null;
        }


        public static Vector3 ToVector3XZ(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
    }
}