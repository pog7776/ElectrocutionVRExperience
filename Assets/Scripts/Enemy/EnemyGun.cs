
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour{

    [SerializeField]
    private GameObject bullet;

    private Rigidbody bulletRB;

    [SerializeField]
    private GameObject firePoint;

    [Header("Weapon Settings")]
    [SerializeField]
    private float bulletSpeed = 20;
    [SerializeField]
    private float fireRate = 1;

    [SerializeField][Tooltip("Bullet gravity over time")]
    private AnimationCurve bulletDrop;

    [SerializeField][Tooltip("Bullet velocity over distance (player distance)")]
    private AnimationCurve bulletVelocity;

    [SerializeField]
    private bool fireGun = false;


    [HideInInspector]
    public bool canFire = true;
    private Enemy enemy;

    private GameObject aimTarget;

    // Start is called before the first frame update
    void Start(){
        enemy = gameObject.GetComponent<Enemy>();
        canFire = true;
    }

    // Update is called once per frame
    void Update(){
        if(fireGun){
            Fire();
            fireGun = false;
        }
    }

    public void Fire(){
        canFire = false;
        StartCoroutine(FireCoroutine());
    }

    public IEnumerator FireCoroutine(){

        // Set rotation
        Quaternion rotation = Quaternion.Euler(new Vector3(90, this.transform.rotation.y, this.transform.rotation.z));
        
        // Instantiate the bullet
        GameObject firedBullet = Instantiate(bullet, firePoint.transform.position, rotation);

        // Get the RigidBody
        bulletRB = firedBullet.GetComponent<Rigidbody>();
        
        // Add the force
        if(enemy.target != null){
            //bulletRB.AddForce(new Vector3(0, Vector3.Distance(bulletRB.transform.position, enemy.target.transform.position) / 100, 1) * bulletSpeed, ForceMode.VelocityChange);
            float distance = Vector3.Distance(bulletRB.transform.position, enemy.target.transform.position);
            bulletRB.AddForce((aimTarget.transform.position - bulletRB.transform.position) * bulletVelocity.Evaluate(distance), ForceMode.VelocityChange);
        }
        else{
            bulletRB.AddForce(new Vector3(0, 0.15f, 1) * bulletSpeed, ForceMode.VelocityChange);
        }

        // Set bullet mass
        StartCoroutine(SetMass());

        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private IEnumerator SetMass(){
        float time = 0;
        while(time < bulletDrop.length){
            bulletRB.mass = bulletDrop.Evaluate(time);

            if(bulletRB.mass == 0){
                bulletRB.useGravity = false;
            }
            else{
                bulletRB.useGravity = true;
            }

            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetAimTarget(GameObject aimTarget){
        this.aimTarget = aimTarget;
    }
}
