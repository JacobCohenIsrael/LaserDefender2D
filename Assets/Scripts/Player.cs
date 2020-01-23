using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("Player Attributes")]
    [SerializeField] float horizontalMoveSpeed = 10f;
    [SerializeField] float verticalMoveSpeed = 10f;
    [SerializeField] float boundryPadding = 1f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFireRate = 0.15f;
    [SerializeField] float health = 100f;

    [Header("User Interface")]
    [SerializeField] Slider playerHealthBar;

    [Header("Projectile")]
    [SerializeField] Projectile laserPrefab;

    [Header("Sounds")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] [Range(0,1)] private float shootSoundVolume = 0.7f;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] private float deathSoundVolume = 0.7f;


    Coroutine fireHandler;
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBounds();
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        playerHealthBar.value = health / maxHealth;
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            fireHandler = StartCoroutine(FireContinuously());
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(fireHandler);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            var laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFireRate);
        }
    }

    private void Move()
    {
        MoveX();
        MoveY();
    }

    private void MoveX()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * horizontalMoveSpeed;
        var newXPosition = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        transform.position = new Vector2(newXPosition, transform.position.y);
    }

    private void MoveY()
    {
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * verticalMoveSpeed;
        var newYPosition = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(transform.position.x, newYPosition);
    }

    private void SetUpMoveBounds()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + boundryPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - boundryPadding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + boundryPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).y - boundryPadding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (null != damageDealer)
        {
            damageDealer.OnHit();
        }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.Damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSoundVolume);
        Level level = FindObjectOfType<Level>();
        level.LoadGameOver();
    }
}
