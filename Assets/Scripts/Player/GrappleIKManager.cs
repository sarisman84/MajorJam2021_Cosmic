using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class GrappleIKManager : MonoBehaviour
{
    public GrappleController grappleController;
    public float transitionTime = 0.2f;

    private TwoBoneIKConstraint m_GrappleIK;
    private Transform m_Target;


    private void Awake()
    {
        m_GrappleIK = GetComponent<TwoBoneIKConstraint>();
        m_Target = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (grappleController.LatestPoint is { } existingPoint && grappleController.IsGrappling)
        {
            m_Target.position = existingPoint.transform.position;
            m_GrappleIK.weight = Mathf.Lerp(m_GrappleIK.weight, 1, transitionTime * Time.deltaTime);
            return;
        }
        m_GrappleIK.weight = Mathf.Lerp(m_GrappleIK.weight, 0, transitionTime * Time.deltaTime);
    }
}