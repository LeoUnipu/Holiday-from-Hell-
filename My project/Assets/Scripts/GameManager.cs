using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Paneli")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    [Header("Timer")]
    public TextMeshProUGUI timerTekst;
    public float vrijemePreostalo = 300f;

    [Header("NPC rutina")]
    [Tooltip("Povuci NPC objekt koji ima HodajNasumicno skriptu.")]
    public HodajNasumicno npcRutina;

    [Header("Zvuk")]
    public AudioSource audioSource;
    public AudioClip gameOverZvuk;
    public AudioClip levelCompleteZvuk;

    private bool gameAktivan = true;
    private int potrebanScoreZaPobjedu = 0;

    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        if (npcRutina != null)
        {
            potrebanScoreZaPobjedu =
                npcRutina.DohvatiUkupneBodoveZamki();

            Debug.Log(
                "Potreban score za pobjedu: " +
                potrebanScoreZaPobjedu
            );
        }
        else
        {
            Debug.LogError(
                "NPC rutina nije postavljena u GameManageru."
            );
        }

        OsvjeziTimer();
    }

    private void Update()
    {
        if (!gameAktivan)
        {
            return;
        }

        vrijemePreostalo -= Time.deltaTime;

        if (vrijemePreostalo < 0f)
        {
            vrijemePreostalo = 0f;
        }

        OsvjeziTimer();

        if (ScoreManager.instance != null &&
            potrebanScoreZaPobjedu > 0 &&
            ScoreManager.instance.score >= potrebanScoreZaPobjedu)
        {
            LevelComplete();
            return;
        }

        if (vrijemePreostalo <= 0f)
        {
            GameOver();
        }
    }

    private void OsvjeziTimer()
    {
        if (timerTekst == null)
        {
            return;
        }

        int minute =
            Mathf.FloorToInt(vrijemePreostalo / 60f);

        int sekunde =
            Mathf.FloorToInt(vrijemePreostalo % 60f);

        timerTekst.text =
            string.Format(
                "{0:00}:{1:00}",
                minute,
                sekunde
            );
    }

    public void GameOver()
    {
        if (!gameAktivan)
        {
            return;
        }

        gameAktivan = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (audioSource != null &&
            gameOverZvuk != null)
        {
            audioSource.PlayOneShot(
                gameOverZvuk
            );
        }

        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {
        if (!gameAktivan)
        {
            return;
        }

        gameAktivan = false;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        if (audioSource != null &&
            levelCompleteZvuk != null)
        {
            audioSource.PlayOneShot(
                levelCompleteZvuk
            );
        }

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}