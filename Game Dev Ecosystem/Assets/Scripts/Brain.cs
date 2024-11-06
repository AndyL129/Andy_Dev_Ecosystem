using System;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public float normalSpeed = 2f;
    public float escapeSpeed = 5f;
    public float detectionRadius = 5f;
    private Vector3 targetPosition;
    private bool isEscaping = false;
    private GameObject zombie;
    public GameObject sunPrefab;
    public AudioSource kiss;
    public Animator anim;

    void Start()
    {
        FindNewTargetPosition();
        zombie = GameObject.FindGameObjectWithTag("Zombie");
    }

    void Update()
    {
        if (isEscaping)
        {
            anim.SetBool("Chasing", true);
            UpdateEscapeDirection();
            EscapeFromZombie();
        }
        else
        {
            anim.SetBool("Chasing", false);
            MoveRandomly();
            DetectZombie();
        }

        transform.position = ConstrainToScreen(transform.position);
    }

    void FindNewTargetPosition()
    {
        float screenX = UnityEngine.Random.Range(-8f, 8f);
        float screenY = UnityEngine.Random.Range(-4f, 4f);
        targetPosition = new Vector3(screenX, screenY, transform.position.z);
    }

    void MoveRandomly()
    {
        MoveAndRotate(targetPosition, normalSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            FindNewTargetPosition();
            kiss.Play();
            anim.SetTrigger("Kiss");
            Instantiate(sunPrefab, this.transform.position, Quaternion.identity);
        }
    }

    void DetectZombie()
    {
        if (zombie != null && Vector3.Distance(transform.position, zombie.transform.position) < detectionRadius)
        {
            isEscaping = true;
        }
    }

    void UpdateEscapeDirection()
    {
        if (zombie != null)
        {
            Vector3 directionAwayFromZombie = (transform.position - zombie.transform.position).normalized;

            Vector3 potentialPosition = transform.position + directionAwayFromZombie * 2f;
            targetPosition = ConstrainToScreen(potentialPosition);
        }
    }

    void EscapeFromZombie()
    {
        if (zombie != null)
        {
            MoveAndRotate(targetPosition, escapeSpeed);

            if (Vector3.Distance(transform.position, zombie.transform.position) > detectionRadius)
            {
                isEscaping = false;
                FindNewTargetPosition();
            }
        }
    }

    void MoveAndRotate(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        Vector3 direction = target - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    Vector3 ConstrainToScreen(Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(position);
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0.05f, 0.95f);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0.05f, 0.95f);
        return Camera.main.ViewportToWorldPoint(screenPosition);
    }
}
