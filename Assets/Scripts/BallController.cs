using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float lateralSpeed = 5f;
    [SerializeField] private float maxX = 2.3f;

    [Header("Launch")]
    [SerializeField] private float minForce = 18f;
    [SerializeField] private float maxForce = 45f;

    private Rigidbody rb;

    private bool hasLaunched = false;

    private float forcePercent = 0f;
    private bool increasingForce = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (hasLaunched)
            return;

        MoveBall();
        ChargeForce();
    }

    void MoveBall()
    {
        float movement = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            movement = -1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            movement = 1f;
        }

        Vector3 position = transform.position;

        position.x += movement * lateralSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, -maxX, maxX);

        transform.position = position;
    }

    void ChargeForce()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (increasingForce)
            {
                forcePercent += 60f * Time.deltaTime;

                if (forcePercent >= 100f)
                {
                    forcePercent = 100f;
                    increasingForce = false;
                }
            }
            else
            {
                forcePercent -= 60f * Time.deltaTime;

                if (forcePercent <= 0f)
                {
                    forcePercent = 0f;
                    increasingForce = true;
                }
            }
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            Launch();
        }
    }

    void Launch()
    {
        hasLaunched = true;

        float force = Mathf.Lerp(
            minForce,
            maxForce,
            forcePercent / 100f
        );

        Debug.Log("=================================");
        Debug.Log("LANZAMIENTO");
        Debug.Log("Porcentaje: " + forcePercent);
        Debug.Log("Fuerza calculada: " + force);

        Debug.Log("Rigidbody existe: " + (rb != null));

        if (rb != null)
        {
            Debug.Log("Is Kinematic: " + rb.isKinematic);
            Debug.Log("Mass: " + rb.mass);
            Debug.Log("Linear Damping: " + rb.linearDamping);
            Debug.Log("Velocity antes: " + rb.linearVelocity);

            rb.linearVelocity = Vector3.forward * force;

            Debug.Log("Velocity después: " + rb.linearVelocity);
            Debug.Log("Magnitud: " + rb.linearVelocity.magnitude);
        }

        Debug.Log("=================================");

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.CheckThrow();
        }
    }

    public float GetForcePercent()
    {
        return forcePercent;
    }

    public void ResetBall()
    {
        hasLaunched = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = new Vector3(0f, 0.5f, -4f);
        transform.rotation = Quaternion.identity;

        forcePercent = 0f;
        increasingForce = true;
    }

}