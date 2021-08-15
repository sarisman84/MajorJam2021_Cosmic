using System;
using System.Collections;
using System.Collections.Generic;
using MajorJam.System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System.Text.RegularExpressions;
using Systems;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private string currentInput;
    public InputActionReference cheatCodeToggle;

    public List<CheatCode> cheatCodeLibrary;
    public float inputCheckRate;
    public int cheatCodeCharLimit = 20;
    [Space] public UnityEvent onCheatInputActive, onCheatInputDisable, onSuccessfulCheatInput, onTypingWhileCheating;

    private float m_CurRate = 0;
    private bool m_AttemptACheatCode;
    private int m_CurrentCheatCodeCharCount = 0;
    private int m_CurInputState = 0;


    private void OnEnable()
    {
        InputManager.SetInputActive(true, cheatCodeToggle);
    }

    private void OnDisable()
    {
        InputManager.SetInputActive(false, cheatCodeToggle);
    }

    private void Awake()
    {
        Keyboard.current.onTextInput += ConvertInputToText;
    }

    private void Update()
    {
        if (cheatCodeToggle.GetButtonDown())
            m_AttemptACheatCode = !m_AttemptACheatCode && UIManager.Get.IsInGame;
        if (!m_AttemptACheatCode && m_CurInputState == 1)
        {
            onCheatInputDisable?.Invoke();
            ResetTextInput();
        }


        if (m_AttemptACheatCode)
        {
            if (m_CurInputState == 0)
            {
                onCheatInputActive?.Invoke();
                m_CurInputState = 1;
            }


            if (cheatCodeLibrary.Find(cc => currentInput.Contains(cc.code)) is { } cheat)
            {
                cheat.cheatCodeEvent?.Invoke();
                onSuccessfulCheatInput?.Invoke();
                onCheatInputDisable?.Invoke();
                ResetTextInput();
                m_AttemptACheatCode = false;
            }
        }
    }


    private void ResetTextInput()
    {
        currentInput = "";
        m_CurInputState = 0;
    }

    private void ConvertInputToText(char c)
    {
        if (m_AttemptACheatCode)
        {
            currentInput += c;
            onTypingWhileCheating?.Invoke();
        }
    }
}


[Serializable]
public class CheatCode
{
    public string code;
    public UnityEvent cheatCodeEvent;
}