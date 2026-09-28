using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] private BallController ball;

    private int score = 0;
    private int currentFrame = 1;
    private int currentThrow = 1;

    private int pinsDownThisFrame = 0;

    private bool checkingThrow = false;

    public int Score => score;
    public int CurrentFrame => currentFrame;
    public int CurrentThrow => currentThrow;

    public void CheckThrow()
    {
        if (!checkingThrow)
        {
            StartCoroutine(CheckPinsAfterThrow());
        }
    }

    IEnumerator CheckPinsAfterThrow()
    {
        checkingThrow = true;

        Rigidbody ballRb = ball != null
            ? ball.GetComponent<Rigidbody>()
            : null;

        if (ballRb == null)
        {
            Debug.LogError("GameManager: No se encontró el Rigidbody de la bola.");
            checkingThrow = false;
            yield break;
        }

        // Esperamos a que la bola realmente empiece a moverse.
        float launchTimeout = 2f;
        float launchTimer = 0f;

        while (ballRb.linearVelocity.magnitude < 0.5f)
        {
            launchTimer += Time.deltaTime;

            if (launchTimer >= launchTimeout)
            {
                Debug.LogWarning("La bola no comenzó a moverse.");
                checkingThrow = false;
                yield break;
            }

            yield return null;
        }

        Debug.Log("La bola comenzó a moverse.");

        // Ahora esperamos a que se detenga.
        float stoppedTime = 0f;
        float maxThrowTime = 10f;
        float throwTimer = 0f;

        while (throwTimer < maxThrowTime)
        {
            throwTimer += Time.deltaTime;

            float speed = ballRb.linearVelocity.magnitude;

            if (speed < 0.2f)
            {
                stoppedTime += Time.deltaTime;

                if (stoppedTime >= 1f)
                {
                    break;
                }
            }
            else
            {
                stoppedTime = 0f;
            }

            yield return null;
        }

        if (throwTimer >= maxThrowTime)
        {
            Debug.LogWarning("La bola tardó demasiado en detenerse.");
        }

        // Damos tiempo a las colisiones de los bolos.
        yield return new WaitForSeconds(0.5f);

        GameObject[] pins = GameObject.FindGameObjectsWithTag("Pin");

        int standingPins = 0;

        foreach (GameObject pin in pins)
        {
            if (pin == null)
                continue;

            Rigidbody rb = pin.GetComponent<Rigidbody>();

            if (rb != null)
            {
                if (Vector3.Dot(pin.transform.up, Vector3.up) > 0.7f)
                {
                    standingPins++;
                }
            }
        }

        int knockedDown = 10 - standingPins;

        score += knockedDown;

        Debug.Log("Pinos derribados: " + knockedDown);
        Debug.Log("Score actual: " + score);

        if (currentThrow == 1 && knockedDown == 10)
        {
            Debug.Log("🎳 CHUZA!");

            currentFrame++;
            currentThrow = 1;

            ResetFrame();
        }
        else if (currentThrow == 1)
        {
            pinsDownThisFrame = knockedDown;
            currentThrow = 2;

            ResetBallOnly();
        }
        else
        {
            int totalFramePins = pinsDownThisFrame + knockedDown;

            if (totalFramePins >= 10)
            {
                Debug.Log("🔥 SPARE!");
            }
            else
            {
                Debug.Log("Frame abierto: " + totalFramePins);
            }

            currentFrame++;
            currentThrow = 1;

            ResetFrame();
        }

        checkingThrow = false;

        if (currentFrame > 10)
        {
            Debug.Log("🎉 PARTIDA TERMINADA");
        }
    }

    void ResetBallOnly()
    {
        if (ball != null)
        {
            ball.ResetBall();
        }
    }

    void ResetFrame()
    {
        ResetPins();

        if (ball != null)
        {
            ball.ResetBall();
        }

        pinsDownThisFrame = 0;
    }

    void ResetPins()
    {
        GameObject[] pins = GameObject.FindGameObjectsWithTag("Pin");

        foreach (GameObject pin in pins)
        {
            if (pin == null)
                continue;

            Rigidbody rb = pin.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            pin.transform.rotation = Quaternion.identity;
        }
    }
}