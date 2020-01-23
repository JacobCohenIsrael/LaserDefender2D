using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] float health = 100;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float explosionDuration = 1.0f;

    [Header("Prefabs")]
    [SerializeField] Projectile enemyProjectile;
    [SerializeField] GameObject explosionVFX;

    [Header("Sounds")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] [Range(0, 1)] private float shootSoundVolume = 0.75f;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] [Range(0,1)] private float explosionSoundVolume = 0.75f;

    [Header("User Interface")]
    [SerializeField] Slider enemyHealthBar;

    float shotCounter;
    float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        shotCounter = GetRandomTimeBetweenShots();
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        CountDownToShoot();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        enemyHealthBar.value = health / maxHealth;
    }

    private float GetRandomTimeBetweenShots()
    {
        return UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    private void CountDownToShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
        }
    }

    private void Fire()
    {
        var laser = Instantiate(enemyProjectile, transform.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);
        shotCounter = GetRandomTimeBetweenShots();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (null != damageDealer)
        {
            damageDealer.OnHit();
            ProcessHit(damageDealer);
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.Damage;
        if (health <= 100)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        var explosion = Instantiate(explosionVFX, transform.position, transform.rotation);
        Destroy(explosion, explosionDuration);
        AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position, explosionSoundVolume);
    }
}
