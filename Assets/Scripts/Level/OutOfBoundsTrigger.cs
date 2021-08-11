using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;

 [RequireComponent(typeof(BoxCollider))]
public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerCheckpointManager.CheckpointManager is { } manager)
        {
            manager.TeleportPlayerToLatestCheckpoint();
        }
    }
}
