using UnityEngine;
using UnityEngine.Events;
using VRShooterKit.Events;
using System.Collections.Generic;
using System.Linq;

namespace VRShooterKit.DamageSystem
{
    public enum DamageType
    {
        Explosion,
        Shoot,
        Physical
    }


    public class DamageableManager : MonoBehaviour
    {
        
        [SerializeField] private float hp = 100.0f;
        [SerializeField] private float regenerationSpeed = 0.0f;
        [SerializeField] private OnDamageEvent onDamage = null;
        [SerializeField] private UnityEvent onDie = null;
        [SerializeField] private OnValueChangeEvent onHPChangeEvent;

        private bool isDead = false;
        private float maxHP = 0.0f;

        public float HP { get { return hp; } }
        public OnDamageEvent OnDamage { get { return onDamage; } }
        public OnValueChangeEvent OnHPChangeEvent { get { return onHPChangeEvent; }  }
        public UnityEvent OnDie { get { return onDie; } }
        public bool Invulnerable { get; set; }
        public bool IsDead { get { return isDead; } }
        public float MaxHP { get { return maxHP; } }

        
        protected virtual void Awake()
        {
            maxHP = hp;

            SetDamageablePartsOwner();
        }

        private void Update()
        {
            if(regenerationSpeed > 0.0f && !isDead)
            {
                ModifyHP( regenerationSpeed * Time.deltaTime );
            }
        }

        private void SetDamageablePartsOwner()
        {
            //get all damageable
            DamageablePart[] damageableArray = GetComponentsInChildren<DamageablePart>();

            //set his owner
            for (int n = 0; n < damageableArray.Length; n++)
            {              
                damageableArray[n].SetOwner( this );
            }
        }

        public void DoDamage(DamageInfo info, DamageablePart damageable)
        {

            //if we are dead just apply the impact force
            if (isDead)
            {
                //ApplyImpactForce( info, damageable );

                if (onDamage != null)
                    onDamage.Invoke( info , damageable );
                return;
            }


            //hp -= info.dmg;

            ModifyHP(-info.dmg);
            

            if (hp <= 0.0f)
            {
                if (onDie != null)
                    onDie.Invoke();

                isDead = true;

                //ApplyImpactForce( info, damageable );
            }

            if (onDamage != null)
                onDamage.Invoke( info, damageable );

        }
        
        public void SetupDamageableLimbs()
        {
            Collider[] colliderArray = transform.GetComponentsInChildren<Collider>();

            for (int n = 0; n < colliderArray.Length; n++)
            {
                if (colliderArray[n].GetComponent<DamageableLimb>() == null)
                {
                    colliderArray[n].gameObject.AddComponent<DamageableLimb>();
                }
            }
        }

        public void ModifyHP(float v)
        {
            hp += v;

            if (hp < 0.0f)
                hp = 0.0f;
            if (hp > MaxHP)
                hp = maxHP;

            onHPChangeEvent.Invoke(hp);
        }

    }
    

}

