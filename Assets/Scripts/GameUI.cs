using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text frameText;
    [SerializeField] private TMP_Text throwText;
    [SerializeField] private Slider forceBar;

    [Header("References")]
    [SerializeField] private BallController ball;

    void Start()
    {
        forceBar.minValue = 0f;
        forceBar.maxValue = 100f;
        forceBar.value = 0f;
    }

    void Update()
    {
        UpdateForceBar();
        UpdateGameInfo();
    }

    void UpdateForceBar()
    {
        if (ball != null && forceBar != null)
        {
            forceBar.SetValueWithoutNotify(
                ball.GetForcePercent()
            );
        }
    }

    void UpdateGameInfo()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return;

        if (scoreText != null)
            scoreText.text = "SCORE: " + gameManager.Score;

        if (frameText != null)
            frameText.text =
                "FRAME " +
                gameManager.CurrentFrame +
                " / 10";

        if (throwText != null)
            throwText.text =
                "THROW " +
                gameManager.CurrentThrow +
                " / 2";
    }
}