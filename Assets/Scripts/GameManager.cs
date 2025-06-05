using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Flappy, Clockwise, CounterClockwise, Bee, HoldFlap, Random };
    public GameMode gameMode = GameMode.Flappy;
    public static GameManager Instance;
    public float speedFactor = 1.0f;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        
    }
}
