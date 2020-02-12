using UnityEngine;

namespace VRShooterKit.DamageSystem
{
    public class DamageablePart : Damageable
    {
        [SerializeField] private float damageMultiplier = 1.0f;
        [SerializeField] private Rigidbody rb = null;

        private DamageableManager owner = null;


        public Rigidbody RB { get { return rb; } }
        public DamageableManager Owner { get { return owner; } }

        private void Awake()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();
        }

        public void SetOwner(DamageableManager owner)
        {
            this.owner = owner;
        }

        public override void DoDamage(DamageInfo info)
        {
            info.dmg *= damageMultiplier;
            owner.DoDamage( info, this );
        }

        private void ProcessHit(Rigidbody rb , GameObject sender)
        {
            DamageInfo info = new DamageInfo();
            info.damageType = DamageType.Physical;
            info.hitDir = rb.velocity.normalized;
            info.dmg = rb.velocity.magnitude * damageMultiplier;
            info.hitForce = rb.velocity.magnitude;
            info.sender = sender;
            

            DoDamage( info );

        }

        private void OnCollisionEnter(Collision other)
        {
            //in this way we can respond to hits from objects and apply damage,
            //like the player throwing a box to a enemy
            if (other.rigidbody != null)
            {              
                
                VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider(other.collider);


                if (grabbable != null && grabbable.ObjectWasThrow && grabbable.LastInteractController != null)
                {                   
                    GameObject sender = grabbable.LastInteractController.transform.root.gameObject;
                    ProcessHit( other.rigidbody, sender );
                }

                
            }

        }
    }
}

