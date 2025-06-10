using System;
using System.Collections;
using UnityEngine;

public class DiscordRP : MonoBehaviour
{
    private long CLIENT_ID = 1381283500765741116;
    private static Discord.Discord discord;
    public static DateTime startTime;
    public static DiscordRP instance;
    public static Discord.ActivityManager activityManager;
    string state = "In Main Menu";
    string detail = "High Score: 0";
    string miniImg = "icon";
    string miniTxt = "Main Menu";
    public int updateRate = 3;
    private float tSince = 0;
    public static bool Enabled = true;
    public bool Log = false;
    public bool runInEditor = false;
    private void Awake()
    {
        instance = this;
        Enabled = Application.platform == RuntimePlatform.WindowsPlayer || (runInEditor && Application.platform == RuntimePlatform.WindowsEditor);
        detail = $"High Score: {PlayerPrefs.GetInt("highScore")}";
    }
    void Start()
    {
        if (Enabled)
        {
            discord = new Discord.Discord(CLIENT_ID, (UInt64)Discord.CreateFlags.NoRequireDiscord);
            activityManager = discord.GetActivityManager();
        }
        if (!Global.discordInitiated)
        {
            startTime = DateTime.Now;
            Global.discordInitiated = true;
        }
    }
    private void Update()
    {
        tSince += Time.unscaledDeltaTime;
        if (tSince > (float)updateRate)
        {
            UpdateRP();
            tSince = 0;
        }
        if (Enabled) discord.RunCallbacks();
    }
    private void UpdateRP()
    {
        if (GameManager.Score == 0)
        {
            /*if (Player.Instance.ldMan.parent.activeInHierarchy || Player.Instance.ldMan.loading.gameObject.activeInHierarchy || Player.Instance.ldMan.error.gameObject.activeInHierarchy)
            {
                state = $"Viewing Leaderboard";
                detail = $"{PlayerPrefs.GetString("myUsername")}: {PlayerPrefs.GetInt("highScore")}";
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
            {*///}
            state = "In Main Menu";
            detail = $"{PlayerPrefs.GetString("myUsername", "High Score")}: {PlayerPrefs.GetInt("highScore")}";
            miniImg = "icon";
            miniTxt = "Main Menu";
        }
        else
        {
            string mode = GameManager.Instance.gameMode.ToString();
            mode = (mode == "Clockwise" || mode == "AntiClockwise") ? "Round Mode" : mode + " Mode";
            state = $"Playing: {mode}";
            detail = $"Score: {GameManager.Score}";
            miniImg = "play";
            miniTxt = "Too busy playing";
        }
        if (GameManager.Instance.over)
        {
            state = $"Just Ended a Round";
            detail = $"Score: {GameManager.Score} | High Score: {PlayerPrefs.GetInt("highScore")}";
            miniImg = "play";
            miniTxt = "Done playing";
        }
        if (Log) Debug.Log($"{state}\n{detail}");
        var unixTimestamp = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
        if (Enabled)
        {
            Discord.Activity activity = new Discord.Activity
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
            if (Log) Debug.Log("Updating Activity");
            activityManager.UpdateActivity(activity, (result) => { Debug.Log(result); });
        }

    }
    static bool WantsToQuit()
    {
        if (Enabled)
        {
            activityManager.ClearActivity((result) => { Application.Quit(); });
            discord.Dispose();
        }
        /*if (Enabled && discord != null)
        {
            DiscordRP.instance.StartCoroutine(OnQuit());
            return false; // Returning false would cancel the quit
        }*/
        return true;
    }
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit; // Connect function
    }
    /*
    private static IEnumerator OnQuit()
    {
        if (Enabled)
        {
            // Try clear activity before quit
            activityManager.ClearActivity((result) => { Application.Quit(); });
            discord.Dispose();
        }
        yield return new WaitForSecondsRealtime(5f);
        Application.Quit(); // Quit anyway if 5 seconds passed
    }*/
}
