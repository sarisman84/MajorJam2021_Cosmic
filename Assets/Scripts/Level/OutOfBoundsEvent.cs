using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OutOfBoundsEvent : MonoBehaviour
{
    public void ResetPlayerPosition()
    {
        if (PlayerCheckpointManager.CheckpointManager is { } manager)
        {
            manager.TeleportPlayerToLatestCheckpoint();
        }
    }
}