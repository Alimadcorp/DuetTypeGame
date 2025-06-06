using System.Collections;
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
    public TextMeshPro ScoreTextMain;
    public AudioClip jump;
    [Header("Tweakables")]
    public float jumpForce = 1.0f;
    public float gravity = 1.0f;
    public float moveSpeed = 1.0f;
    public int direction = 0;
    private int score = 0;
    public bool initialStop = true;
    public float coolDown = 1f;
    public float initialG;
    public float reflectionPercentage = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialG = rb.gravityScale;
        rb.gravityScale = 0;
        inputActions = new InputActions();
        normalActions = inputActions.Normal;
        normalActions.Click.performed += ctx => Click();
        normalActions.Pause.performed += ctx => Pause();
        Instance = this;
    }
    private void Start()
    {
        game = GameManager.Instance;
    }
    private void FixedUpdate()
    {
        if(score < GameManager.Score)
        {
            score += (int)Mathf.Max((float)(GameManager.Score - score)/10f, 1);
        }
        ScoreTextMain.text = $"Score: {score}";
        if (coolDown > 0) {
            coolDown -= Time.fixedUnscaledDeltaTime;
        }
        
        switch (direction)
        {
            case 1:
                rb.AddForce(new Vector3(0, moveSpeed, 0), ForceMode2D.Force);
                break;
            case 2:
                rb.AddForce(new Vector3(moveSpeed, 0, 0), ForceMode2D.Force);
                break;
            case 3:
                rb.AddForce(new Vector3(0, -moveSpeed, 0), ForceMode2D.Force);
                break;
            case 4:
                rb.AddForce(new Vector3(-moveSpeed, 0, 0), ForceMode2D.Force);
                break;
        }
    }
    private void Click()
    {
        if (coolDown > 0) return;
        game.hintVisible = false;
        if (initialStop)
        {
            if (game.gameMode == GameManager.GameMode.Flappy)
            {
                rb.gravityScale = initialG;
            }
            game.StartGame();
            initialStop = false;
        }
        AudioSource.PlayClipAtPoint(jump, transform.position);
        switch (game.gameMode)
        {
            case GameManager.GameMode.Flappy:
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
                GameManager.Instance.AddScore(5, "Flap");
                break;
            case GameManager.GameMode.Clockwise:
                GameManager.Instance.AddScore(1, "Flip");
                SwitchDirection();
                if (reflectionPercentage < 0.9f)
                {
                    reflectionPercentage += 0.05f;
                }
                else
                {
                    reflectionPercentage = 0.9f;
                }
                break;
        }
    }
    private void SwitchDirection()
    {
        if (direction == 0)
        {
            direction = 1;
        }
        direction += 1;
        if(direction == 5) { direction = 1; }
        Vector2 v = rb.linearVelocity * reflectionPercentage;
        rb.linearVelocity = new Vector2(v.y, -v.x);
    }
    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }
    private IEnumerator RestartCoroutine()
    {
        while (Time.timeScale > 0)
        {
            Time.timeScale = Mathf.Max(Time.timeScale - 1f * Time.fixedUnscaledDeltaTime, 0);
            music.pitch = Time.timeScale;
            yield return new WaitForSecondsRealtime(1f / 60f);
        }
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        if (screenshot != null)
        {
            Sprite sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
            loseScreenshot.sprite = sprite;
        }
        yield return new WaitForSecondsRealtime(0.01f);
        loseScreenshotParent.gameObject.SetActive(true);
        loseScreenshotSound.Play();
        yield return new WaitForSecondsRealtime(1f);
        float t = 0;
        while (t < 1) {
            t += 1/60f;
            ScoreText.text = $"Score: {(int)Mathf.Lerp(0, GameManager.Score, t)}";
            yield return new WaitForSecondsRealtime(1f / 60f);
        }
        ScoreText.text = $"Score: {GameManager.Score}";
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Wall")
        {
            Restart();
        }
        if (collision.transform.tag == "Blob")
        {
            GameManager.Instance.AddScore(100, "Blob");
            collision.transform.gameObject.GetComponent<Blob>().Collect();
        }
    }
    public void ResetPosition() {
        StartCoroutine(resetPosition());
    }
    private IEnumerator resetPosition()
    {
        yield return null;
    }
    private void Pause()
    {
        Debug.Log("Pause action performed!");
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