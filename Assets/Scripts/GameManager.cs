using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Flappy, Clockwise, CounterClockwise, Bee, HoldFlap, Random };
    public GameMode gameMode = GameMode.Flappy;
    public static GameManager Instance;
    public float speedFactor = 1.0f;
    public FlappyWallSpawner flappyWallSpawner;
    private float sinceGameModeChange = 0f;
    private float sinceWallSpawn = 0f;
    [Header("Parameters")]
    public float wallSpeed, opening;
    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }
    void FixedUpdate()
    {
        sinceGameModeChange += Time.fixedDeltaTime;
        sinceWallSpawn += Time.fixedDeltaTime;
        if (sinceGameModeChange > 2f)
        {
            switch (gameMode)
            {
                case GameMode.Flappy:
                    if (sinceWallSpawn > 1f)
                    {
                        sinceWallSpawn = Random.Range(-1f, 0f);
                        flappyWallSpawner.Spawn(wallSpeed, false, opening * Random.Range(0.7f, 1.3f));
                    }
                    break;
            }
        }
    }
}
