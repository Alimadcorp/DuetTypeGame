using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private InputActions inputActions;
    private InputActions.NormalActions normalActions;
    public Rigidbody2D rb;
    private GameManager game;
    public static Player Instance;
    public SpriteRenderer loseScreenshot;
    public Transform loseScreenshotParent;
    public AudioSource loseScreenshotSound;
    public AudioSource music;
    public TextMeshPro ScoreText;
    public TextMeshPro HighScoreText;
    public TextMeshPro ScoreTextMain;
    public AudioClip jump;
    public LeaderboardManager ldMan;
    public GameObject credObj;
    [Header("Tweakables")]
    public float jumpForce = 1.0f;
    public float gravity = 1.0f;
    public bool ClickEnabled = false;
    public float moveSpeed = 1.0f;
    public int direction = 0;
    private int score = 0;
    public bool initialStop = true;
    public float coolDown = 1f;
    public float initialG;
    public float reflectionPercentage = 0;
    public bool ContinueMode = false;
    public bool lost = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialG = rb.gravityScale;
        rb.gravityScale = 0;
        inputActions = new InputActions();
        normalActions = inputActions.Normal;
        normalActions.Click.started += ctx => Click();
        normalActions.Pause.performed += ctx => Pause();
        Instance = this;
    }
    public void BeginGame()
    {
        ClickEnabled = true;
        Click();
    }
    private void Start()
    {
        game = GameManager.Instance;
        game.over = false;
    }
    private void FixedUpdate()
    {
        if (score < GameManager.Score)
        {
            score += (int)Mathf.Max((float)(GameManager.Score - score) / 10f, 1);
        }
        ScoreTextMain.text = $"Score: {score}";
        if (coolDown > 0)
        {
            coolDown -= Time.fixedUnscaledDeltaTime;
        }
        if (game.gameMode == GameManager.GameMode.Clockwise || game.gameMode == GameManager.GameMode.AntiClockwise)
        {
            switch (direction)
            {
                case 1:
                    rb.AddForce(new Vector3(0, -moveSpeed * game.Speed, 0), ForceMode2D.Force);
                    break;
                case 2:
                    rb.AddForce(new Vector3(-moveSpeed * game.Speed, 0, 0), ForceMode2D.Force);
                    break;
                case 3:
                    rb.AddForce(new Vector3(0, moveSpeed * game.Speed, 0), ForceMode2D.Force);
                    break;
                case 4:
                    rb.AddForce(new Vector3(moveSpeed * game.Speed, 0, 0), ForceMode2D.Force);
                    break;
            }
        }
    }
    private void Click()
    {
        if (Time.timeSinceLevelLoad < 1) return;
        if (ContinueMode) { OnContinue(); ContinueMode = false; };
        if (!ClickEnabled) return;
        if (coolDown > 0) return;
		if(lost) return;
        game.hintVisible = false;
        if (initialStop)
        {
            game.StartGame();
            initialStop = false;
        }
        if (game.gameMode == GameManager.GameMode.Flappy)
        {
            rb.gravityScale = initialG;
        }
       // AudioSource.PlayClipAtPoint(jump, transform.position);
        switch (game.gameMode)
        {
            case GameManager.GameMode.Flappy:
                rb.linearVelocity = new Vector2(0, jumpForce);
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
                GameManager.Instance.AddScore(5, "Flap");
                break;
            case GameManager.GameMode.Clockwise:
                GameManager.Instance.AddScore(1, "Flip");
                SwitchDirection();
                break;
            case GameManager.GameMode.AntiClockwise:
                GameManager.Instance.AddScore(1, "Flip");
                SwitchDirection();
                if (reflectionPercentage < 0.8f)
                {
                    reflectionPercentage += 0.05f;
                }
                else
                {
                    reflectionPercentage = 0.8f;
                }
                break;
        }
    }
    private void SwitchDirection()
    {
        if (direction == 0)
        {
            direction = 2;
            return;
        }
        if (game.gameMode == GameManager.GameMode.Clockwise)
        {
            direction += 1;
            if (direction == 5) { direction = 1; }
        }
        if (game.gameMode == GameManager.GameMode.AntiClockwise)
        {
            direction -= 1;
            if (direction == 0) { direction = 4; }
        }
        Vector2 v = rb.linearVelocity * reflectionPercentage;
        rb.linearVelocity = game.gameMode == GameManager.GameMode.Clockwise ? new Vector2(v.y, -v.x) : new Vector2(-v.y, v.x);
    }
    public void Restart()
    {
		if(lost) return;
        game.over = true;
		lost = true;
        StartCoroutine(RestartCoroutine());
    }
    bool submitt = false;
    private IEnumerator RestartCoroutine()
    {
        ParticleSystem system = GetComponentInChildren<ParticleSystem>();
        string history = PlayerPrefs.GetString("history", DateTime.UtcNow.ToString().Replace(" ", "T"));
        history += $":{GameManager.Score.ToString()}.{(int)(Time.timeSinceLevelLoad*100)}";
        if (Application.platform == RuntimePlatform.WindowsEditor) Debug.Log(history);
        PlayerPrefs.SetString("history", history);
        PlayerPrefs.Save();
        system.gameObject.SetActive(true);
        system.Play();
        int LastHighScore = PlayerPrefs.GetInt("highScore");
        int HighScore = PlayerPrefs.GetInt("highScore");
        if (GameManager.Score > HighScore)
        {
            HighScore = GameManager.Score;
            PlayerPrefs.SetInt("highScore", HighScore);
            PlayerPrefs.Save();
            submitt = true;
        }
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            while (Time.timeScale > 0)
            {
                Time.timeScale = Mathf.Max(Time.timeScale - 1f * Time.fixedUnscaledDeltaTime, 0);
                music.pitch = Time.timeScale;
                yield return new WaitForSecondsRealtime(1f / 60f);
            }
        } else {
			yield return new WaitForSecondsRealtime(0.1f);
			Time.timeScale = 0;
		}
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Sprite sprite = null;
        if (screenshot != null)
        {
            float baseWidth = 720f;
            float baseHeight = 1280f;
            float scaleFactor = Mathf.Sqrt((screenshot.width * screenshot.height) / (baseWidth * baseHeight));
            float baseDPI = 100f;

            sprite = Sprite.Create(
                screenshot,
                new Rect(0, 0, screenshot.width, screenshot.height),
                new Vector2(0.5f, 0.5f),
                baseDPI * scaleFactor
            );
        }
        yield return new WaitForSecondsRealtime(0.1f);
        ContinueMode = true;
        loseScreenshotParent.gameObject.SetActive(true);
        HighScoreText.text = $"High Score: {LastHighScore}";
        loseScreenshot.sprite = sprite;
        loseScreenshotSound.Play();
        yield return new WaitForSecondsRealtime(1f);
        float t = 0;
        while (t < 1)
        {
            t += 1 / 60f;
            ScoreText.text = $"Score: {(int)Mathf.Lerp(0, GameManager.Score, t)}";
            yield return new WaitForSecondsRealtime(1f / 60f);
        }
        ScoreText.text = $"Score: {GameManager.Score}";
        HighScoreText.text = $"High Score: {HighScore}";
    }
    public void OnContinue()
    {
        if (submitt)
        {
            ldMan.OpenLeaderboard(true);
            ldMan.SubmitEntry(GameManager.Score);
            submitt = false;
        }
        else
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Wall")
        {
            game.over = true;
            Restart();
        }
        if (collision.transform.tag == "Blob")
        {
            GameManager.Instance.AddScore(100, "Blob");
            collision.transform.gameObject.GetComponent<Blob>().Collect();
            if (reflectionPercentage < 0.9f)
            {
                reflectionPercentage += 0.1f;
            }
            else
            {
                reflectionPercentage = 0.9f;
            }
            if (collision.transform.gameObject.GetComponent<Blob>().id != "")
            {
                game.AddPowerup(collision.transform.gameObject.GetComponent<Blob>().id);
            }
        }
    }
    public void ResetPosition()
    {
        StartCoroutine(resetPosition());
    }
    private IEnumerator resetPosition()
    {
        Vector3 i = transform.position;
        float t = 0;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(i, new Vector3(game.bigDaddy.transform.position.x - 6, i.y, game.bigDaddy.transform.position.z), t);
            t += Time.fixedUnscaledDeltaTime;
            yield return null;
        }
        transform.position = new Vector3(game.bigDaddy.transform.position.x - 6, i.y, game.bigDaddy.transform.position.z);
        yield return null;
    }
    private void Pause()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}