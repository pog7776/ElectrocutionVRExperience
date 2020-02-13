using UnityEngine;
using VRShooterKit.DamageSystem;
using System.Collections.Generic;
using System.Linq;

namespace VRShooterKit.WeaponSystem
{   
    //this script controls the melee weapons like the sword, 
    //in the demo scene and the weapons prefabs, all the weapons can be use as melee weapons to, 
    //so they use this script
    public class VR_MeleeWeapon : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private Transform weaponStartRayMarker = null;
        [SerializeField] private Transform weaponEndRayMarker = null;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float minSpeed = 0.0f;
        [SerializeField] private float dmg = 0.0f;
        [SerializeField] private float hitForce = 0.0f;
        [SerializeField] private float maxHitForce = 800.0f;
        #endregion

        #region PRIVATE
        private VR_Grabbable grabbable = null;
        private float rayLength = 0.0f;
        private List<Damageable> thisDamageableList = null;
        #endregion

        private void Awake()
        {
            grabbable = GetComponent<VR_Grabbable>();            
            thisDamageableList = transform.GetComponentsInChildren<Damageable>().ToList();

            rayLength = Vector3.Distance(weaponStartRayMarker.position , weaponEndRayMarker.position);
        }

        private void Update()
        {
            //check if we are hitting something 
            //we do it in the fixed update because the player can move his hands very quickly
            if ( grabbable.CurrentGrabState == GrabState.Grab && grabbable.GrabController.Velocity.magnitude > minSpeed )
            {
                
                Ray ray = new Ray( weaponStartRayMarker.position , GetDirection() );

                RaycastHit[] hitInfoArray = Physics.RaycastAll( ray, rayLength , layerMask.value , QueryTriggerInteraction.Ignore);

                for (int n = 0; n < hitInfoArray.Length; n++)
                {
                    TryDoDamage( hitInfoArray[n].collider.transform, hitInfoArray[n].point );
                }
            }

        }

        private void OnDrawGizmosSelected()
        {

            if (weaponStartRayMarker == null || weaponEndRayMarker == null)
                return;
            
            Debug.DrawLine( weaponStartRayMarker.position , weaponEndRayMarker.position );
        }

        protected bool TryDoDamage(Transform target, Vector3 hitPoint)
        {
            Damageable[] damageableArray = target.GetComponents<Damageable>();
            Vector3 controllerVelocity = grabbable.GrabController.Velocity;

            if (damageableArray != null && damageableArray.Length > 0)
            {
                for (int n = 0; n < damageableArray.Length; n++)
                {
                    if (damageableArray[n] != null && !thisDamageableList.Contains( damageableArray[n]) )
                    {
                        DamageInfo damageInfo = new DamageInfo();
                        damageInfo.dmg = dmg;
                        damageInfo.hitDir = controllerVelocity.normalized;
                        damageInfo.hitPoint = hitPoint;                        
                        damageInfo.hitForce = Mathf.Min( ( controllerVelocity * hitForce ).magnitude, maxHitForce );                                                
                        damageInfo.sender = grabbable.GrabController != null ? grabbable.GrabController.transform.root.gameObject : null;

                        damageableArray[n].DoDamage( damageInfo );
                    }
                }

                return true;
            }

            return false;
        }

        private Vector3 GetDirection()
        {
            return (weaponEndRayMarker.position - weaponStartRayMarker.position).normalized;
        }

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun
        /// touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        void OnCollisionEnter(Collision other){
            if(other.gameObject.tag == "Enemy"){
                other.gameObject.GetComponent<Enemy>().DoDamage(1000f);
            }

            if(other.gameObject.tag == "Vision"){
                Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
            }
        }
    }

}
