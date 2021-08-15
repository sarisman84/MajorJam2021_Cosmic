using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class SceneManager : MonoBehaviour
{
    private Camera m_Cam;
    private Color m_DefaultColor;

    public UnityEvent onLevelLoad, onLevelUnload;


    private void Awake()
    {
        m_Cam = Camera.main;
        m_Cam.clearFlags = CameraClearFlags.SolidColor;
        m_DefaultColor = m_Cam.backgroundColor;
        m_Cam.backgroundColor = Color.gray;
    }

    public void LoadLevel()
    {
        m_Cam.clearFlags = CameraClearFlags.Skybox;
        m_Cam.backgroundColor = m_DefaultColor;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        onLevelLoad?.Invoke();
    }

    public void UnloadLevel()
    {
        m_Cam.clearFlags = CameraClearFlags.SolidColor;
        m_Cam.backgroundColor = Color.gray;
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
            UnityEngine.SceneManagement.SceneManager.GetSceneAt(1));
        onLevelUnload?.Invoke();
    }
}