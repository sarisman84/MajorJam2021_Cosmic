using System;
using UnityEngine;

namespace MajorJam.System
{
    public class GrapplePoint : MonoBehaviour
    {
        public Collider Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<Collider>() is { } collider1 ? collider1 : gameObject.AddComponent<SphereCollider>();
        }
        
        
    }
}