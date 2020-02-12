﻿using UnityEngine;
using VRShooterKit.DamageSystem;

namespace VRShooterKit
{
    //this script details about how should a bullet bounce over this object
    public class SurfaceDetails : Damageable
    {
        [SerializeField] private bool bulletsCanBounce = false;
        [SerializeField] private float bulletsSpeedLoseOnBounce = 0.20f;
        [SerializeField] private AudioClip[] hitSoundArray = null;
        [SerializeField] [Range(0.0f , 1.0f)] private float soundvolume = 1.0f;
        [SerializeField] private GameObject[] hitEffectArray = null;
        [SerializeField] private float lifeTime = 0.0f;
        [SerializeField] private bool parentEffect = false;

        public bool BulletsCanBounce { get { return bulletsCanBounce; } }
        public float BulletsSpeedLoseOnBounce { get { return bulletsSpeedLoseOnBounce; } }

        private AudioSource audioSource = null;

        public void CopySettings(SurfaceDetails surface)
        {
            bulletsCanBounce = surface.bulletsCanBounce;
            bulletsSpeedLoseOnBounce = surface.bulletsSpeedLoseOnBounce;
            hitSoundArray = surface.hitSoundArray;
            hitEffectArray = surface.hitEffectArray;
            soundvolume = surface.soundvolume;
            lifeTime = surface.lifeTime;
            parentEffect = surface.parentEffect;
            
        }

        public override void DoDamage(DamageInfo info)
        {
            InstantiateHitEffect(info.hitPoint , Quaternion.LookRotation( info.hitDir ));
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            if (hitSoundArray == null || hitSoundArray.Length == 0)
                return;

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();

                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = Random.Range(soundvolume/2.0f , soundvolume);
            audioSource.pitch = Random.Range(0.7f , 1.0f);
            audioSource.clip = hitSoundArray[ Random.Range(0 , hitSoundArray.Length) ];

            audioSource.Play();

        }

        private void InstantiateHitEffect(Vector3 position , Quaternion rotation)
        {
            if (hitEffectArray == null || hitEffectArray.Length == 0)
                return;

            GameObject go = Instantiate( hitEffectArray[Random.Range( 0, hitEffectArray.Length )], position, rotation);
            Destroy( go, lifeTime );

            if (parentEffect)
            {
                go.transform.parent = transform;
            }
        }

    }
}

