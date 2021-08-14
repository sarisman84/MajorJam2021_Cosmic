using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MajorJam.System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Level
{
    public class PlayerCheckpointManager : MonoBehaviour
    {
        public InputActionReference resetToLastCheckpoint;
        public List<Checkpoint> currentPlayerCheckpoints = new List<Checkpoint>();
        public UnityEvent<Vector3> onPlayerResetToCheckpoint;

        private CheckpointInfo m_LatestPlayerInfo, m_LatestPlayerCameraInfo;
        private GameObject m_PlayerController;
        private Camera m_PlayerCamera;


        #region Singleton Implementation

        public static PlayerCheckpointManager CheckpointManager { get; private set; }

        #endregion


        private void Awake()
        {
            m_PlayerController = GameObject.FindGameObjectWithTag("Player");
            m_PlayerCamera = Camera.main;
            currentPlayerCheckpoints.AddRange(FindObjectsOfType<Checkpoint>());

            foreach (var checkpoint in currentPlayerCheckpoints)
            {
                checkpoint.ONTriggerEnter += obj => OnCheckpointTrigger(currentPlayerCheckpoints,checkpoint, obj);
            }

            if (!CheckpointManager)
                CheckpointManager = this;
            else
                Destroy(gameObject);
        }

        #region Checkpoint implementation

        private void OnCheckpointTrigger(List<Checkpoint> playerCheckpoints, Checkpoint self, Collider other)
        {
            if (other.CompareTag("Player") && !self.HasAlreadySaved)
            {
                m_LatestPlayerInfo = new CheckpointInfo(self.transform.position, self.transform.rotation);
                Transform cam = m_PlayerCamera.transform;
                m_LatestPlayerCameraInfo = new CheckpointInfo(cam.position, cam.rotation);
                self.HasAlreadySaved = true;
                Debug.Log("Checkpoint reached");
                
                //Reset other checkpoints so that backwards player progress save can be possible.
                foreach (var otherCheckpoint in playerCheckpoints)
                {
                    otherCheckpoint.HasAlreadySaved = false;
                }
            }
        }

        #endregion


        private void Update()
        {
            if (resetToLastCheckpoint.GetButtonDown())
            {
                TeleportPlayerToLatestCheckpoint();
            }
        }

        public void TeleportPlayerToLatestCheckpoint()
        {
            m_PlayerController.transform.position = m_LatestPlayerInfo.SavedPos;
            m_PlayerController.transform.rotation = m_LatestPlayerInfo.SavedRot;

            m_PlayerCamera.transform.position = m_LatestPlayerCameraInfo.SavedPos;
            m_PlayerCamera.transform.rotation = m_LatestPlayerCameraInfo.SavedRot;


            onPlayerResetToCheckpoint?.Invoke(m_LatestPlayerInfo.SavedPos);
        }


        private void OnEnable()
        {
            InputManager.SetInputActive(true, resetToLastCheckpoint);
        }

        private void OnDisable()
        {
            InputManager.SetInputActive(false, resetToLastCheckpoint);
        }
    }

    public struct CheckpointInfo
    {
        public Vector3 SavedPos;
        public Quaternion SavedRot;

        public CheckpointInfo(Vector3 pos, Quaternion rot)
        {
            SavedPos = pos;
            SavedRot = rot;
        }
    }
}