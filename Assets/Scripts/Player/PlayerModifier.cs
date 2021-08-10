using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerModifier : MonoBehaviour
    {
        private PlayerController _playerController;
        private bool _isModifierActive;
        
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void ApplySpeedModifier(float value)
        {
            if (!_isModifierActive)
            {

                _playerController.CustomAcceleration = value;
                _playerController.CustomMaxMovementSpeed = value;
                _isModifierActive = true;
            }
        }
    }
}