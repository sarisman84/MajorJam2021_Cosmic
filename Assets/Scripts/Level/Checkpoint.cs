using System;
using UnityEngine;

namespace Level
{
    public class Checkpoint : MonoBehaviour
    {
        public event Action<Collider> ONTriggerEnter;
        public bool HasAlreadySaved { get; set; }
        public bool gizmoPreview;

        private void OnTriggerEnter(Collider other)
        {
            ONTriggerEnter?.Invoke(other);
        }


        private BoxCollider Collider;

        private void OnDrawGizmos()
        {
            Collider = Collider ? Collider : GetComponent<BoxCollider>();

            if (Collider && gizmoPreview)
            {
                Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position + Collider.center, transform.rotation, Collider.size);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
    }
}