using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Balancing Values")]
    public float maxSpeed = 10f;
    public float maxAcceleration;
    public float targetRadius = 0.2f;
    public float slowRadius;
    public float dt = 0.1f;

    private Vector2 currentVelocity;

    protected bool move = true;

    [Header("Destination")]
    [SerializeField]
    protected Vector3 seekPoint = Vector3.zero;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentVelocity = default;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        SteerToTarget();
    }

    public void SteerToTarget()
    {
        Vector2 direction = seekPoint - transform.position;
        float distance = direction.magnitude;

        move = distance > targetRadius;

        if (move)
        {
            float targetSpeed = 0;
            Vector2 targetVelocity = default;

            if(distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * (distance / slowRadius);
            }

            targetVelocity = direction.normalized * targetSpeed;

            Vector2 acceleration = targetVelocity - rb.velocity;
            acceleration /= dt;

            if (acceleration.magnitude > maxAcceleration)
            {
                acceleration = acceleration.normalized * maxAcceleration;
            }

            currentVelocity = new Vector2(
                currentVelocity.x + acceleration.x * Time.deltaTime,
                currentVelocity.y + acceleration.y * Time.deltaTime);

            FaceTarget(currentVelocity);

            rb.velocity = currentVelocity;
            //transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime, transform.position.y + velocity.y * Time.deltaTime, transform.position.z);
        }
    }

    protected void FaceTarget(Vector3 velocity)
    {
        if(velocity.magnitude > 0)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
