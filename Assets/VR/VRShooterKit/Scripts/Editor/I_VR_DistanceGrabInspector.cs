using UnityEngine;
using UnityEditor;

namespace VRShooterKit.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_DistanceGrab))]
    public class I_VR_DistanceGrabInspector : Editor
    {

        private SerializedProperty pointerTransform = null;
        private SerializedProperty grabDistance = null;
        private SerializedProperty grabRadius = null;
        private SerializedProperty checkForObstruction = null;
        private SerializedProperty guideLineAlwaysVisible = null;
        private SerializedProperty canTriggerLineRender = null;
        private SerializedProperty lineTriggerInput = null;
        private SerializedProperty lineRender = null;
        private SerializedProperty layerMask = null;

        private void OnEnable()
        {
            pointerTransform = serializedObject.FindProperty( "pointerTransform" );
            grabDistance = serializedObject.FindProperty( "grabDistance" );
            grabRadius = serializedObject.FindProperty( "grabRadius" );
            checkForObstruction = serializedObject.FindProperty( "checkForObstruction" );
            guideLineAlwaysVisible = serializedObject.FindProperty( "guideLineAlwaysVisible" );
            canTriggerLineRender = serializedObject.FindProperty( "canTriggerLineRender" );
            lineTriggerInput = serializedObject.FindProperty( "lineTriggerInput" );
            lineRender = serializedObject.FindProperty( "lineRender" );
            layerMask = serializedObject.FindProperty( "layerMask" );
        }

        public override void OnInspectorGUI()
        {
            EditorTools.PropertyField(pointerTransform);
            EditorTools.PropertyField(grabDistance);
            EditorTools.PropertyField(grabRadius);
            EditorTools.PropertyField(checkForObstruction);
            EditorTools.PropertyField(guideLineAlwaysVisible);

            if (!guideLineAlwaysVisible.boolValue)
            {
                EditorTools.PropertyField( canTriggerLineRender );

                if (canTriggerLineRender.boolValue)
                    EditorTools.PropertyField( lineTriggerInput );

            }
            else
            {
                canTriggerLineRender.boolValue = false;
            }

            EditorTools.PropertyField(lineRender);
            EditorTools.PropertyField(layerMask);

            serializedObject.ApplyModifiedProperties();

        }

    }
}

