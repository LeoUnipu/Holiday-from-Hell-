using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreTekst;
    private int score = 0;

    void Awake()
    {
        instance = this;
    }

    public void DodajBod()
    {
        score++;
        scoreTekst.text = "Score: " + score;
    }
}