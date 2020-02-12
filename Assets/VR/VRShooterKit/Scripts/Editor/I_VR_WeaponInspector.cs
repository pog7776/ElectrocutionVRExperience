using UnityEngine;
using UnityEditor;
using VRShooterKit.WeaponSystem;
using UnityEditor.SceneManagement;

namespace VRShooterKit.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( VR_Weapon ) )]
    public class I_VR_WeaponInspector : Editor
    {

        private SerializedProperty reloadMode = null;       
        private SerializedProperty barrelScript = null;
        private SerializedProperty reloadAngle = null;
        private SerializedProperty bulletPrefab = null;
        private SerializedProperty weaponUI = null;
        private SerializedProperty magazineDropZone = null;
        private SerializedProperty muzzleFlash = null;
        private SerializedProperty bulletSpeed = null;
        private SerializedProperty dmg = null;
        private SerializedProperty minHitForce = null;
        private SerializedProperty maxHitForce = null;
        private SerializedProperty range = null;
        private SerializedProperty fireButton = null;
        private SerializedProperty shootPoint = null;
        private SerializedProperty shootRate = null;
        private SerializedProperty hitLayer = null;
        private SerializedProperty minRecoilPositionForce = null;
        private SerializedProperty maxRecoilPositionForce = null;
        private SerializedProperty recoilPositionLimit = null;
        private SerializedProperty minRecoilRotationForce = null;
        private SerializedProperty maxRecoilRotationForce = null;
        private SerializedProperty recoilAngleLimit = null;
        private SerializedProperty positionLerpSpeed = null;
        private SerializedProperty rotationLerpSpeed = null;        
        private SerializedProperty shootSound = null;
        private SerializedProperty clipSize = null;
        private SerializedProperty reloadTime = null;
        private SerializedProperty useSpread = null;
        private SerializedProperty minSpreadCount = null;
        private SerializedProperty maxSpreadCount = null;
        private SerializedProperty minSpreadAngle = null;
        private SerializedProperty maxSpreadAngle = null;
        private SerializedProperty isAutomatic = null;
        private SerializedProperty maxBulletBounceCount = null;
        private SerializedProperty weaponHammer = null;
        private SerializedProperty shellEjector = null;
        private SerializedProperty parentMuzzleFlash = null;
        private SerializedProperty disableMuzzleWhileNoShooting = null;
        private SerializedProperty muzzleLiveTime = null;
        private SerializedProperty slider = null;
        private SerializedProperty bulletInsertPoint = null;
        private SerializedProperty hitEffect = null;


        private const float MAX_HIT_FORCE = 1500.0f;
        private const float MIN_HIT_FORCE = 0.0f;
        private const float MAX_POSITION_FORCE = 1.0f;
        private const float MIN_POSITION_FORCE = 0.0f;
        private const float MAX_ROTATION_FORCE = 90.0f;
        private const float MIN_ROTATION_FORCE = 0.0f;


        private void OnEnable()
        {
            
            reloadMode = serializedObject.FindProperty( "reloadMode" );           
            barrelScript = serializedObject.FindProperty( "barrelScript" );
            reloadAngle = serializedObject.FindProperty( "reloadAngle" );
            bulletPrefab = serializedObject.FindProperty( "bulletPrefab" );
            weaponUI = serializedObject.FindProperty( "weaponUI" );
            magazineDropZone = serializedObject.FindProperty( "magazineDropZone" );           
            muzzleFlash = serializedObject.FindProperty( "muzzleFlash" );
            bulletSpeed = serializedObject.FindProperty( "bulletSpeed" );
            dmg = serializedObject.FindProperty( "dmg" );
            minHitForce = serializedObject.FindProperty( "minHitForce" );
            maxHitForce = serializedObject.FindProperty( "maxHitForce" );
            range = serializedObject.FindProperty( "range" );
            fireButton = serializedObject.FindProperty( "fireButton" );
            shootPoint = serializedObject.FindProperty( "shootPoint" );
            hitLayer = serializedObject.FindProperty( "hitLayer" );
            shootRate = serializedObject.FindProperty( "shootRate" );
            minRecoilPositionForce = serializedObject.FindProperty( "minRecoilPositionForce" );
            maxRecoilPositionForce = serializedObject.FindProperty( "maxRecoilPositionForce" );
            recoilAngleLimit = serializedObject.FindProperty("recoilAngleLimit");
            minRecoilRotationForce = serializedObject.FindProperty( "minRecoilRotationForce" );
            maxRecoilRotationForce = serializedObject.FindProperty( "maxRecoilRotationForce" );
            recoilPositionLimit = serializedObject.FindProperty("recoilPositionLimit");
            positionLerpSpeed = serializedObject.FindProperty( "positionLerpSpeed" );
            rotationLerpSpeed = serializedObject.FindProperty( "rotationLerpSpeed" );           
            shootSound = serializedObject.FindProperty( "shootSound" );
            clipSize = serializedObject.FindProperty( "clipSize" );
            reloadTime = serializedObject.FindProperty( "reloadTime" );
            useSpread = serializedObject.FindProperty( "useSpread" );
            minSpreadCount = serializedObject.FindProperty( "minSpreadCount" );
            maxSpreadCount = serializedObject.FindProperty( "maxSpreadCount" );
            minSpreadAngle = serializedObject.FindProperty( "minSpreadAngle" );
            maxSpreadAngle = serializedObject.FindProperty( "maxSpreadAngle" );
            isAutomatic = serializedObject.FindProperty( "isAutomatic" );
            maxBulletBounceCount = serializedObject.FindProperty( "maxBulletBounceCount" );
            weaponHammer = serializedObject.FindProperty("weaponHammer");
            shellEjector = serializedObject.FindProperty("shellEjector");
            parentMuzzleFlash = serializedObject.FindProperty( "parentMuzzleFlash" );
            disableMuzzleWhileNoShooting = serializedObject.FindProperty( "disableMuzzleWhileNoShooting" );
            muzzleLiveTime = serializedObject.FindProperty("muzzleLiveTime");
            slider = serializedObject.FindProperty( "slider" );
            bulletInsertPoint = serializedObject.FindProperty( "bulletInsertPoint" );
            hitEffect = serializedObject.FindProperty("hitEffect");
        }

        public override void OnInspectorGUI()
        {


            EditorTools.PropertyField( reloadMode );

            if ((ReloadMode) reloadMode.enumValueIndex != ReloadMode.InfiniteBullets)
                EditorTools.DrawTittle( "Reload Settings" );

            if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.Realistic)
            {
                DrawRealisticReloadSettings();
            }
            else if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.UI)
            {
                DrawUIReloadSettings();
            }

            if ((ReloadMode) reloadMode.enumValueIndex != ReloadMode.InfiniteBullets )
            {
                EditorTools.PropertyField( weaponUI );
            }

            if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.Physics)
            {
                EditorTools.PropertyField( barrelScript );
                EditorTools.PropertyField( reloadAngle );
            }
            if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.PumpActionInfiniteBullets || (ReloadMode) reloadMode.enumValueIndex == ReloadMode.PumpActionRealistic)
            {
                EditorTools.PropertyField( slider );               
            }

            if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.PumpActionRealistic)
            {
                EditorTools.PropertyField( clipSize );
                EditorTools.PropertyField( bulletInsertPoint );
            }

            if ((ReloadMode) reloadMode.enumValueIndex == ReloadMode.Launcher)
            {
                EditorTools.PropertyField( bulletInsertPoint );
            }

            EditorTools.DrawTittle( "Shooting Settings" );
            EditorTools.PropertyField( shootPoint );

            if ((ReloadMode) reloadMode.enumValueIndex != ReloadMode.Launcher)
                EditorTools.PropertyField( bulletPrefab );
            EditorTools.PropertyField(hitEffect);
            EditorTools.PropertyField( weaponHammer );
            EditorTools.PropertyField(shellEjector);
            EditorTools.PropertyField( shootRate );
            EditorTools.PropertyField( isAutomatic );

            if (shootRate.floatValue <= 0.0f)
                shootRate.floatValue = 0.001f;

            //rpm
            EditorGUILayout.LabelField( 60.0f / shootRate.floatValue + " RPM" );

            EditorTools.PropertyField( bulletSpeed );
            EditorTools.PropertyField( dmg );
            EditorTools.PropertyField( hitLayer );
            EditorTools.PropertyField( maxBulletBounceCount );

            EditorTools.DrawTittle( "Hit Force" );
            EditorTools.MinMaxFloatSlider( minHitForce, maxHitForce, MIN_HIT_FORCE, MAX_HIT_FORCE );


            EditorTools.PropertyField( range );
            EditorTools.PropertyField( shootSound );
            EditorTools.PropertyField( muzzleFlash );
            EditorTools.PropertyField( parentMuzzleFlash );
            EditorTools.PropertyField( disableMuzzleWhileNoShooting );
            EditorTools.PropertyField( muzzleLiveTime );
            EditorTools.PropertyField( fireButton );

            EditorTools.DrawTittle( "Recoil Position Settings" );
            EditorTools.MinMaxFloatSlider( minRecoilPositionForce, maxRecoilPositionForce, MIN_POSITION_FORCE, MAX_POSITION_FORCE );
            EditorTools.PropertyField( recoilPositionLimit );

            EditorTools.DrawTittle( "Recoil Rotation Settings" );
            EditorTools.MinMaxFloatSlider( minRecoilRotationForce, maxRecoilRotationForce, MIN_ROTATION_FORCE, MAX_ROTATION_FORCE );
            EditorTools.PropertyField(recoilAngleLimit);

            EditorTools.PropertyField( useSpread );

            if (useSpread.boolValue)
            {
                EditorTools.PropertyField( minSpreadCount );
                EditorTools.PropertyField( maxSpreadCount );
                EditorTools.PropertyField( minSpreadAngle );
                EditorTools.PropertyField( maxSpreadAngle );
            }

            EditorTools.PropertyField( positionLerpSpeed );
            EditorTools.PropertyField( rotationLerpSpeed );

            serializedObject.ApplyModifiedProperties();

        }

       

        private void DrawRealisticReloadSettings()
        {           
            EditorTools.PropertyField( magazineDropZone );
        }

        private void DrawUIReloadSettings()
        {

            EditorTools.PropertyField( clipSize );
            EditorTools.PropertyField( reloadTime );


            if (clipSize.intValue <= 0)
                clipSize.intValue = 1;

            if (reloadTime.floatValue < 0.0f)
                clipSize.floatValue = 0.001f;
        }




    }

}

