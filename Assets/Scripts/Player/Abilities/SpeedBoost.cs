using UnityEngine;

namespace MajorJam.System
{
    [CreateAssetMenu(menuName = "Cheats/Speed Mod", fileName = "New Speed Mod", order = 0)]
    public class SpeedBoost : Ability
    {
        public float moddedMaxMovementSpeed, moddedAcceleration;

        public override void Use(PlayerController player)
        {
            player.CustomAcceleration = Keybind.GetButton() ? moddedAcceleration : 0;
            player.CustomMaxMovementSpeed = Keybind.GetButton() ? moddedMaxMovementSpeed : 0;
        }
    }
}