using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Flappy, Clockwise, CounterClockwise, Bee, HoldFlap, Random };
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
    private bool isSpawning = false;
    public bool Paused = true;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        Score = 0;
        SetGamemode(GameMode.Flappy);
    }
    void FixedUpdate()
    {
        if (!Paused)
        {
            multiplier += multiplierUpSpeed * Time.fixedDeltaTime;
            if (multiplier >= 1.5f)
            {
                multiText.text = $"x{multiplier:F1}";
            }
            sinceGameModeChange += Time.fixedDeltaTime;
            sinceWallSpawn += Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;
            if (time > 10f)
            {
                AddScore(100, "Time Bonus");
                time = 0;
            }
            if (sinceGameModeChange > 2f)
            {
                switch (gameMode)
                {
                    case GameMode.Flappy:
                        if (sinceWallSpawn > 1f)
                        {
                            sinceWallSpawn = Random.Range(-3f, -1f);
                            sinceWallSpawn += Mathf.Clamp((multiplier - 1f) / 3, 0, 3);
                            flappyWallSpawner.Spawn(wallSpeed * Mathf.Clamp(multiplier / 4, 1, 2), false, opening * Random.Range(0.7f, 1.3f) / Mathf.Clamp(multiplier / 4, 1, 2));
                        }
                        break;
                }
            }
        }
    }
    public void SetGamemode(GameMode _gameMode)
    {
        Invoke("NextGameMode", 25f);
        Invoke("StopSpawning", 22f);
        gameMode = _gameMode;
        switch (gameMode)
        {
            case GameMode.Clockwise:
                BigDaddy.moveDirection = Vector3.zero;
                Paused = true;
                Player.Instance.rb.gravityScale = 0;
                Player.Instance.rb.linearVelocity = Vector2.zero;
                break;
            case GameMode.Flappy:
                BigDaddy.moveDirection = Vector3.right * 8;
                Invoke("StartSpawning", 2f);
                break;
        }
    }
    private void NextGameMode()
    {
        SetGamemode(GameMode.Clockwise);
    }
    public void AddScore(int amt, string source)
    {
        Score += (int)(amt * multiplier);
        if (amt > 5)
        {
            GameObject scr = Instantiate(ScoreFade, scoreFadeSource.position, scoreFadeSource.rotation, bigDaddy.transform);
            scr.GetComponent<TextMeshPro>().text = $"+{(int)(amt * multiplier)} {source}";
        }
        if (amt >= 100)
        {
            AudioSource.PlayClipAtPoint(moreScore, transform.position);
        }
    }
    public void StartGame()
    {
        Paused = false;
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
