using UnityEngine;
using System.Collections;

namespace VRShooterKit.WeaponSystem
{
    public class Bullet : Projectile{
        [SerializeField] private TrailRenderer trailRender = null;
        [SerializeField] private Gradient invisibleGrandient;
        [SerializeField] private float damage = 10;

        private float distanceTraveled = 0.0f;
        private bool shouldHit = false;
        private int bounceCount = 0;
        private Gradient originalGradient;

        protected override void Awake()
        {
            base.Awake();

            if (trailRender != null)
            {
                originalGradient = trailRender.colorGradient;
            }

        }

        private void Update()
        {
            if (!launched)
                return;
            
            if (shouldHit)
            {
                
                if (shootInfo.hitEffect != null)
                    Instantiate(shootInfo.hitEffect , transform.position , Quaternion.identity);

                Destroy( gameObject );           
                return;
            }

                      
            float travelDistance = Time.deltaTime * shootInfo.speed;
            distanceTraveled += travelDistance;

           

            RaycastHit hit;
            bool hitSomething = Physics.Raycast( transform.position, transform.forward, out hit, travelDistance, shootInfo.hitLayer.value );
            
            if (hitSomething )//&& hit.collider != playerCollider)
            {                
                transform.position += transform.forward * hit.distance;                
                bool handleByDamageSystem = TryDoDamage(hit.collider);

                //if we hit a object that dont process the damage in his on way using Damageable components, add just the impact force in a default way
                if (!handleByDamageSystem)
                {
                     ApplyImpactForce( hit.rigidbody, hit.point );                    
                }
                    

                SurfaceDetails surface = hit.collider.GetComponent<SurfaceDetails>();

                if (surface != null)
                {
                    if (surface.BulletsCanBounce && bounceCount < shootInfo.maxBounceCount)
                    {
                        BounceOnSurface( hit , surface );
                    }
                    else
                    {
                        shouldHit = true;
                    }
                }
                else if (bounceCount < shootInfo.maxBounceCount)
                {
                    BounceOnSurface( hit , null );
                }
                else
                {
                    shouldHit = true;
                }

            }
            else
            {
                transform.position += shootInfo.dir * travelDistance;

                if (distanceTraveled >= shootInfo.range)
                {
                    Destroy( gameObject );
                }
                   
            }

        }

        private void BounceOnSurface(RaycastHit hit , SurfaceDetails surface = null)
        {
            if(surface.gameObject.tag != "Vision"){
                bounceCount++;
                //reflect the bullet on the surface
                Vector3 newDir = Vector3.Reflect( transform.forward, hit.normal );
                transform.forward = newDir;
                shootInfo.dir = newDir;

                float speedLose = 0.20f;

                if (surface != null)
                    speedLose = surface.BulletsSpeedLoseOnBounce;

                shootInfo.speed -= speedLose * shootInfo.speed;
            }
        }

        private int RandomSign()
        {
            return Random.value < .5 ? 1 : -1;
        }

        public override void Launch(ShootInfo info)
        {
            ApplyPlayerVelocity();
            StartCoroutine( SetInvisibleForTwoFrames() );
            base.Launch( info );
            MoveForward(info);
         
        }

        private void ApplyPlayerVelocity()
        {
            Vector3 playerVelocity = VR_Manager.instance.CharacterController.velocity;
            playerVelocity.y = 0.0f;
            transform.position += playerVelocity * Time.deltaTime * 3.0f;
        }

        private void MoveForward(ShootInfo info)
        {
            float originalSpeed = info.speed;
            info.speed = 0.15f;

            Update();
            info.speed = originalSpeed;
        }

        private IEnumerator SetInvisibleForTwoFrames()
        {
            trailRender.colorGradient = invisibleGrandient;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            trailRender.colorGradient = originalGradient;


           

            launched = true;
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Enemy"){
                other.GetComponent<Enemy>().DoDamage(damage);
                Destroy(gameObject);
            }

            // if(other.gameObject.tag == "Vision"){
            //     Physics.IgnoreCollision(other, GetComponent<Collider>());
            // }
        }

    }

}

