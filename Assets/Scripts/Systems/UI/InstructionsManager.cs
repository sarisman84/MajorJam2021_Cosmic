using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    public TMP_Text[] textField;

    public void UpdateInstructionText(string value)
    {
        foreach (var field in textField)
        {
            field.text += $"\n{value}";
        }
        
    }
}