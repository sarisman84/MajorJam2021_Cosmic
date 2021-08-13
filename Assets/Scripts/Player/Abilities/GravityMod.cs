using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MajorJam.System
{
    public class GravityMod : Ability
    {
        public enum GravityDirection
        {
            Down,
            Up
        }

        public GravityDirection gravityDirection;

        protected override void OnAbilityUse(PlayerController playerController)
        {
            if (isUsable)
            {
                Physics.gravity = new Vector3(Physics.gravity.x,
                    gravityDirection == GravityDirection.Up
                        ? -Mathf.Abs(Physics.gravity.y)
                        : Mathf.Abs(Physics.gravity.y), Physics.gravity.z);
                isUsable = false;

                gravityDirection = gravityDirection == GravityDirection.Up
                    ? GravityDirection.Down
                    : GravityDirection.Up;
            }
        }
    }
}