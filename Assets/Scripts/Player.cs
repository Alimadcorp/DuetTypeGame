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
    public TrailRenderer trail;
    public AudioClip jump;
    public LeaderboardManager ldMan;
    public GameObject credObj;
    [Header("Tweakables")]
    public int sinceLastCollect = 100;
    public float jumpForce = 1.0f;
    public bool MouseHeld = false;
    public float gravity = 1.0f;
    public bool ClickEnabled = false;
    public float moveSpeed = 1.0f;
    public int direction = 0;
    private int score = 0;
    public bool initialStop = true;
    public float coolDown = 1f;
    public float fishForce = 1f;
    public float fishHeight = 1f;
    public float fishTimer = 1f;
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
        normalActions.Click.canceled += ctx => UnClick();
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
        if(game.gameMode == GameManager.GameMode.Fish)
        {
            fishTimer -= Time.fixedDeltaTime;
            if (MouseHeld)
            {
                rb.AddForce(Vector3.up * fishForce, ForceMode2D.Force);
            }
            float yDist = Mathf.Abs(transform.position.y - RodNFish.fishY);
            if(yDist < fishHeight)
            {
                RodNFish.instance.fish.GetComponent<SpriteRenderer>().color = Color.white;
                if (fishTimer < 0 && !initialStop)
                {
                    game.AddScore((int)(fishHeight - yDist + 30), "Fishy");
                    fishTimer = 0.5f;
                }
            }
            else
            {
                RodNFish.instance.fish.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
    private void UnClick()
    {
        MouseHeld = false;
    }
    private void Click()
    {
        if (Time.timeSinceLevelLoad < 1) return;
        if (ContinueMode) { OnContinue(); ContinueMode = false; }
        if (!ClickEnabled) return;
        if (coolDown > 0) return;
		if(lost) return;
        sinceLastCollect++;
        if(sinceLastCollect > 2)
        {
            game.AddCombo(0);
        }
        game.hintVisible = false;
        MouseHeld = true;
        if (initialStop)
        {
            game.StartGame();
            initialStop = false;
        }
        if (game.gameMode == GameManager.GameMode.Flappy)
        {
            rb.gravityScale = initialG;
        }
        if (game.gameMode == GameManager.GameMode.Fish)
        {
            rb.gravityScale = initialG * Mathf.Sqrt(game.Speed) * game.SpeedMiniFishFactor;
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
    public void Restart(Collision2D collision)
    {
		if(lost) return;
        game.over = true;
		lost = true;
        StartCoroutine(RestartCoroutine(collision));
    }
    bool submitt = false;
    private IEnumerator RestartCoroutine(Collision2D collision)
    {
        ParticleSystem system = GetComponentInChildren<ParticleSystem>();
        system.startColor = GetComponent<SpriteRenderer>().color;
        string history = PlayerPrefs.GetString("history", DateTime.UtcNow.ToString().Replace(" ", "T"));
        history += $":{GameManager.Score.ToString()}.{(int)(Time.timeSinceLevelLoad*100)}";
        if (Application.platform == RuntimePlatform.WindowsEditor) Debug.Log(history);
        PlayerPrefs.SetString("history", history);
        PlayerPrefs.SetInt("Blobs", game.Blobs);
        PlayerPrefs.Save();
        Logger.Log("GameOver: " + PlayerPrefs.GetString("myUsername") + ", " + GameManager.Score.ToString() + " History: " + history);
        system.gameObject.SetActive(true);
        system.gameObject.transform.position = collision.contacts[0].point;
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
            Restart(collision);
        }
        if (collision.transform.tag == "Blob")
        {
            if (sinceLastCollect == 0)
            {
                game.AddCombo(1f);
            }
            if (sinceLastCollect == 1)
            {
                game.AddCombo(0.5f);
            }
            if (sinceLastCollect == 2)
            {
                game.AddCombo(0.25f);
            }
            if (sinceLastCollect == 3)
            {
                game.AddCombo(0);
            }
            sinceLastCollect = 0;
            game.Blobs++;
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
    public void FishMode(bool Exit)
    {
        if (Exit)
        {
            StartCoroutine(ExitFishMode());
        }
        else
        {
            StartCoroutine(EnterFishMode());
        }
    }
    private IEnumerator ExitFishMode()
    {
        Vector3 ipo = transform.position;
        Vector3 isc = transform.localScale;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        float t = 0;
        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(isc, new Vector3(1, 1, 1), t);
            t += Time.fixedUnscaledDeltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
        GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.2f);
        yield return null;
    }
    private IEnumerator EnterFishMode()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
        Vector3 ipo = transform.position;
        Vector3 isc = transform.localScale;
        float t = 0;
        MouseHeld = false;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(ipo, new Vector3(game.bigDaddy.transform.position.x, ipo.y, game.bigDaddy.transform.position.z), t);
            transform.localScale = Vector3.Lerp(isc, new Vector3(3, 8, 1), t);
            t += Time.fixedUnscaledDeltaTime;
            yield return null;
        }
        MouseHeld = false;
        transform.position = new Vector3(game.bigDaddy.transform.position.x, ipo.y, game.bigDaddy.transform.position.z);
        transform.localScale = new Vector3(3, 8, 1);
        GetComponent<BoxCollider2D>().size = Vector2.one;
        yield return null;
        MouseHeld = false;
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