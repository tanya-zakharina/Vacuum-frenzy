using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameMode { Normal, Endless }
    public GameMode CurrentGameMode { get; private set; } = GameMode.Normal;

    private Vacuum[] vacuums;
    protected Cat cat;
    private Transform pellets;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI livesText;
    private TextMeshProUGUI highScoreText;
    public AudioSource audioSource;
    public AudioClip pelletEatClip;
    public AudioClip powerPelletEatClip;
    public AudioClip catEatenClip;
    public AudioClip catWinClip;
    public AudioClip vacuumEatenClip;
    private bool waitingForRestart = false;

    public int highScore { get; private set; }
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 9;

    private int vacuumMultiplier = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name.ToLower();

        if (name.Contains("endless"))
            SetGameMode(GameMode.Endless);
        else
            SetGameMode(GameMode.Normal);

        vacuums = FindObjectsOfType<Vacuum>();

        pellets = GameObject.Find("Pellets")?.transform;
        gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        livesText = GameObject.Find("LivesText")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        UpdateUI();
        NewRound();
        Time.timeScale = 1f;
        waitingForRestart = false;
    }

    private void Update()
    {
        if (lives <= 0 && gameOverText != null && gameOverText.enabled && !waitingForRestart)
        {
            if ((Keyboard.current?.anyKey.wasPressedThisFrame ?? false) ||
                (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame ?? false) ||
                (Mouse.current?.leftButton.wasPressedThisFrame ?? false))
            {
                waitingForRestart = true;
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        NewGame();

        if (CurrentGameMode == GameMode.Endless)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(9);
        vacuumMultiplier = 1;
    }

    protected void NewRound()
    {
        if (gameOverText != null)
            gameOverText.enabled = false;

        if (pellets != null)
        {
            foreach (Transform pellet in pellets)
                pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        foreach (Vacuum vacuum in vacuums)
            vacuum.ResetState();

        if (cat != null)
            cat.ResetState();

        VacuumShoot.AllowShooting = true;
    }

    private void SetLives(int value)
    {
        lives = value;
        if (livesText != null)
            livesText.text = "x" + lives.ToString();
    }

    protected void SetScore(int value)
    {
        score = value;
        if (scoreText != null)
            scoreText.text = score.ToString().PadLeft(4, '0');

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            if (highScoreText != null)
                highScoreText.text = highScore.ToString().PadLeft(4, '0');
        }
    }

    private void UpdateUI()
    {
        SetScore(score);
        SetLives(lives);
        if (highScoreText != null)
            highScoreText.text = highScore.ToString().PadLeft(4, '0');
    }

    public void CatEaten()
    {
        if (cat != null)
        {
            Projectile.DestroyAllProjectiles();
            cat.DeathSequence();
            if (audioSource != null && catEatenClip != null)
                audioSource.PlayOneShot(catEatenClip);
        }

        SetLives(lives - 1);

        VacuumShoot bossShoot = FindObjectOfType<VacuumShoot>();
        if (bossShoot != null && bossShoot.vacuum != null && bossShoot.vacuum.isBoss)
        {
            VacuumShoot.AllowShooting = false;
        }

        if (lives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (gameOverText != null)
            gameOverText.enabled = true;

        foreach (Vacuum vacuum in vacuums)
            vacuum.gameObject.SetActive(false);

        if (cat != null)
            cat.gameObject.SetActive(false);
    }

    public void VacuumEaten(Vacuum vacuum)
    {
        int points = vacuum.points * vacuumMultiplier;
        SetScore(score + points);
        vacuumMultiplier++;

        if (audioSource != null && vacuumEatenClip != null)
            audioSource.PlayOneShot(vacuumEatenClip);
    }

    public virtual void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        if (audioSource != null && pelletEatClip != null)
            audioSource.PlayOneShot(pelletEatClip);

        if (!HasRemainingPellets())
        {
            if (cat != null)
            {
                cat.WinSequence();
                if (audioSource != null && catWinClip != null)
                    audioSource.PlayOneShot(catWinClip);
            }

            if (CurrentGameMode == GameMode.Endless)
                Invoke(nameof(NewRound), 3f);
            else
                Invoke(nameof(LoadNextLevel), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        foreach (Vacuum vacuum in vacuums)
            vacuum.frightened.Enable(pellet.duration);

        if (audioSource != null && powerPelletEatClip != null)
            audioSource.PlayOneShot(powerPelletEatClip);

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetVacuumMultiplier));
        Invoke(nameof(ResetVacuumMultiplier), pellet.duration);
    }

    protected bool HasRemainingPellets()
    {
        if (pellets == null) return false;

        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    private void ResetVacuumMultiplier()
    {
        vacuumMultiplier = 1;
    }

    protected virtual void LoadNextLevel()
    {
        if (CurrentGameMode == GameMode.Endless)
        {
            NewRound(); // В Endless просто новая волна
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
            NewGame();
        }
    }

    public bool AllPelletsEaten() => !HasRemainingPellets();

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
        UpdateUI();

        MenuHighScoreDisplay menuDisplay = FindObjectOfType<MenuHighScoreDisplay>();
        if (menuDisplay != null)
            menuDisplay.UpdateHighScoreDisplay();
    }

    public void RegisterCat(Cat catInstance)
    {
        cat = catInstance;
        cat.ResetState();
    }

    public void SetGameMode(GameMode mode)
    {
        CurrentGameMode = mode;
    }
}
