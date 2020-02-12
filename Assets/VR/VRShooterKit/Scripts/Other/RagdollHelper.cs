﻿using UnityEngine;
using UnityEngine.Events;

namespace VRShooterKit
{
    /// <summary>
    /// Code for activate the ragdoll on this character
    /// </summary>
    public class RagdollHelper : MonoBehaviour
    {
        /// <summary>
        /// Has the character something like a sword? put it here
        /// </summary>
        [SerializeField] private Transform[] equipment = null;
        [SerializeField] private UnityEvent onEnableRagdoll = null;

        public UnityEvent OnEnableRagdoll { get { return onEnableRagdoll; } }

        private Rigidbody[] rbArray = null;

        private void Awake()
        {
            rbArray = GetComponentsInChildren<Rigidbody>();
            SetKinematic(true);
        }

        /// <summary>
        /// Enable the ragdoll
        /// </summary>
        public void EnableRagdoll()
        {
            onEnableRagdoll.Invoke();
            SetKinematic(false);

            for (int n = 0 ; n < equipment.Length ; n++)
            {
                equipment[n].parent = null;                
            }
        }

        private void SetKinematic(bool newValue)
        {
            for (int n = 0 ; n < rbArray.Length ; n++)
            {
                rbArray[n].isKinematic = newValue;
            }

        }

        private void OnDestroy()
        {
            for (int n = 0; n < equipment.Length; n++)
            {
                Destroy(equipment[n].gameObject);
            }
        }

    }

}

