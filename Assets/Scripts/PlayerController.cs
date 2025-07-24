using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public bool hasPowerup = false;
    private float powerupStrength = 15.0f;
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public GameObject powerupIndicator;
    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;
    public float hangTime = 0.5f;
    public float smashSpeed = 50.0f;
    public float explosionForce = 100.0f;
    public float explosionRadius = 10.0f;
    private bool smashing = false;
    private float floorY = 0.0f;
    public bool gameOver = false;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("ReplayButton").GetComponent<GameManager>();
        gameManager.gameObject.SetActive(false);
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }
        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
        if (transform.position.y < -10)
        {
            gameManager.GameOver();
            gameOver = true;
            // Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        // Cycle through all enenies.
        var enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            // Apple an explosion force that originates from our position.
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }
        // Store the y position before taking off
        floorY = transform.position.y;
        // Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            // move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        // Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }
        // We are no longer smashing, so set the boolean to false.
        smashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
            if (currentPowerUp == PowerUpType.Pushback)
            {
                Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = collision.transform.position - transform.position;
                enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
