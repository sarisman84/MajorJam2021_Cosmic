using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MajorJam.System
{
    [CreateAssetMenu(fileName = "New Teleport Setting", menuName = "Cheats/Teleport to a Position", order = 0)]
    public class Teleport : Ability
    {
        [Header("Teleport Settings")]
        public string targetPositionTag;
        public Vector3 teleportPositionOffset;

        protected override void OnAbilityUse(PlayerController playerController)
        {
            if (CanUseAbility)
            {
                playerController.transform.position = GameObject.FindGameObjectWithTag(targetPositionTag).transform.position + teleportPositionOffset;
                CanUseAbility = false;
            }
           
        }
    }
}