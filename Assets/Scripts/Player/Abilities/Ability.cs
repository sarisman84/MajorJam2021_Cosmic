using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MajorJam.System
{
    public abstract class Ability : ScriptableObject
    {
        public enum KeybindType
        {
            Toggle,
            Hold,
            Press
        }

        [Header("General Settings")] public string abilityID;
        public bool isUsable;
        [Space] public InputActionReference keybind;
        public KeybindType keybindingType;
        [Header("Keybind Settings: Press")] public float abilityDuration;


        private bool m_AbilityInUse;

        protected bool AbilityInUse => m_AbilityInUse;
        public bool CanUseAbility { get; set; }

        protected virtual void OnEnable()
        {
            m_AbilityInUse = false;
            CanUseAbility = false;
        }

        public void Use(PlayerController playerController)
        {
            if (keybind)
            {
                switch (keybindingType)
                {
                    case KeybindType.Toggle:
                        if (keybind.GetButtonDown())
                            m_AbilityInUse = !m_AbilityInUse;
                        break;
                    case KeybindType.Hold:
                        m_AbilityInUse = keybind.GetButton();
                        break;
                    case KeybindType.Press:
                        if (keybind.GetButtonDown() && !m_AbilityInUse)
                            playerController.StartCoroutine(ResetInput());
                        break;
                }
            }

            OnAbilityUse(playerController);
        }

        private IEnumerator ResetInput()
        {
            m_AbilityInUse = true;
            yield return new WaitForSeconds(abilityDuration);
            m_AbilityInUse = false;
        }


        protected abstract void OnAbilityUse(PlayerController playerController);
    }
}