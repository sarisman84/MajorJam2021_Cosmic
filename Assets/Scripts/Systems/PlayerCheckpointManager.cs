using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Systems;

using Cinemachine;

using DG.Tweening;
using DG.Tweening.Core;

using MajorJam.System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Level {
    public class PlayerCheckpointManager : MonoBehaviour {
        public InputActionReference resetToLastCheckpoint;
        public List<Checkpoint> currentPlayerCheckpoints = new List<Checkpoint> ();
        public UnityEvent<Vector3> onPlayerResetToCheckpoint;
        [Header ("Transition Settings during checkpoint reset")]
        public Image transitionImage;
        public float transitionTime;

        private CheckpointInfo m_LatestPlayerInfo, m_LatestPlayerCameraInfo;
        private GameObject m_PlayerController;
        private Camera m_PlayerCamera;
        private Coroutine m_PlayerResetCoroutine;

        #region Singleton Implementation

        public static PlayerCheckpointManager CheckpointManager { get; private set; }

        #endregion

        private void Awake () {
            m_PlayerController = GameObject.FindGameObjectWithTag ("Player");
            m_PlayerCamera = Camera.main;
            currentPlayerCheckpoints.AddRange (FindObjectsOfType<Checkpoint> ());

            foreach (var checkpoint in currentPlayerCheckpoints) {
                checkpoint.ONTriggerEnter += obj => OnCheckpointTrigger (currentPlayerCheckpoints, checkpoint, obj);
            }

            if (!CheckpointManager)
                CheckpointManager = this;
            else
                Destroy (gameObject);

            if (transitionImage)
                transitionImage.SetAlpha (0);
            else
                Debug.LogError ($"Missing Transition Image in {gameObject.name}!");
        }

        #region Checkpoint implementation

        private void OnCheckpointTrigger (List<Checkpoint> playerCheckpoints, Checkpoint self, Collider other) {
            if (other.CompareTag ("Player") is { } player && !self.HasAlreadySaved) {
                m_LatestPlayerInfo = new CheckpointInfo (self.transform.position + Vector3.up, self.transform.rotation);
                Transform cam = m_PlayerCamera.transform;
                m_LatestPlayerCameraInfo = new CheckpointInfo (cam.position, cam.rotation);
                self.HasAlreadySaved = true;
                Debug.Log ("Checkpoint reached");

                //Reset other checkpoints so that backwards player progress save can be possible.
                foreach (var otherCheckpoint in playerCheckpoints) {
                    otherCheckpoint.HasAlreadySaved = false;
                }

            }
        }

        #endregion

        private void Update () {
            if (resetToLastCheckpoint.GetButtonDown ()) {
                TeleportPlayerToLatestCheckpoint ();
            }
        }

        public void TeleportPlayerToLatestCheckpoint () {

            if (m_PlayerResetCoroutine != null)
                StopCoroutine (m_PlayerResetCoroutine);

            m_PlayerResetCoroutine = StartCoroutine (DoTransition (transitionImage, transitionTime, () => {
                m_PlayerController.transform.position = m_LatestPlayerInfo.SavedPos;
                m_PlayerController.transform.rotation = m_LatestPlayerInfo.SavedRot;

                m_PlayerCamera.transform.position = m_LatestPlayerCameraInfo.SavedPos;
                m_PlayerCamera.transform.rotation = m_LatestPlayerCameraInfo.SavedRot;
            }));

        }

        private IEnumerator DoTransition (Image transition, float duration, Action duringTransitionEvent) {

            yield return DOTween.ToAlpha (() => transition.color, (x) => transition.color = x, 1f, duration / 2f).WaitForCompletion ();
            duringTransitionEvent?.Invoke ();
            yield return new WaitForSeconds (duration / 2f);
            yield return DOTween.ToAlpha (() => transition.color, (x) => transition.color = x, 0f, duration / 4f).WaitForCompletion ();
            onPlayerResetToCheckpoint?.Invoke (m_LatestPlayerInfo.SavedPos);
        }

        private void OnEnable () {
            InputManager.SetInputActive (true, resetToLastCheckpoint);
        }

        private void OnDisable () {
            InputManager.SetInputActive (false, resetToLastCheckpoint);
        }
    }

    public struct CheckpointInfo {
        public Vector3 SavedPos;
        public Quaternion SavedRot;

        public CheckpointInfo (Vector3 pos, Quaternion rot) {
            SavedPos = pos;
            SavedRot = rot;
        }
    }
}