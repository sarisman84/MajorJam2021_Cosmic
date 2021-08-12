using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MajorJam.System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private Vector3 m_OriginalSize;

    public float pressurePlateHeightWhilePressed = 0.5f;
    public float pressurePlatePressingSpeed = 1f;
    public float pressurePlateReleaseSpeed = 1f;


    public void OnPressurePlatePressed()
    {
        m_OriginalSize = transform.localScale;
        transform.DOScale(new Vector3(m_OriginalSize.x, pressurePlateHeightWhilePressed, m_OriginalSize.z),
            pressurePlatePressingSpeed);
        AudioManager.Manager.Play("PP_Pressed");
    }


    public void OnPressurePlateRelease()
    {
        transform.DOScale(m_OriginalSize, pressurePlateReleaseSpeed);
        AudioManager.Manager.Play("PP_Released");
    }
}