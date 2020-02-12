using UnityEngine;

namespace VRShooterKit.WeaponSystem
{
    public class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] private float visibleTime = 0.05f;

        private float timer = 0.0f;

        private void OnEnable()
        {
            timer = visibleTime;
        }

        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                gameObject.SetActive( false );
            }
        }

    }

}

