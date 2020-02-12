using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRShooterKit.DamageSystem;


namespace VRShooterKit
{
    public class Explosion : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private float dmg = 200.0f;        
        [SerializeField] private float explosionRange = 0.0f;
        [SerializeField] private float explosionForce = 0.0f;
        [SerializeField] private float upwardsModifier = 0.0f;
        #endregion

        private void Awake()
        {
            Explode();
        }

        public void Explode()
        {           
            Collider[] colliderArray = Physics.OverlapSphere( transform.position, explosionRange );

            Dictionary<DamageablePart, DamageableManager> connections = new Dictionary<DamageablePart, DamageableManager>();

            for (int n = 0; n < colliderArray.Length; n++)
            {
                DamageablePart damageable = colliderArray[n].GetComponent<DamageablePart>();

                if (damageable != null && !connections.ContainsValue( damageable.Owner ))
                {
                    float distance = Vector3.Distance( transform.position, damageable.transform.position );
                    float distanceFactor = Mathf.Abs( ( distance / explosionRange ) - 1.0f ); // a distance factor from 0.0f to 1.0f
                    //set the connection
                    connections[damageable] = damageable.Owner;

                    //create damage info
                    DamageInfo info = new DamageInfo();
                    info.hitForce = explosionForce;
                    info.hitPoint = transform.position;
                    info.explosionRadius = explosionRange;
                    info.upwardsModifier = upwardsModifier;
                    info.dmg = dmg * distanceFactor;
                    info.damageType = DamageType.Explosion;

                    //send damage event
                    damageable.DoDamage( info );
                }
                else
                {
                    Rigidbody rb = colliderArray[n].GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        ApplyImpactForce( rb );
                    }
                    else
                    {
                        VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider( colliderArray[n] );

                        if (grabbable != null && grabbable.RB != null)
                        {
                            ApplyImpactForce( grabbable.RB );
                        }
                    }
                }
            }


            Destroy( gameObject  , 5.0f);
        }

        private void ApplyImpactForce(Rigidbody rb)
        {
            rb.AddExplosionForce( explosionForce, transform.position, explosionRange, upwardsModifier );
        }       

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, explosionRange );
        }
    }

}

