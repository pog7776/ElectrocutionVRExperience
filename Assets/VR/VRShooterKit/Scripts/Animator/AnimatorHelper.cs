using UnityEngine;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace VRShooterKit
{
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private List<LayerInfo> animatorControllerInfo = null;
        [SerializeField] private float layerTransitionTime = 0.15f;

        private Animator animator = null;        
        private Coroutine changeLayerValueCoroutine = null;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void EnableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer, 1.0f, layerTransitionTime ) );
        }

        public void DisableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer, 0.0f, layerTransitionTime ) );
        }

        public IEnumerator ChangeLayerValueRoutine(int layer, float v, float t)
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

        public bool IsPlayingOrTransitionToState(int layer , string stateName)
        {
            return IsPlayingState( layer, stateName ) || IsTransitionToState(layer , stateName);
        }

        public bool IsPlayingState(int layer , string stateName)
        {            
            return animator.GetCurrentAnimatorStateInfo( layer ).IsName( stateName );
        }

        public bool IsTransitionToState(int layer , string stateName)
        {
            List<string> states = animatorControllerInfo[layer].animatorStates;

            for (int n = 0; n < states.Count; n++)
            {
                if (animator.GetAnimatorTransitionInfo( layer ).IsName( states[n] + " -> " + stateName ))
                    return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public void ConstructAnimatorControllerInfo()
        {
            animatorControllerInfo = new List<LayerInfo>();

            AnimatorController ac = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;

            AnimatorControllerLayer[] layerArray = ac.layers;

            for (int n = 0; n < layerArray.Length; n++)
            {
                LayerInfo info = new LayerInfo();
                info.animatorStates = GetStatesFromLayer( layerArray[n] ); ;

                animatorControllerInfo.Add(info);
            }
        }

        private List<string> GetStatesFromLayer(AnimatorControllerLayer layer)
        {
            List<string> states = new List<string>();

            for (int n = 0; n < layer.stateMachine.states.Length; n++)
            {
                states.Add( layer.stateMachine.states[n].state.name );
            }

            return states;
        }
#endif

    }

    [System.Serializable]
    public class LayerInfo
    {
        public List<string> animatorStates = new List<string>();
    }
}

