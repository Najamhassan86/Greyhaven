using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float detectionRadius = 12f;
    public float fieldOfView = 90f;
    public float attackDistance = 2f; //Distance at which enemy starts attacking

    private Rigidbody rb;
    private Animator animator;
    private bool isChasing = false;
    private bool isAttacking = false;
    private Vector3 chaseDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        animator = GetComponent<Animator>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        bool shouldChase = false;
        bool shouldAttack = false;

        // Check if in attack distance
        if (distance <= attackDistance)
        {
            // Enemy is in attack range
            shouldAttack = true;
            isAttacking = true;
            
            // Still rotate toward player when attacking
            Vector3 dir = toPlayer.normalized;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
            }
        }
        else
        {
            // Not in attack distance, check if should chase
            isAttacking = false;
            
            // Check distance
            if (distance <= detectionRadius)
            {
                // Check field of view
                Vector3 dir = toPlayer.normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle <= fieldOfView * 0.5f)
                {
                    // Set chase mode
                    dir.y = 0f;
                    chaseDirection = dir;
                    shouldChase = true;

                    // Smooth rotate toward player
                    if (dir.sqrMagnitude > 0.0001f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
                    }
                }
            }
        }

        isChasing = shouldChase;

        // Drive Animator bools (if we have an Animator)
        if (animator != null)
        {
            animator.SetBool("isChasing", isChasing);
            animator.SetBool("isAttacking", isAttacking);
        }
    }

    void FixedUpdate()
    {
        // Only move if chasing and not attacking
        if (isChasing && !isAttacking)
        {
            rb.MovePosition(rb.position + chaseDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
