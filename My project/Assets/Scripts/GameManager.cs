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
    public AudioSource audioSource;
    public AudioClip gameOverZvuk;
    public AudioClip levelCompleteZvuk;

    void Update()
    {
        if (!gameAktivan) return;

        vrijemePreostalo -= Time.deltaTime;

        // Prikaži timer na ekranu
        int minute = Mathf.FloorToInt(vrijemePreostalo / 60f);
        int sekunde = Mathf.FloorToInt(vrijemePreostalo % 60f);
        timerTekst.text = string.Format("{0:00}:{1:00}", minute, sekunde);

        // Igrač je preživio 5 minuta
        if (vrijemePreostalo <= 0 || ScoreManager.instance.score >= 20)
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
    audioSource.PlayOneShot(gameOverZvuk); // NOVO
    Time.timeScale = 0f;
}

    public void LevelComplete()
{
    gameAktivan = false;
    if (levelCompletePanel != null)
        levelCompletePanel.SetActive(true);
    audioSource.PlayOneShot(levelCompleteZvuk); // NOVO
    Time.timeScale = 0f;
}

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}