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
                if (ability.isUsable)
                    ability.Use(_playerController);
            }
        }


        private void OnEnable()
        {
            foreach (var ability in currentAbilities)
            {
                InputManager.SetInputActive(ability.isUsable, ability.keybind);
            }
        }

        private void OnDisable()
        {
            foreach (var ability in currentAbilities)
            {
                InputManager.SetInputActive(false, ability.keybind);
            }
        }

        public void EnableAbility(string id)
        {
            if (currentAbilities.Find(a => a.abilityID.Equals(id)) is {isUsable: false} foundAbility)
            {
                foundAbility.isUsable = true;
                InputManager.SetInputActive(foundAbility.isUsable, foundAbility.keybind);
            }
        }
    }
}