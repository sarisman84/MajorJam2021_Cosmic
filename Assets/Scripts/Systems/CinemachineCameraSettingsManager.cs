using System;
using Cinemachine;
using UnityEngine;

namespace Systems
{
    public class CinemachineCameraSettingsManager : MonoBehaviour
    {
        public CinemachineFreeLook freelookAffector;
        public float defaultSensitivity = 0.25f;

        private void Awake()
        {
            UpdateSensitivity(defaultSensitivity);
        }

        public void UpdateSensitivity(float value)
        {
            freelookAffector.m_XAxis.m_MaxSpeed = value;
            freelookAffector.m_YAxis.m_MaxSpeed = value/100f;
        }

    }
}