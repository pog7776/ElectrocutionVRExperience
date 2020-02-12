using UnityEngine;

namespace VRShooterKit
{
    [System.Serializable]
    public class VR_HandInteractSettings
    {
        public Transform interactPoint = null;
        public Transform highlightPoint = null;
        public Vector3 rotationOffset = Vector3.zero;
        public AnimationClip animation = null;
        public bool canInteract = true;
        public bool hideHandOnGrab = false;
    }

}

