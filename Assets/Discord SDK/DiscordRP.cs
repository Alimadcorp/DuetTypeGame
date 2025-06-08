using System;
using UnityEngine;

public class DiscordRP : MonoBehaviour
{
    private long CLIENT_ID = 1381283500765741116;
    private Discord.Discord discord;
    public static DateTime startTime;
    public static DiscordRP instance;
    Discord.ActivityManager activityManager;
    string state = "In Main Menu";
    string detail = "High Score: 0";
    string miniImg = "icon";
    string miniTxt = "Main Menu";
    private void Awake()
    {
        instance = this;
        detail = $"High Score: {PlayerPrefs.GetInt("highScore")}";
    }
    void Start()
    {
        discord = new Discord.Discord(CLIENT_ID, (UInt64)Discord.CreateFlags.NoRequireDiscord);
        activityManager = discord.GetActivityManager();
        InvokeRepeating("UpdateRP", 1f, 15f);
        startTime = DateTime.Now;
    }

    private void UpdateRP()
    {
        if (Player.Instance.ClickEnabled)
        {
            if (Player.Instance.ldMan.parent.activeInHierarchy) {
                state = $"Viewing Leaderboard";
                detail = $"High Score: {PlayerPrefs.GetInt("highScore")}";
                miniImg = "leaderboard";
                miniTxt = "Viewing leaderboard";
            }
            else if (Player.Instance.credObj.activeInHierarchy)
            {
                state = $"Viewing Credits:";
                detail = $"Made by Muhammad Ali";
                miniImg = "credits";
                miniTxt = "Viewing credits";
            }
            else
            {
                state = "In Main Menu";
                detail = $"High Score: {PlayerPrefs.GetInt("highScore")}";
                miniImg = "icon";
                miniTxt = "Main Menu";
            }
        }
        else
        {
            string mode = GameManager.Instance.gameMode.ToString();
            mode = (mode == "Clockwise" || mode == "Anticlockwise") ? "Normal Mode" : mode + " Mode";
            state = $"Playing: {mode}";
            detail = $"Score: {GameManager.Score}";
            miniImg = "play";
            miniTxt = "Too busy playing";
        }
        if (Player.Instance.ContinueMode)
        {
            state = $"Just Ended a Round";
            detail = $"Score: {GameManager.Score} | High Score: {PlayerPrefs.GetInt("highScore")}";
            miniImg = "play";
            miniTxt = "Done playing";
        }
        var unixTimestamp = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
        var activity = new Discord.Activity
        {
            State = state,
            Details = detail,
            Timestamps =
                {
                    Start = unixTimestamp,
                },
            Assets =
                {
                    LargeImage = "icon",
                    LargeText = "Gleam Box",
                    SmallImage = miniImg,
                    SmallText = miniTxt,
                },
            Instance = true,
        };

        activityManager.UpdateActivity(activity, (result) => {});

    }
    private void Update()
    {
        discord.RunCallbacks();
    }
    private void OnApplicationQuit()
    {
        activityManager.ClearActivity((result) => {});
        discord.Dispose();
    }
}
