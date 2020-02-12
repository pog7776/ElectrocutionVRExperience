﻿using UnityEngine;
using System;

namespace VRShooterKit
{
    public class TriggerCollisionListener : MonoBehaviour
    {

        public Action<Collider> OnTriggerEnterEvent = null;
        public Action<Collider> OnTriggerExitEvent = null;
        public Action<Collider> OnTriggerStayEvent = null;

        private void Awake()
        {
            Collider c = GetComponent<Collider>();

            if (c != null)
                c.isTrigger = true;
            else
                Debug.LogError( "you add a trigger collision listener in a gameobject with no collider!" );
        }

        private void OnTriggerEnter(Collider other)
        {
            if (OnTriggerEnterEvent != null)
                OnTriggerEnterEvent( other );
        }

        private void OnTriggerExit(Collider other)
        {
            if (OnTriggerExitEvent != null)
                OnTriggerExitEvent( other );
        }

        private void OnTriggerStay(Collider other)
        {
            if (OnTriggerStayEvent != null)
                OnTriggerStayEvent( other );
        }
    }

}

