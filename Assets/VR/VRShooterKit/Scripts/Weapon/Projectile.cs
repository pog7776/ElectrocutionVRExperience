using System;
using UnityEngine;
using VRShooterKit.DamageSystem;

namespace VRShooterKit.WeaponSystem
{
    //basic class for all projectiles, arrow, bullets etc
    public class Projectile : MonoBehaviour
    {
        protected ShootInfo shootInfo = null;
        protected bool launched = false;
        protected Collider playerCollider = null;

        public ShootInfo ShootInfo { get { return shootInfo; } }

        protected virtual void Awake()
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag( "Player" );

            if(playerGO != null)
            {
                playerCollider = playerGO.GetComponent<Collider>();
            }
            
        }

        public virtual void Launch(ShootInfo info)
        {
            shootInfo = info;
            launched = true;
        }

        /// <summary>
        /// Do damage to a target
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true if can do damage</returns>
        protected bool TryDoDamage(Collider c)
        {
            Damageable[] damageable = c.GetComponents<Damageable>();

            if (damageable != null && damageable.Length > 0)
            {
                for (int n = 0; n < damageable.Length; n++)
                {
                    DamageInfo damageInfo = new DamageInfo( shootInfo, this );
                    damageable[n].DoDamage( damageInfo );

                    if (shootInfo.hitCallback != null)
                        shootInfo.hitCallback( damageable[n] );

                    
                }

                return true;
            }


            return false;
        }

        protected void ApplyImpactForce(Rigidbody rb , Vector3 impactPoint)
        {
            if (rb == null)
                return;

            rb.AddForceAtPosition( shootInfo.dir * shootInfo.hitForce, impactPoint );
        }
    }

    public class ShootInfo : ICloneable
    {
        public Vector3 dir = Vector3.zero;
        public float dmg = 0.0f;
        public float speed = 0.0f;
        public float range = 0.0f;
        public float hitForce = 0.0f;
        public float gravity = 0.0f;
        public int maxBounceCount = 0;
        public LayerMask hitLayer = new LayerMask();
        public Action<Damageable> hitCallback = null;
        public GameObject hitEffect = null;
        public GameObject sender = null;

        public object Clone()
        {
            ShootInfo clone = new ShootInfo();
            clone.dir = dir;
            clone.dmg = dmg;
            clone.speed = speed;
            clone.range = range;
            clone.hitForce = hitForce;
            clone.gravity = gravity;
            clone.hitLayer = hitLayer;
            clone.hitCallback = hitCallback;
            clone.hitEffect = hitEffect;
            clone.sender = sender;

            return clone;

        }
    }

}

