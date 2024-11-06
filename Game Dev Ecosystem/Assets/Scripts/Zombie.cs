using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float normalSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;
    public float breatheInterval = 10f;
    public float breatheSpeed = 2f;
    private Vector3 targetPosition;
    private GameObject brain;
    private bool isChasing = false;
    private bool isBreathing = false;
    private float breatheTimer = 0f;
    public AudioSource bite;
    public AudioSource breatheSound;
    public GameObject zombiePrefab;
    public Animator anim;

    void Start()
    {
        brain = GameObject.FindGameObjectWithTag("Brain");
        FindNewTargetPosition();
    }

    void Update()
    {
        breatheTimer += Time.deltaTime;

        if (isBreathing)
        {
            BreatheAction();
        }
        else if (breatheTimer >= breatheInterval)
        {
            StartBreathing();
        }
        else if (isChasing)
        {
            anim.SetBool("Chasing", true);
            ChaseBrain();
        }
        else
        {
            anim.SetBool("Chasing", false);
            MoveRandomly();
            DetectBrain();
        }

        ConstrainToScreen();
    }

    void StartBreathing()
    {
        isBreathing = true;
        breatheTimer = 0f;
        anim.SetBool("Chasing", false);
    }

    void BreatheAction()
    {
        transform.rotation = Quaternion.Euler(0, 0, -90);

        transform.position += Vector3.up * breatheSpeed * Time.deltaTime;

        if ((this.transform.position.y) >= 7f)
        {
            breatheSound.Play();
            isBreathing = false;

            transform.rotation = Quaternion.Euler(0, 0, 0);

            FindNewTargetPosition();
        }
    }

    void FindNewTargetPosition()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
        float screenX = Random.Range(bottomLeft.x, topRight.x);
        float screenY = Random.Range(bottomLeft.y, topRight.y);
        targetPosition = new Vector3(screenX, screenY, transform.position.z);
    }

    void MoveRandomly()
    {
        MoveAndRotate(targetPosition, normalSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            FindNewTargetPosition();
        }
    }

    void DetectBrain()
    {
        if (brain != null && Vector3.Distance(transform.position, brain.transform.position) < detectionRadius)
        {
            isChasing = true;
        }
    }

    void ChaseBrain()
    {
        if (brain != null)
        {
            MoveAndRotate(brain.transform.position, chaseSpeed);
            if (Vector3.Distance(transform.position, brain.transform.position) > detectionRadius)
            {
                isChasing = false;
                FindNewTargetPosition();
            }
        }
        else
        {
            isChasing = false;
            FindNewTargetPosition();
        }
    }

    void MoveAndRotate(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Brain"))
        {
            bite.Play();
            Instantiate(zombiePrefab, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }

    void ConstrainToScreen()
    {
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0.05f, 0.95f);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(screenPosition);
    }
}
