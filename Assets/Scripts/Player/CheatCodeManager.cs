using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private string currentInput;

    public List<CheatCode> cheatCodeLibrary;
    public float inputCheckRate;

    private float m_CurRate = 0;
    public bool attemptACheatCode;

    private void Awake()
    {
        Keyboard.current.onTextInput += ConvertInputToText;
    }

    private void Update()
    {
        if (attemptACheatCode)
        {
            m_CurRate += Time.deltaTime;
            if (m_CurRate >= inputCheckRate)
            {
                if (cheatCodeLibrary.Find(cc => cc.code.Contains(currentInput)) is { } cheat)
                {
                    cheat.cheatCodeEvent?.Invoke();
                }

                currentInput = "";
                m_CurRate = 0;
            }
        }
    }

    private void ConvertInputToText(char c)
    {
        if (attemptACheatCode)
        {
            currentInput += c;
            m_CurRate = 0;
        }
    }
}


[Serializable]
public class CheatCode
{
    public string code;
    public UnityEvent cheatCodeEvent;
}