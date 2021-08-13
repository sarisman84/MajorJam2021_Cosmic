using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MajorJam.System
{
    [CreateAssetMenu(menuName = "Cheats/Gravity Mod", fileName = "New Gravity Modifier", order = 0)]
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
                Transform worldPivot = GameObject.FindGameObjectWithTag("World Pivot").transform;

                worldPivot.transform.rotation = gravityDirection == GravityDirection.Up
                    ? Quaternion.Euler(0, 0, 180)
                    : Quaternion.identity;

                playerController.transform.parent.localScale =
                    gravityDirection == GravityDirection.Up ? new Vector3(1, -1, 1) : Vector3.one;

                Physics.gravity = new Vector3(Physics.gravity.x,
                    gravityDirection == GravityDirection.Up
                        ? Mathf.Abs(Physics.gravity.y)
                        : -Mathf.Abs(Physics.gravity.y), Physics.gravity.z);
             

                gravityDirection = gravityDirection == GravityDirection.Up
                    ? GravityDirection.Down
                    : GravityDirection.Up;
                
                isUsable = false;
            }
        }
    }
}