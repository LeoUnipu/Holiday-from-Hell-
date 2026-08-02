using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreTekst;
    public Image scoreFill;

    public int score = 0;
    public int maksimalniScore = 100;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OsvjeziScore();
    }

    public void DodajBod()
    {
        score++;
        OsvjeziScore();
    }

    public void DodajBodove(int koliko)
    {
        score += koliko;
        OsvjeziScore();
    }

    private void OsvjeziScore()
    {
        if (scoreTekst != null)
        {
            scoreTekst.text = "Score: " + score;
        }

        if (scoreFill != null)
        {
            scoreFill.fillAmount =
                Mathf.Clamp01(
                    (float)score / maksimalniScore
                );
        }
    }
}