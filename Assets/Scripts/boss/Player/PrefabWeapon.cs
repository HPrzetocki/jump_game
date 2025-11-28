using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabWeapon : MonoBehaviour
{
    [SerializeField] private AudioClip damageSoundClip;

    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Strzelanie")]
    public float fireCooldown = 0.3f;  // czas między strzałami w sekundach
    private float nextFireTime = 0f;

    void Update()
    {
        // Fire1 = domyślnie LPM / Ctrl
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            SoundFXManager.instance.PlaySoundFXClip(damageSoundClip, transform, 1f);

            // ustaw kolejny możliwy czas strzału
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
