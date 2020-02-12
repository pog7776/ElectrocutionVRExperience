using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRShooterKit.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( VR_DropZone ) )]
    public class I_VR_DropZoneInspector : Editor
    {
        private VR_DropZone targetScript = null;

        private SerializedProperty dropZoneMode = null;
        private SerializedProperty dropPoint = null;
        private SerializedProperty dropZoneColliderArray = null;
        private SerializedProperty startingDrop = null;
        private SerializedProperty shouldFly = null;
        private SerializedProperty flyTime = null;
        private SerializedProperty syncronizePosition = null;
        private SerializedProperty syncronizeRot = null;
        private SerializedProperty dropRadius = null;
        private SerializedProperty usePreview = null;
        private SerializedProperty onDropStateChange = null;
        private SerializedProperty disableCollidersOnDrop = null;     
        private SerializedProperty canStack = null;

        private void OnEnable()
        {
            targetScript = (VR_DropZone) target;

            dropZoneMode = serializedObject.FindProperty( "dropZoneMode" );
            dropPoint = serializedObject.FindProperty( "dropPoint" );
            dropZoneColliderArray = serializedObject.FindProperty( "dropZoneColliderArray" );
            startingDrop = serializedObject.FindProperty( "startingDrop" );
            shouldFly = serializedObject.FindProperty( "shouldFly" );
            flyTime = serializedObject.FindProperty( "flyTime" );
            syncronizePosition = serializedObject.FindProperty( "syncronizePosition" );
            syncronizeRot = serializedObject.FindProperty( "syncronizeRot" );
            dropRadius = serializedObject.FindProperty( "dropRadius" );
            usePreview = serializedObject.FindProperty( "usePreview" );
            onDropStateChange = serializedObject.FindProperty( "onDrop" );
            disableCollidersOnDrop = serializedObject.FindProperty( "disableCollidersOnDrop" );           
            canStack = serializedObject.FindProperty( "canStack" );
        }

        public override void OnInspectorGUI()
        {
            EditorTools.PropertyField( dropZoneMode );
            EditorTools.PropertyField( dropPoint );
            EditorTools.PropertyField( startingDrop );

            if ((DropZoneMode) dropZoneMode.enumValueIndex == DropZoneMode.Collider)
            {
                EditorTools.PropertyField( dropZoneColliderArray, true );
            }
            else
            {
                EditorTools.PropertyField( dropRadius );
            }

            EditorTools.PropertyField( shouldFly );

            if (shouldFly.boolValue)
            {
                EditorTools.PropertyField( flyTime );
            }
           
            EditorTools.PropertyField( syncronizePosition );
            EditorTools.PropertyField( syncronizeRot );
            EditorTools.PropertyField( usePreview );
            EditorTools.PropertyField(canStack);
            EditorTools.PropertyField(disableCollidersOnDrop);
            EditorTools.PropertyField( onDropStateChange );

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (targetScript == null)
                OnEnable();

            if (targetScript.DropPoint == null)
                return;


            if (targetScript.DropPoint == null || (DropZoneMode) dropZoneMode.enumValueIndex == DropZoneMode.Collider)
                return;

            //handler for dropzone range
            EditorGUI.BeginChangeCheck();

            Handles.color = Color.blue;
            float r = Handles.RadiusHandle( Quaternion.identity, targetScript.DropPoint.position, dropRadius.floatValue );



            if (GUI.changed)
            {
                targetScript.SetDropRadiusViaInspector( r );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject( target, "Changed Drop Distance" );
                targetScript.SetDropRadiusViaInspector(r);
                EditorUtility.SetDirty(targetScript);
                EditorSceneManager.MarkAllScenesDirty();
            }

           
        }

    }

}

