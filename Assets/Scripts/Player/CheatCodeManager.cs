using System;
using System.Collections;
using System.Collections.Generic;
using MajorJam.System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private string currentInput;
    public InputActionReference cheatCodeToggle;

    public List<CheatCode> cheatCodeLibrary;
    public float inputCheckRate;
    public int cheatCodeCharLimit = 20;
    [Space] public UnityEvent onCheatInputActive, onCheatInputDisable, onSuccessfulCheatInput;

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
            m_AttemptACheatCode = !m_AttemptACheatCode;


        if (m_AttemptACheatCode)
        {
            if (m_CurInputState == 0)
            {
                onCheatInputActive?.Invoke();
                m_CurInputState = 1;
            }

            m_CurRate += Time.deltaTime;
            if (cheatCodeLibrary.Find(cc => cc.code.Equals(currentInput)) is { } cheat)
            {
                cheat.cheatCodeEvent?.Invoke();
                ResetTextInput();
                onSuccessfulCheatInput?.Invoke();
             
            }


            if (m_CurRate >= inputCheckRate)
            {
                ResetTextInput();
            }
        }
    }


    private void ResetTextInput()
    {
        m_CurRate = 0;
        currentInput = "";
        onCheatInputDisable?.Invoke();
        m_CurInputState = 0;
        m_AttemptACheatCode = false;
    }

    private void ConvertInputToText(char c)
    {
        if (m_AttemptACheatCode)
        {
            if (m_CurrentCheatCodeCharCount >= cheatCodeCharLimit)
            {
                m_AttemptACheatCode = false;
            }

            currentInput += c;
            m_CurRate = 0;

            m_CurrentCheatCodeCharCount++;
        }
    }
}


[Serializable]
public class CheatCode
{
    public string code;
    public UnityEvent cheatCodeEvent;
}