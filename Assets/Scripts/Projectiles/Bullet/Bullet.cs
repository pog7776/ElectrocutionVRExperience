using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{

    [SerializeField]
    private float projectileLife = 10;
    [SerializeField]
    private float damage = 30;

    [Header("Bullet Shrink-out Settings")]
    [SerializeField]
    private AnimationCurve shrinkAnimation;
    [SerializeField]
    private float shrinkResolution = 0.1f;

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(LifeTimer(projectileLife));
    }
    
    private IEnumerator LifeTimer(float duration){
        // Wait for timer to run out
        yield return new WaitForSeconds(duration);
        
        // Shrink bullet according to anim curve
        float time = 0;
        while(time < shrinkAnimation.length){
            float size = shrinkAnimation.Evaluate(time);
            transform.localScale = new Vector3(size, size, size);
            time += shrinkResolution;
            yield return new WaitForSeconds(shrinkResolution);
        }

        // Destroy the object
        Destroy(this.gameObject);
        // TODO Object pooler
    }

    public float GetDamage(){
        return damage;
    }
}
