using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
  
    public abstract class Ability : ScriptableObject
    {
        public enum AbilityType
        {
            Speedboost,
            Noclip,
            Jumpboost,
            Grabbleboost
        }

        public string AbilityID;
        public InputActionReference Keybind;
        public bool IsUseable;


        public abstract void Use(PlayerController playerController);
    }
}