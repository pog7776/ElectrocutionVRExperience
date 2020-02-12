using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRShooterKit.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_Slider))]
    public class I_VR_SliderInspector : Editor
    {
        private SerializedProperty onGrabStateChange = null;
        private SerializedProperty perfectGrab = null;
        private SerializedProperty grabDistance = null;        
        private SerializedProperty shouldFly = null;
        private SerializedProperty startOnRightcController = null;
        private SerializedProperty startOnLeftController = null;
        private SerializedProperty autoGrab = null;
        private SerializedProperty grabButton = null;
        private SerializedProperty grabLayer = null;
        private SerializedProperty unGrabLayer = null;
        private SerializedProperty enableCollidersOnGrab = null;
        private SerializedProperty usePerHandSettings = null;
        private SerializedProperty leftHandSettings = null;
        private SerializedProperty handSettings = null;
        private SerializedProperty rightHandSettings = null;        
        private SerializedProperty preserveKinematicState = null;
        private SerializedProperty slideAxis = null;
        private SerializedProperty grabMode = null;

        private VR_Grabbable targetScript = null;

        private void OnEnable()
        {
            onGrabStateChange = serializedObject.FindProperty( "onGrabStateChange" );
            perfectGrab = serializedObject.FindProperty( "perfectGrab" );
            grabDistance = serializedObject.FindProperty( "interactDistance" );            
            shouldFly = serializedObject.FindProperty( "shouldFly" );
            startOnRightcController = serializedObject.FindProperty( "startOnRightcController" );
            startOnLeftController = serializedObject.FindProperty( "startOnLeftController" );
            autoGrab = serializedObject.FindProperty( "autoGrab" );
            grabButton = serializedObject.FindProperty( "interactButton" );
            grabLayer = serializedObject.FindProperty( "grabLayer" );
            unGrabLayer = serializedObject.FindProperty( "unGrabLayer" );
            enableCollidersOnGrab = serializedObject.FindProperty( "enableColliderOnGrab" );                 
            usePerHandSettings = serializedObject.FindProperty( "usePerHandSettings" );
            rightHandSettings = serializedObject.FindProperty( "rightHandSettings" );
            leftHandSettings = serializedObject.FindProperty( "leftHandSettings" );
            handSettings = serializedObject.FindProperty( "handSettings" );           
            preserveKinematicState = serializedObject.FindProperty( "preserveKinematicState" );
            slideAxis = serializedObject.FindProperty("slideAxis");
            grabMode = serializedObject.FindProperty("grabMode");

            targetScript = (VR_Grabbable) target;
        }

        public override void OnInspectorGUI()
        {
            EditorTools.PropertyField(grabMode);
            EditorTools.PropertyField( slideAxis );
            EditorTools.PropertyField( grabDistance );            
            EditorTools.PropertyField( usePerHandSettings );

            if (usePerHandSettings.boolValue)
            {
                EditorGUILayout.PropertyField( rightHandSettings, true );
                EditorGUILayout.PropertyField( leftHandSettings, true );
            }
            else
            {
                EditorGUILayout.PropertyField( handSettings, true );
            }

            
            EditorTools.PropertyField( enableCollidersOnGrab );
            
            if (!autoGrab.boolValue)
                EditorTools.PropertyField( grabButton );

            grabLayer.intValue = EditorGUILayout.LayerField( "Grab Layer", grabLayer.intValue );
            unGrabLayer.intValue = EditorGUILayout.LayerField( "UnGrab Layer", unGrabLayer.intValue );

            EditorTools.PropertyField( preserveKinematicState );

            EditorTools.PropertyField( onGrabStateChange );

            if (perfectGrab.boolValue)
            {
                shouldFly.boolValue = false;
            }

            if (perfectGrab.boolValue)
            {
                autoGrab.boolValue = false;
            }

            if (!autoGrab.boolValue)
            {
                startOnLeftController.boolValue = false;
                startOnRightcController.boolValue = false;
            }



            //layer = EditorGUILayout.LayerField(layer);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHandSettings(SerializedProperty settings)
        {
            EditorGUILayout.PropertyField( settings, true );
        }


        public void OnSceneGUI()
        {
            if (targetScript == null)
                OnEnable();

            if (autoGrab.boolValue)
                return;


            //handler for grabbable distance
            EditorGUI.BeginChangeCheck();

            Handles.color = Color.blue;


            if (usePerHandSettings.boolValue)
            {
                float rightValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointRightHand == null ? targetScript.transform.position : targetScript.HighlightPointRightHand.position, grabDistance.floatValue );
                float leftValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointLeftHand == null ? targetScript.transform.position : targetScript.HighlightPointLeftHand.position, grabDistance.floatValue );

                UpdateGrabDistanceValue( rightValue, leftValue );
            }
            else
            {
                Transform center = targetScript.HighlightPointHandSettings != null ? targetScript.HighlightPointHandSettings : targetScript.transform;


                float value = Handles.RadiusHandle( Quaternion.identity, center.position, grabDistance.floatValue );
                targetScript.SetInteractDistanceViaInspector( value );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }

        }

        private void UpdateGrabDistanceValue(float rightValue, float leftValue)
        {
            if (grabDistance.floatValue != rightValue)
            {
                targetScript.SetInteractDistanceViaInspector( rightValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
            else if (grabDistance.floatValue != leftValue)
            {
                targetScript.SetInteractDistanceViaInspector( leftValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}

