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
            playerController.CustomFallMultiplier = reset ? 0 : fallMultiplier;
            playerController.CustomLowJumpHeight = reset ? 0 : lowJumpHeight;
            playerController.CustomJumpHeight = reset ? 0 : jumpHeight;

            playerController.CustomAcceleration = reset ? 0 : acceleration;
            playerController.CustomMaxMovementSpeed = reset ? 0 : maxMovementSpeed;
        }
    }
}