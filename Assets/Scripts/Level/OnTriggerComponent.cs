using System;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [RequireComponent(typeof(BoxCollider))]
    public class OnTriggerComponent : MonoBehaviour
    {
        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        public bool showCollisionGizmo;
        private Collider m_Col;


        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }


        private void OnDrawGizmos()
        {
       
            if (showCollisionGizmo)
            {
                m_Col = m_Col ? m_Col : GetComponent<Collider>();
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, m_Col.bounds.size);
                Gizmos.color = Color.magenta - new Color(0, 0, 0, 0.75f);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}