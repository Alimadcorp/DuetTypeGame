using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Flappy, Clockwise };
    public GameMode gameMode;
    public static GameManager Instance;
    private float sinceGameModeChange = 0f;
    private float sinceWallSpawn = 0f;
    private float time = 0f;
    public static int Score = 0;

    [Header("Parameters")]
    public float wallSpeed, opening, speedFactor = 1.0f, multiplier = 1.0f, multiplierUpSpeed = 1f;

    [Header("Object References")]
    public GameObject bigDaddy, ScoreFade;
    public Transform scoreFadeSource;
    public FlappyWallSpawner flappyWallSpawner;
    public TextMeshPro multiText;
    public AudioClip moreScore;
    public GameObject switchText;
    public TextMeshPro hintText;
    public GameObject blob;

    private int blobAmt = 0;
    public bool hintVisible = true;
    private bool isSpawning = false;
    public bool Paused = true;
    private int scoreSinceChange = 0;
    public float xs, ys;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
        Score = 0;
    }

    private void Start()
    {
        SetGamemode(gameMode);
    }

    void FixedUpdate()
    {
        UpdateHintVisibility();

        if (!Paused)
        {
            UpdateMultiplier();
            UpdateTimers();
            HandleWallSpawning();
        }
    }

    private void UpdateHintVisibility()
    {
        hintText.color = new Color(1, 1, 1, Mathf.Lerp(hintText.color.a, hintVisible ? 1f : 0f, 0.05f));
    }

    private void UpdateMultiplier()
    {
        multiplier += multiplierUpSpeed * Time.fixedDeltaTime;
        if (multiplier >= 1.5f)
        {
            multiText.text = $"x{multiplier:F1}";
        }
    }

    private void UpdateTimers()
    {
        sinceWallSpawn += Time.fixedDeltaTime;
        time += Time.fixedDeltaTime;
        sinceGameModeChange += Time.fixedDeltaTime;

        if (time > 10f)
        {
            AddScore(100, "Time Bonus");
            time = 0;
        }
    }

    private void HandleWallSpawning()
    {
        if (gameMode == GameMode.Flappy && sinceGameModeChange > 2f && sinceWallSpawn > 1f)
        {
            sinceWallSpawn = Random.Range(-3f, -1f);
            sinceWallSpawn += Mathf.Clamp((multiplier - 1f) / 3, 0, 3);
            flappyWallSpawner.Spawn(
                wallSpeed * Mathf.Clamp(multiplier / 4, 1, 2),
                false,
                opening * Random.Range(0.7f, 1.3f) / Mathf.Clamp(multiplier / 4, 1, 2)
            );
        }
        if (gameMode == GameMode.Clockwise && blobAmt < 2)
        {
            Instantiate(blob, RandomTransform(), Quaternion.identity, bigDaddy.transform);
            blobAmt++;
        }
    }

    private Vector3 RandomTransform()
    {
        return (new Vector3(Random.Range(-xs, xs), Random.Range(-ys, ys), -0.5f) + bigDaddy.transform.position);
    }

    public void SetGamemode(GameMode _gameMode)
    {
        Player.Instance.coolDown = 1f;
        Player.Instance.direction = 0;
        Player.Instance.rb.gravityScale = 0;
        Player.Instance.rb.linearVelocity = Vector2.zero;

        if (gameMode == GameMode.Clockwise)
        {
            blobAmt = 0;
        }

        gameMode = _gameMode;
        sinceGameModeChange = 0f;

        TextMeshPro tmp;
        switch (gameMode)
        {
            case GameMode.Clockwise:
                BigDaddy.moveDirection = Vector3.zero;
                Paused = true;
                tmp = Instantiate(switchText, bigDaddy.transform.position + Vector3.down * 3, Quaternion.identity, bigDaddy.transform).GetComponent<TextMeshPro>();
                tmp.text = "Round Mode";
                hintVisible = true;
                isSpawning = true;
                Player.Instance.reflectionPercentage = 0;
                hintText.text = "Tap to switch directions";
                break;

            case GameMode.Flappy:
                BigDaddy.moveDirection = Vector3.right * 13;
                Paused = true;
                Player.Instance.initialStop = true;
                tmp = Instantiate(switchText, bigDaddy.transform.position + Vector3.up * 3, Quaternion.identity, bigDaddy.transform).GetComponent<TextMeshPro>();
                tmp.text = "Flappy Mode";
                hintText.text = "Tap to jump";
                hintVisible = true;
                Invoke("StartSpawning", 2f);
                Blob[] blobs = bigDaddy.GetComponentsInChildren<Blob>();
                foreach (var blob in blobs)
                {
                    if (blob != null) blob.Collect();
                }
                break;
        }
    }

    private void NextGameMode()
    {
        sinceGameModeChange = 0f;
        scoreSinceChange = 0;
        GameMode nextMode = gameMode == GameMode.Flappy ? GameMode.Clockwise : GameMode.Flappy;
        SetGamemode(nextMode);
    }

    public void AddScore(int amt, string source)
    {
        if (source == "Blob") blobAmt--;

        int scoreToAdd = (int)(amt * multiplier);
        Score += scoreToAdd;
        scoreSinceChange += scoreToAdd;

        if (amt > 5)
        {
            GameObject scr = Instantiate(ScoreFade, scoreFadeSource.position, scoreFadeSource.rotation, bigDaddy.transform);
            scr.GetComponent<TextMeshPro>().text = $"+{scoreToAdd} {source}";
        }

        if (amt >= 100 && moreScore != null)
        {
            AudioSource.PlayClipAtPoint(moreScore, transform.position);
        }

        if (scoreSinceChange > 1000 * multiplier)
        {
            NextGameMode();
        }
    }

    public void StartGame()
    {
        Paused = false;
        Player.Instance.rb.gravityScale = gameMode == GameMode.Flappy ? Player.Instance.initialG : 0;
    }

    private void StartSpawning()
    {
        isSpawning = true;
    }

    private void StopSpawning()
    {
        isSpawning = false;
    }
}