using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MajorJam.System
{
    [CreateAssetMenu(menuName = "Cheats/Player Stat Mod", fileName = "New Stat Mod", order = 0)]
    public class StatModifier : Ability
    {
        [Header("Movement Mods")] public float maxMovementSpeed;
        public float acceleration;
        [Header("Jump Mod")] public float jumpHeight;
        public float lowJumpHeight, fallMultiplier;


        protected override void OnAbilityUse(PlayerController playerController)
        {
            SetStatsToPlayer(playerController, !AbilityInUse);
        }

        private void SetStatsToPlayer(PlayerController playerController, bool reset = false)
        {
            playerController.CustomFallMultiplier = reset ? playerController.CustomFallMultiplier == fallMultiplier ? 0 : playerController.CustomFallMultiplier : fallMultiplier;
            playerController.CustomLowJumpHeight = reset ? playerController.CustomLowJumpHeight == lowJumpHeight ? 0 : playerController.CustomLowJumpHeight : lowJumpHeight;
            playerController.CustomJumpHeight = reset ? playerController.CustomJumpHeight == jumpHeight ? 0 : playerController.CustomJumpHeight : jumpHeight;

            playerController.CustomAcceleration = reset ? playerController.CustomAcceleration == acceleration ? 0 : playerController.CustomAcceleration : acceleration;
            playerController.CustomMaxMovementSpeed = reset ? playerController.CustomMaxMovementSpeed == maxMovementSpeed ? 0 : playerController.CustomMaxMovementSpeed : maxMovementSpeed;
        }
    }
}