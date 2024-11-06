using System.Collections;
using UnityEngine;

public class Shroom : MonoBehaviour
{
    public float burstSpeed = 3f;           
    public float burstDuration = 0.5f;
    public float decelerationDuration = 0.5f;
    public float minSpawnRate = 2f;         
    public float maxSpawnRate = 5f;         
    public GameObject shroomPrefab;         
    public float spawnRadius = 5f;
    public AudioSource spawn;
    public AudioSource explode;
    public Animator anim;
    public int maxShroomCount = 12;         

    private Vector3 targetPosition;         
    private bool isBursting = true;         
    private float stateTimer = 0f;          
    private float currentSpeed;             
    private float spawnTimer = 0f;          
    private float spawnRate;                
    private ParticleSystem trailEffect;

    private bool isFollowingZombie = false; 
    private Transform zombieToFollow;       

    void Start()
    {
        FindNewTargetPosition();
        trailEffect = GetComponentInChildren<ParticleSystem>();

        spawnRate = Random.Range(minSpawnRate, maxSpawnRate);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        int activeShroomCount = GameObject.FindGameObjectsWithTag("Shroom").Length;

        if (activeShroomCount < maxShroomCount)
        {
            if (spawnTimer >= spawnRate)
            {
                SpawnNewShroom();
                spawn.Play();
                spawnTimer = 0f;

                spawnRate = Random.Range(minSpawnRate, maxSpawnRate);
            }
        }

        if (isFollowingZombie && zombieToFollow != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, zombieToFollow.position, burstSpeed * Time.deltaTime);
        }
        else
        {
            if (isBursting)
            {
                if (stateTimer >= burstDuration)
                {
                    isBursting = false;
                    stateTimer = 0f;
                }
                else
                {
                    currentSpeed = burstSpeed;
                }
            }
            else
            {
                float decelerationFactor = Mathf.Lerp(1f, 0f, stateTimer / decelerationDuration);
                currentSpeed = burstSpeed * decelerationFactor;

                if (stateTimer >= decelerationDuration)
                {
                    isBursting = true;
                    stateTimer = 0f;
                    FindNewTargetPosition();
                }
            }

            MoveWithCurrentSpeed();
            transform.position = ConstrainToScreen(transform.position);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                FindNewTargetPosition();
            }
        }
    }

    void MoveWithCurrentSpeed()
    {
        float moveStep = currentSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveStep);
    }

    void FindNewTargetPosition()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-5f, 5f);
        targetPosition = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);

        targetPosition = ConstrainToScreen(targetPosition);
    }

    void SpawnNewShroom()
    {
        anim.SetTrigger("Rotate");
        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
        spawnPosition = ConstrainToScreen(spawnPosition);

        if (trailEffect != null)
        {
            trailEffect.Play();
        }

        Instantiate(shroomPrefab, spawnPosition, Quaternion.identity);

        if (trailEffect != null)
        {
            trailEffect.Stop();
        }
    }

    Vector3 ConstrainToScreen(Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(position);
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, 1f);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0f, 1f);
        return Camera.main.ViewportToWorldPoint(screenPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            explode.Play();
            anim.SetBool("Explode", true);

            isFollowingZombie = true;
            zombieToFollow = other.transform;

            StartCoroutine(DestroyShroomAndZombie(other.gameObject));
        }
    }

    private IEnumerator DestroyShroomAndZombie(GameObject zombie)
    {
        yield return new WaitForSeconds(0.8f);

        Destroy(gameObject);
        Destroy(zombie);
    }
}
