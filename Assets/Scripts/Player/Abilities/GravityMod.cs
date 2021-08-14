using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MajorJam.System
{
    [CreateAssetMenu(menuName = "Cheats/Gravity Mod", fileName = "New Gravity Modifier", order = 0)]
    public class GravityMod : Ability
    {
        private bool m_ReverseGravity = false;

        private void OnEnable()
        {
            m_ReverseGravity = false;
        }

        private void OnDisable()
        {
            m_ReverseGravity = false;
        }

        protected override void OnAbilityUse(PlayerController playerController)
        {
            if (isUsable)
            {
                m_ReverseGravity = !m_ReverseGravity;
                playerController.IsGravityReversed = m_ReverseGravity;
                Transform worldPivot = GameObject.FindGameObjectWithTag("World Pivot").transform;

                worldPivot.transform.rotation = m_ReverseGravity
                    ? Quaternion.Euler(0, 0, 180)
                    : Quaternion.identity;

                Transform model = GameObject.FindGameObjectWithTag("Player/Model").transform;
                model.transform.localRotation = worldPivot.transform.rotation;
                model.transform.localScale = m_ReverseGravity ? new Vector3(-1, 1, 1) : Vector3.one;
                model.transform.localPosition = m_ReverseGravity ? Vector3.up : Vector3.down;

                Physics.gravity = new Vector3(Physics.gravity.x,
                    !m_ReverseGravity
                        ? -Mathf.Abs(Physics.gravity.y)
                        : Mathf.Abs(Physics.gravity.y), Physics.gravity.z);

                Debug.Log(Physics.gravity);

                var velocity = playerController.physics.velocity;
                velocity = new Vector3(velocity.x, 0,
                    velocity.z);
                playerController.physics.velocity = velocity;


                isUsable = false;
            }
        }
    }
}