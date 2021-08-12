using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovablePlatform : MonoBehaviour
    {
        public enum LoopType
        {
            Full,
            Reverse,
            Teleport,
            None
        }

        public List<Vector3> waypoints;
        public LoopType loopType;
        public float movementSpeed;
        public float waypointTransitionDelay = 0;

        private int m_CurrentWaypoint;
        private Vector3 m_StartingPosition;
        private Rigidbody m_Rigidbody;
        private bool m_HasReachedDestination;
        private float m_LocalTimer;

        public Vector3 GetTargetWaypointPosition(int index)
        {
            return waypoints[index] + m_StartingPosition;
        }


        private void Awake()
        {
            m_StartingPosition = transform.position;
            m_Rigidbody = GetComponent<Rigidbody>();
            transform.position = GetTargetWaypointPosition(m_CurrentWaypoint);
            m_CurrentWaypoint++;
        }

        private void Update()
        {
            if (m_HasReachedDestination)
            {
                m_Rigidbody.velocity = Vector3.zero;
                m_LocalTimer += Time.deltaTime;
                if (m_LocalTimer >= waypointTransitionDelay)
                {
                    m_CurrentWaypoint = (m_CurrentWaypoint + 1) % waypoints.Count;
                    m_LocalTimer = 0;
                    m_HasReachedDestination = false;
                }
            }
        }


        private void FixedUpdate()
        {
            if (!m_HasReachedDestination)
            {
                Vector3 dir = (GetTargetWaypointPosition(m_CurrentWaypoint) - transform.position).normalized;
                m_Rigidbody.MovePosition(m_Rigidbody.position + dir * (movementSpeed * Time.fixedDeltaTime));
                m_HasReachedDestination =
                    IsPositionReached(transform.position, GetTargetWaypointPosition(m_CurrentWaypoint));
                CarryRigidbodiesOver(FetchGameObjects(), dir * (movementSpeed * Time.fixedDeltaTime));
            }
        }

        private Collider[] FetchGameObjects()
        {
            Collider[] foundObjects = new Collider[50];
            Physics.OverlapBoxNonAlloc(
                new Vector3(transform.position.x, (transform.position.y + transform.localScale.y / 2f),
                    transform.position.z), transform.localScale / 2f, foundObjects, transform.rotation);
            return foundObjects;
        }

        private void CarryRigidbodiesOver(Collider[] foundObjects, Vector3 dir)
        {
            foreach (var foundObject in foundObjects)
            {
                if (foundObject)
                    foundObject.transform.position += dir;
            }
        }


        private void OnDrawGizmos()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                m_StartingPosition = transform.position;
            }

            DrawWaypoints();
        }

        private void DrawWaypoints()
        {
            if (waypoints != null)
                for (int i = 0; i < waypoints.Count; i++)
                {
                    Vector3 waypointA = GetTargetWaypointPosition(i);


                    Vector3 waypointB = GetTargetWaypointPosition((i + 1) % waypoints.Count);


                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(waypointA, 0.25f);
                    if (waypointA != waypointB)
                    {
                        Gizmos.DrawSphere(waypointB, 0.25f);
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(waypointA, waypointB);
                    }
                }
        }


        private bool IsPositionReached(Vector3 origin, Vector3 targetPosition,
            float minDistanceFromTargetPosition = 0.1f)
        {
            bool isNearPosX = origin.x > targetPosition.x - minDistanceFromTargetPosition &&
                              origin.x < targetPosition.x + minDistanceFromTargetPosition;
            bool isNearPosY = origin.y > targetPosition.y - minDistanceFromTargetPosition &&
                              origin.y < targetPosition.y + minDistanceFromTargetPosition;
            bool isNearPosZ = origin.z > targetPosition.z - minDistanceFromTargetPosition &&
                              origin.z < targetPosition.z + minDistanceFromTargetPosition;

            return isNearPosX && isNearPosY && isNearPosZ;
        }
    }
}