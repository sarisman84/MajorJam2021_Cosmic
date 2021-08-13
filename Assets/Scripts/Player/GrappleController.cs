using System;
using MajorJam.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GrappleController : MonoBehaviour
{
    [SerializeField] private InputActionReference grapple;
    [SerializeField] private LineRenderer grappleRenderer;
    [SerializeField] private float grappleDetectionDistance = 20f;
    [SerializeField] private float grappleDistance;
    [SerializeField] private Sprite grappleSelectionIndicator;

    private bool m_IsAlreadyGrapling = false;
    private List<GrapplePoint> m_GrapplePoints;
    private SpringJoint m_Joint;
    private Vector3 m_CurrentGrapplePos;
    private Vector3 m_GrapplePoint;
    private GrapplePoint m_ClosestPoint;
    private SpriteRenderer m_GrappleSelectionRenderer;

    Camera m_Cam;

    public GrapplePoint LatestPoint => m_ClosestPoint;
    public bool IsGrappling => grapple.GetButton();

    // Start is called before the first frame update
    void Start()
    {
        m_Cam = Camera.main;
        m_GrapplePoints = GameObject.FindObjectsOfType<GrapplePoint>().ToList();
        grappleRenderer.enabled = false;
        m_GrappleSelectionRenderer = new GameObject("Grapple Point Indicator").AddComponent<SpriteRenderer>();
        m_GrappleSelectionRenderer.sprite = grappleSelectionIndicator;
        m_GrappleSelectionRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       

        m_ClosestPoint = !m_IsAlreadyGrapling ? GetClosestGrapplePoint() : m_ClosestPoint;

        UpdateGrappleSelectionIndicator(IsGrappling);


        if (IsGrappling && !m_IsAlreadyGrapling && m_ClosestPoint)
        {
            if (m_ClosestPoint)
                StartGrappling(m_ClosestPoint.transform.position);
        }

        if (m_IsAlreadyGrapling && !IsGrappling)
        {
            StopGrappling();
        }

        m_IsAlreadyGrapling = IsGrappling && m_ClosestPoint;
    }

    /// <summary>
    /// Updates the indicator's active state as well as position and rotation information to align with the latest grapple point found and rotate towards the player's camera.
    /// </summary>
    /// <param name="grappling">Indication of an attempt to grapple something.</param>
    private void UpdateGrappleSelectionIndicator(bool grappling)
    {
        m_GrappleSelectionRenderer.gameObject.SetActive(m_ClosestPoint && !grappling);

        if (!grappling && m_ClosestPoint)
        {
            var indicator = m_GrappleSelectionRenderer.transform;
            var closestPointTransform = m_ClosestPoint.transform;

            indicator.position = m_ClosestPoint
                ? closestPointTransform.position
                : indicator.position;

            indicator.rotation =
                Quaternion.LookRotation((m_Cam.transform.position - closestPointTransform.position).normalized,
                    Vector3.up);
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StopGrappling()
    {
        grappleRenderer.positionCount = 0;
        Destroy(m_Joint);
        grappleRenderer.enabled = false;
    }

    private void StartGrappling(Vector3 grapplePoint)
    {
        grappleRenderer.enabled = true;
        m_GrapplePoint = grapplePoint;
        m_Joint = gameObject.AddComponent<SpringJoint>();
        m_Joint.autoConfigureConnectedAnchor = false;
        m_Joint.connectedAnchor = grapplePoint;

        //The distance grapple will try to keep from grapple point. 
        m_Joint.maxDistance = grappleDistance * 0.8f;
        m_Joint.minDistance = grappleDistance * 0.2f;

        //Adjust these values to fit your game.
        m_Joint.spring = 4.5f;
        m_Joint.damper = 7f;
        m_Joint.massScale = 4.5f;

        grappleRenderer.positionCount = 2;
        m_CurrentGrapplePos = grappleRenderer.transform.position;
    }

    private void DrawRope()
    {
        if (!m_Joint) return;

        m_CurrentGrapplePos = Vector3.Lerp(m_CurrentGrapplePos, m_GrapplePoint, Time.deltaTime * 8f);

        grappleRenderer.SetPosition(0, grappleRenderer.transform.position);
        grappleRenderer.SetPosition(1, m_CurrentGrapplePos);
    }

    private GrapplePoint GetClosestGrapplePoint()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_Cam);
        // m_GrapplePoints = m_GrapplePoints.OrderBy(p => Vector3.Distance(p.transform.position, transform.position))
        //     .ToList();

        GrapplePoint closestPoint = m_GrapplePoints.Find(p =>
            Vector3.Distance(p.transform.position, transform.position) <= grappleDetectionDistance &&
            GeometryUtility.TestPlanesAABB(planes, p.Collider.bounds) &&
            Physics.Raycast(transform.position, (p.transform.position - transform.position).normalized, out var hitInfo,
                grappleDetectionDistance) &&
            hitInfo.transform.CompareTag("GrapplePoint") && IsInfrontOfThePlayer(p) > 0.8f);
        return closestPoint;
    }

    private float IsInfrontOfThePlayer(GrapplePoint grapplePoint)
    {
        Vector3 directionFromCam = (grapplePoint.transform.position - m_Cam.transform.position).normalized;
        Vector3 directionFromPlayer = (grapplePoint.transform.position - transform.position).normalized;

        float dot = Vector3.Dot(directionFromCam, directionFromPlayer);


        return dot;
    }


    private void OnEnable()
    {
        InputManager.SetInputActive(true, grapple);
    }

    private void OnDisable()
    {
        InputManager.SetInputActive(false, grapple);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue - new Color(0, 0, 0, 0.75f);
        m_GrapplePoints = m_GrapplePoints != null && m_GrapplePoints.Count != 0 &&
                          FindObjectsOfType<GrapplePoint>() is { } foundObjects &&
                          foundObjects.Length == m_GrapplePoints.Count
            ? m_GrapplePoints
            : FindObjectsOfType<GrapplePoint>().ToList();
        foreach (var grapplePoint in m_GrapplePoints)
        {
            Gizmos.DrawSphere(grapplePoint.transform.position, grappleDetectionDistance);
        }
    }
}