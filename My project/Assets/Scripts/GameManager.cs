using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI timerTekst;

    private float vrijemePreostalo = 300f; // 5 minuta
    private bool gameAktivan = true;

    void Update()
    {
        if (!gameAktivan) return;

        vrijemePreostalo -= Time.deltaTime;

        // Prikaži timer na ekranu
        int minute = Mathf.FloorToInt(vrijemePreostalo / 60f);
        int sekunde = Mathf.FloorToInt(vrijemePreostalo % 60f);
        timerTekst.text = string.Format("{0:00}:{1:00}", minute, sekunde);

        // Igrač je preživio 5 minuta
        if (vrijemePreostalo <= 0)
        {
            vrijemePreostalo = 0;
            LevelComplete();
        }
    }

    public void GameOver()
    {
        gameAktivan = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {
        gameAktivan = false;
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}