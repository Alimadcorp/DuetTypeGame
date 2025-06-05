using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputActions inputActions;
    private InputActions.NormalActions normalActions;
    private Rigidbody2D rb;
    private GameManager game;
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
        switch (GameManager.Instance.gameMode)
        {
            case GameManager.GameMode.Flappy:
                //pass
                break;
        }
    }
    private void Click()
    {
        switch (GameManager.Instance.gameMode)
        {
            case GameManager.GameMode.Flappy:
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode2D.Impulse);
                break;
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