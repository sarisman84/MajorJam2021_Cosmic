using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MajorJam.System
{
    [RequireComponent(typeof(PlayerController))]
    public class AbilityManager : MonoBehaviour
    {
        public List<Ability> currentAbilities;
        private PlayerController _playerController;


        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }


        private void Update()
        {
            foreach (var ability in currentAbilities)
            {
                if (ability.IsUseable)
                    ability.Use(_playerController);
            }
        }


        private void OnEnable()
        {
            foreach (var ability in currentAbilities)
            {
                InputManager.SetInputActive(ability.IsUseable, ability.Keybind);
            }
        }

        private void OnDisable()
        {
            foreach (var ability in currentAbilities)
            {
                InputManager.SetInputActive(false, ability.Keybind);
            }
        }

        public void EnableAbility(string id)
        {
            if (currentAbilities.Find(a => a.AbilityID.Equals(id)) is {IsUseable: false} foundAbility)
            {
                foundAbility.IsUseable = true;
                InputManager.SetInputActive(foundAbility.IsUseable, foundAbility.Keybind);
            }
        }
    }
}