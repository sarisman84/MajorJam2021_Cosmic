using System;
using System.Collections;
using System.Collections.Generic;
using MajorJam.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class UIManager : MonoBehaviour
    {
        public bool IsInGame { get; set; }
        public static UIManager Get { get; private set; }

        public InputActionReference puaseKeybind;
        [Space] public Canvas mainMenu;
        public Canvas pauseMenu, settingsMenu;
        public event Action<bool> ONPauseEvent;

        private bool latestMainMenuState, latestPauseMenuState;

        private void Awake()
        {
            if (Get != null)
            {
                Destroy(gameObject);
                return;
            }

            Get = this;
        }

        private void OnEnable()
        {
            InputManager.SetInputActive(true, puaseKeybind);
        }

        private void OnDisable()
        {
            InputManager.SetInputActive(false, puaseKeybind);
        }


        private void Update()
        {
            if (puaseKeybind.GetButtonDown())
            {
                if (IsInGame)
                {
                    if (settingsMenu.gameObject.activeSelf)
                    {
                        settingsMenu.gameObject.SetActive(false);
                        pauseMenu.gameObject.SetActive(true);
                        return;
                    }
                    
                    GameObject o;
                    (o = pauseMenu.gameObject).SetActive(!pauseMenu.gameObject.activeSelf);
                    Cursor.visible = o.activeSelf;
                    Cursor.lockState = o.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
                    ONPauseEvent?.Invoke(o.activeSelf);
                }
            }
        }


        public void ToMenu()
        {
            mainMenu.gameObject.SetActive(latestMainMenuState);
            pauseMenu.gameObject.SetActive(latestPauseMenuState);
            settingsMenu.gameObject.SetActive(false);
        }

        public void ToSettingsMenu()
        {
            latestMainMenuState = mainMenu.gameObject.activeSelf;
            latestPauseMenuState = pauseMenu.gameObject.activeSelf;

            mainMenu.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(true);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.ExitPlaymode();
                return;
            }
#endif
            Application.Quit();
        }
    }
}