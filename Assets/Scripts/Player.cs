using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private InputActions inputActions;
    private InputActions.NormalActions normalActions;
    private Rigidbody2D rb;
    private GameManager game;
    public SpriteRenderer loseScreenshot;
    public Transform loseScreenshotParent;
    public AudioSource loseScreenshotSound;
    public AudioSource music;
    [Header("Tweakables")]
    public float jumpForce = 1.0f;
    public float gravity = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        normalActions = inputActions.Normal;
        normalActions.Click.performed += ctx => Click();
        normalActions.Pause.performed += ctx => Pause();
    }
    private void Start()
    {
        game = GameManager.Instance;
    }
    private void FixedUpdate()
    {

    }
    private void Click()
    {
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
        loseScreenshotSound.Play();
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        loseScreenshotParent.gameObject.SetActive(true);
        if (screenshot != null)
        {
            Sprite sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
            loseScreenshot.sprite = sprite;
        }
        yield return new WaitForSecondsRealtime(5f);
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