using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public enum GameMode { Flappy, Clockwise, AntiClockwise };
    public GameMode gameMode;
    public static GameManager Instance;
    private float sinceGameModeChange = 0f;
    private float sinceWallSpawn = 0f;
    public float time = 0f;
    [Range(0f, 10f)]
    public static int Score = 0;
    public float Speed = 0.25f;
    public float TargetSpeed = 0.25f;
    public int switchThreshold = 1000;
    public float rateOfIncrease;

    [Header("Parameters")]
    public float wallSpeed, opening, speedFactor = 1.0f, multiplier = 1.0f, multiplierUpSpeed = 1f;

    [Header("Object References")]
    public GameObject bigDaddy, ScoreFade;
    public Transform scoreFadeSource;
    public GameObject askUsername;
    public FlappyWallSpawner flappyWallSpawner;
    public TextMeshPro multiText;
    public AudioClip moreScore;
    public AudioSource music;
    public GameObject switchText;
    public TextMeshPro hintText;
    public TextMeshPro titleText;
    public GameObject blob;

    public int boxesPerSpawn = 2;
    public int isLucky = 0;
    public int blobAmt = 0;
    public bool hintVisible = true;
    public bool titleVisible = true;
    public bool Paused = true;
    private int scoreSinceChange = 0;
    public int powerMultiplier = 1;
    public int speedMultiplier = 1;
    public float xs, ys;
    public bool over = false;
    public float alpha = 1;
    public Image[] imgs;

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
    public void AddPowerup(string name)
    {
        if (name == "2x")
        {
            Powerups.instance.AddPowerup(name, 12f);
            powerMultiplier *= 2;
        }
        else if (name == "4x")
        {
            Powerups.instance.AddPowerup(name, 8f);
            powerMultiplier *= 4;
        }
        else if (name == "slow")
        {
            Powerups.instance.AddPowerup("Slow Mo", 10f);
            speedMultiplier++;
        }
        else if (name == "luck")
        {
            Powerups.instance.AddPowerup("Luck", 7.5f);
            isLucky++;
        }
        AddScore(50, "Collected Powerup");
    }
    public void RemovePowerup(string name)
    {
        if (name == "2x")
        {
            powerMultiplier /= 2;
        }
        else if (name == "4x")
        {
            powerMultiplier /= 4;
        }
        else if (name == "Slow Mo")
        {
            speedMultiplier--;
        }
        else if (name == "Luck")
        {
            isLucky--;
        }
    }
    void FixedUpdate()
    {
        UpdateHintVisibility();
        if (!Paused)
        {
            if (!over)
            {
                Time.timeScale = Mathf.Sqrt((speedMultiplier > 1 ? 0.6f : 1f));
                TargetSpeed += Time.fixedDeltaTime * rateOfIncrease;
                Speed = Mathf.Lerp(Speed, TargetSpeed * (speedMultiplier > 1 ? 0.6f : 1f), 0.2f);
                music.pitch = Mathf.Lerp(music.pitch, (speedMultiplier > 1 ? 0.6f : 1f), 0.2f);
            }
            UpdateMultiplier();
            UpdateTimers();
            HandleWallSpawning();
        }
    }

    private void UpdateHintVisibility()
    {
        hintText.color = new Color(1, 1, 1, Mathf.Lerp(hintText.color.a, hintVisible ? 1f : 0f, 0.05f));
        titleText.color = new Color(1, 1, 1, Mathf.Lerp(titleText.color.a, titleVisible ? 1f : 0f, 0.05f));
        if (alpha > 0.00001f)
        {
            alpha = Mathf.Lerp(alpha, titleVisible ? 1f : 0f, 0.05f);
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].color = new Color(imgs[i].color.r, imgs[i].color.g, imgs[i].color.b, alpha);
            }
        }
        else
        {
            alpha = 0;
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].gameObject.GetComponent<Button>().interactable = false;
            }
        }
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
        sinceWallSpawn += Time.fixedDeltaTime * (speedMultiplier > 1 ? 0.6f : 1f);
        time += Time.fixedDeltaTime;
        sinceGameModeChange += Time.fixedDeltaTime;

        if (time > 10f / multiplier)
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
            sinceWallSpawn += Mathf.Clamp((multiplier - 1f) / 3, 0f, 1f);
            flappyWallSpawner.Spawn(
                wallSpeed * Mathf.Clamp(multiplier / 4, 1, 2),
                false,
                opening * Random.Range(0.7f, 1.3f) / Mathf.Clamp(multiplier / 4, 1, 2)
            );
        }
        if ((gameMode == GameMode.AntiClockwise || gameMode == GameMode.Clockwise) && blobAmt < boxesPerSpawn)
        {
            Blob newBlob = Instantiate(blob, RandomTransform(), Quaternion.identity, bigDaddy.transform).GetComponent<Blob>();
            int r = (isLucky > 0) ? 1 : Random.Range(0, 15);
            if (r == 1)
            {
                int random = (int)Random.Range(0, 4);
                if (random == 3 && (isLucky > 0)) random--;
                newBlob.MakePowerup(Blob.intToId(random));
            }
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
        Player.Instance.initialStop = true;
        opening = Mathf.Clamp(opening - 2, 9, 20);

        if (gameMode == GameMode.Clockwise || gameMode == GameMode.AntiClockwise)
        {
            blobAmt = 0;
        }

        gameMode = _gameMode;
        sinceGameModeChange = 0f;

        Blob[] blobs = bigDaddy.GetComponentsInChildren<Blob>();
        TextMeshPro tmp;
        switch (gameMode)
        {

            case GameMode.Clockwise:
                BigDaddy.moveDirection = Vector3.zero;
                Paused = true;
                tmp = Instantiate(switchText, bigDaddy.transform.position + Vector3.down * 3, Quaternion.identity, bigDaddy.transform).GetComponent<TextMeshPro>();
                tmp.text = "Snake\nMode";
                hintVisible = true;
                Player.Instance.reflectionPercentage = 0f;
                hintText.text = "Tap to switch direction";
                foreach (var blob in blobs)
                {
                    if (blob != null) blob.Collect();
                }
                break;
            case GameMode.AntiClockwise:
                BigDaddy.moveDirection = Vector3.zero;
                Paused = true;
                tmp = Instantiate(switchText, bigDaddy.transform.position + Vector3.down * 3, Quaternion.identity, bigDaddy.transform).GetComponent<TextMeshPro>();
                tmp.text = "Snake\nMode\n(Anti Clockwise)";
                hintVisible = true;
                Player.Instance.reflectionPercentage = 0f;
                hintText.text = "Tap to switch directions";
                foreach (var blob in blobs)
                {
                    if (blob != null) blob.Collect();
                }
                break;
            case GameMode.Flappy:
                BigDaddy.moveDirection = Vector3.right * 13;
                Paused = true;
                Player.Instance.initialStop = true;
                Player.Instance.ResetPosition();
                tmp = Instantiate(switchText, bigDaddy.transform.position + Vector3.up * 3, Quaternion.identity, bigDaddy.transform).GetComponent<TextMeshPro>();
                tmp.text = "Flappy\nMode";
                hintText.text = "Tap to jump";
                hintVisible = true;
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
        boxesPerSpawn++;
        int current = (int)gameMode;
        int next;
        do { next = Random.Range(0, 3); }
        while (next == current);
        GameMode nextMode = (GameMode)next;
        SetGamemode(nextMode);
    }

    public void AddScore(int amt, string source)
    {
        if (over) return;
        if (source == "Blob") blobAmt--;
        titleVisible = false;
        int scoreToAdd = (int)(amt * multiplier * powerMultiplier);
        Score += scoreToAdd;
        scoreSinceChange += (int)(amt * multiplier);

        if (amt > 5)
        {
            GameObject scr = Instantiate(ScoreFade, scoreFadeSource.position, scoreFadeSource.rotation, bigDaddy.transform);
            scr.GetComponent<TextMeshPro>().text = $"+{scoreToAdd} {source}";
        }

        if (amt >= 100 && moreScore != null)
        {
            AudioSource.PlayClipAtPoint(moreScore, transform.position, 0.2f);
        }

        if (scoreSinceChange > switchThreshold * multiplier)
        {
            scoreSinceChange = 0;
            if (gameMode == GameMode.Flappy)
            {
                DestroyAllPipes();
            }
            NextGameMode();
        }
    }
    void DestroyAllPipes()
    {
        foreach (FlappyWall wall in bigDaddy.GetComponentsInChildren<FlappyWall>())
        {
            wall.Blow();
        }
    }
    public void StartGame()
    {
        Paused = false;
        Player.Instance.rb.gravityScale = gameMode == GameMode.Flappy ? Player.Instance.initialG : 0;
    }
}