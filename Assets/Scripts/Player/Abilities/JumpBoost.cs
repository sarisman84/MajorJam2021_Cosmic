using UnityEngine;

namespace MajorJam.System
{
    [CreateAssetMenu(fileName = "New Jump Mod", menuName = "Cheats/Jump Mod", order = 0)]
    public class JumpBoost : Ability
    {
        public float moddedJumpHeight;
        public float moddedLowJumpHeight;
        public float moddedFallMultiplier;
        public override void Use(PlayerController playerController)
        {
            playerController.CustomJumpHeight = Keybind.GetButton() ? moddedJumpHeight : 0;
            playerController.CustomLowJumpHeight = Keybind.GetButton() ? moddedLowJumpHeight : 0;
            playerController.CustomFallMultiplier = Keybind.GetButton() ? moddedFallMultiplier : 0;
        }
    }
}