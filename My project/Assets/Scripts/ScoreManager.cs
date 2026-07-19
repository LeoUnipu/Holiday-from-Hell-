using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreTekst;
    public int score = 0;

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
    }
}