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
    private int score = 0;
    public bool initialStop = true;
    private float initialG;

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
    }
    private void Click()
    {
        if (initialStop)
        {
            rb.gravityScale = initialG;
            game.StartGame();
            initialStop = false;
        }
        AudioSource.PlayClipAtPoint(jump, transform.position);
        GameManager.Instance.AddScore(5, "Flap");
        switch (game.gameMode)
        {
            case GameManager.GameMode.Flappy:
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
                break;
        }
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