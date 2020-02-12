using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRShooterKit
{
    public class FaceCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            Vector3 dir = Camera.main.transform.forward;
            transform.forward = (Camera.main.transform.position - transform.position).normalized;
        }
    }

}

