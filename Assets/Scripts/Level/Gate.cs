using System;
using DG.Tweening;
using MajorJam.System;
using UnityEngine;

namespace Level
{
    public class Gate : MonoBehaviour
    {
        public Vector3 openGatePosition = Vector3.down;
        public float gateOpeningSpeed = 1;
        private Vector3 LocalizedOpenPosition => m_StartingPosition + openGatePosition;

        private Vector3 m_StartingPosition;

        void Awake()
        {
            m_StartingPosition = transform.position;
        }

        public void Open()
        {
            AudioManager.Manager.Play("Gate_Open");
            transform.DOMove(LocalizedOpenPosition, gateOpeningSpeed);
        }

        public void Close()
        {
            AudioManager.Manager.Play("Gate_Closed");
            transform.DOMove(m_StartingPosition, gateOpeningSpeed);
        }


        private void OnDrawGizmos()
        {
            m_StartingPosition = !Application.isPlaying ? transform.position : m_StartingPosition;

            Gizmos.matrix = Matrix4x4.TRS(LocalizedOpenPosition, transform.rotation, transform.localScale);
            Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawCube(Vector3.one, Vector3.zero);
        }
    }
}