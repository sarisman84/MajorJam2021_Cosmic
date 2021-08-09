using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GrappleController : MonoBehaviour
{
    [SerializeField] private InputActionReference grapple;
    [SerializeField] private float grappleDistance;

    private bool isGrapling = false;
    private List<GameObject> grapplePoints;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        grapplePoints = GameObject.FindGameObjectsWithTag("GrapplePoint").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        bool grapling = grapple.GetButton();

        if (grapling && !isGrapling)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            grapplePoints = grapplePoints.OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).ToList();
            
            GameObject closestPoint = grapplePoints.Find(p => 
                GeometryUtility.TestPlanesAABB(planes, p.GetComponent<Collider>().bounds) && 
                Physics.Raycast(transform.position, (p.transform.position - transform.position).normalized, out var hitInfo, grappleDistance) && 
                hitInfo.transform.CompareTag("GrapplePoint"));

            if (closestPoint)
            {
                Debug.Log(closestPoint.name);
            }
        }
        isGrapling = grapling;

    }

    private void FixedUpdate()
    {

    }


    private void OnEnable()
    {
        InputManager.SetInputActive(true, grapple);
    }

    private void OnDisable()
    {
        InputManager.SetInputActive(false, grapple);
    }
}
