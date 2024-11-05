using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerCombat : MonoBehaviour
{
    public float health;
    public float maxHealth;
    [SerializeField] private bool flashOnHit;
    [SerializeField] private float flashTime = 0.2f;
    [SerializeField] Color flashcolor = Color.white;
    public GameOverScreen gameOverScreen;

    [Header("Invincibility")]
    [SerializeField] private bool hasIFrames = false;
    public bool invincible = false;
    [SerializeField] private int numberOfFlashes = 5;
    [SerializeField] private float invincibilityTime = 1.5f;

    [Header("Magic")]
    public Transform bulletSpawnPoint;
    public GameObject bullet;
    public float fireCooldown;
    public float timeToNextFire;
    public float diveRadius;
    public LayerMask enemyLayer;
    public AudioSource shootSound;

    [Header("Camera")]
    public float screenShakeFactor = 0.1f;

    [Header("Effects")]
    public Transform diveEffectSpawnPoint;
    public GameObject diveEffect;
    public AudioSource hitSound;
    public AudioSource diveHitSound;
    public Light2D bigDiveLight;

    public Animator animator;
    public SpriteRenderer entitySprite;
    private Material entityMaterial;
    private CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        //animator = GetComponent<Animator>();
        //entitySprite = GetComponent<SpriteRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        entityMaterial = entitySprite.material;
    }

    private void CameraShake(float amount)
    {
        impulseSource.GenerateImpulseWithForce(amount);
    }

    public void takeHit(float damage)
    {
        if (!invincible)
        {
            health -= damage;
            CameraShake(damage * screenShakeFactor);
            hitSound.Play();

            if (hasIFrames)
            {
                StartCoroutine("FlashInvincible");
            }
            else if (flashOnHit)
            {
                StartCoroutine("DamageFlash");
            }
        }
        if (health <= 0)
        {
            gameOver();
        }
    }
    public void heal(float amount)
    {
        health = Mathf.Clamp(health+amount, 0, maxHealth);
    }

    public void gameOver()
    {
        gameOverScreen.Setup();
        //end game
    }

    // Update is called once per frame
    void Update()
    {
        timeToNextFire -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Z) && timeToNextFire < 0)
        {
            GameObject bulletObj = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            shootSound.Play();
            animator.Play(Animator.StringToHash("Player_Attack"));
            //bulletObj.GetComponent<Bullet>().isPlayerBullet = true;
            timeToNextFire = fireCooldown;
        }

        if (bigDiveLight.intensity >= 0f)
        {
            bigDiveLight.intensity -= 0.1f;
        }
    }

    public void diveAttack()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, diveRadius, enemyLayer);

        Instantiate(diveEffect, diveEffectSpawnPoint.position, Quaternion.identity);

        diveHitSound.Play();

        bigDiveLight.intensity = 10f;

        // Loop through each enemy and apply damage
        foreach (Collider2D enemy in enemiesInRange)
        {
            // Try to access an enemy health script to apply damage
            BasicEnemyMovement basicEnemy = enemy.GetComponent<BasicEnemyMovement>();
            if (basicEnemy != null)
            {
                basicEnemy.die();
            }

            print("Dive attack hit " + enemy.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, diveRadius);
    }

    private IEnumerator DamageFlash()
    {
        setFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            setFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private IEnumerator FlashInvincible()
    {
        setFlashColor();

        float waitTime = (invincibilityTime / numberOfFlashes) / 2f;
        invincible = true;
        for (int temp = 0; temp < numberOfFlashes; temp++)
        {
            setFlashAmount(0.5f);
            yield return new WaitForSeconds(waitTime);
            setFlashAmount(0f);
            yield return new WaitForSeconds(waitTime);
        }
        invincible = false;
    }

    private void setFlashColor()
    {
        entityMaterial.SetColor("_FlashColor", flashcolor);
    }

    private void setFlashAmount(float amount)
    {
        entityMaterial.SetFloat("_FlashAmount", amount);
    }
}
