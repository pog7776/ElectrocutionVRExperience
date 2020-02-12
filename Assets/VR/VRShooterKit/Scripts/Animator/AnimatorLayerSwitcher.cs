using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRShooterKit
{    
    public class AnimatorLayerSwitcher : MonoBehaviour
    {
        [SerializeField] private float fadeTime = 0.15f;

        private int currentActiveLayer = 0;
        private Animator animator = null;
        private List<int> animatorLayers = new List<int>();
        private Coroutine layerSwitchCoroutine = null;
        private Coroutine changeLayerValueCoroutine = null;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            GetAllAnimatorLayers();
            currentActiveLayer = GetFirstActiveLayer();
        }

        private void GetAllAnimatorLayers()
        {
            for (int n = 0; n < int.MaxValue; n++)
            {
                string layerName = animator.GetLayerName( n );

                if (layerName.Length == 0)
                {
                    animatorLayers.Add( n );
                }
                else
                {
                    break;
                }

            }
        }

        private int GetFirstActiveLayer()
        {
            for (int n = 0; n < animatorLayers.Count; n++)
            {
                int layer = animatorLayers[n];

                if ( animator.GetLayerWeight(layer) > 0.0f )
                {
                    return layer;
                }
            }

            return -1;
        }

        public void SwitchLayer(int layer)
        {
            if (layerSwitchCoroutine != null)
                StopCoroutine(layerSwitchCoroutine);

            layerSwitchCoroutine = StartCoroutine( SwitchLayerRoutine( layer , fadeTime ) );
        }

        public void EnableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer , 1.0f , fadeTime ) );
        }

        public void DisableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer, 0.0f, fadeTime ) );
        }

        public IEnumerator ChangeLayerValueRoutine(int layer , float v , float t)
        {
            float timer = 0.0f;

            while (timer < t)
            {
                float layerWeight = animator.GetLayerWeight( layer );
                animator.SetLayerWeight( layer, Mathf.Lerp( layerWeight, v, timer / t ) );

                timer += Time.deltaTime;

                yield return new WaitForSeconds( Time.deltaTime );
            }

            animator.SetLayerWeight( layer, v );
        }

        private IEnumerator SwitchLayerRoutine(int layer , float t)
        {
            float timer = 0.0f;

            while (timer < t)
            {
                float layerWeight = animator.GetLayerWeight(layer);
                animator.SetLayerWeight(layer, Mathf.Lerp( layerWeight, 1.0f, timer / t ) );

                layerWeight = animator.GetLayerWeight( (int) currentActiveLayer );
                animator.SetLayerWeight( currentActiveLayer , Mathf.Lerp( layerWeight, 0.0f, timer / t ) );

                timer += Time.deltaTime;

                yield return new WaitForSeconds( Time.deltaTime );
            }

            animator.SetLayerWeight( layer, 1.0f );
            animator.SetLayerWeight( currentActiveLayer , 0.0f );

            currentActiveLayer = layer;

        }


        


    }
}

