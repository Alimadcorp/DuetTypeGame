using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class Logger : MonoBehaviour
{
    const string PendingKey = "PendingLogs";
    public static int Enabled = 1;
    private void Start()
    {
        Enabled = PlayerPrefs.GetInt("LogEnabled", 1);
    }

    public static void Log(string text)
    {
        if (Enabled == 1)
        {
            string url = $"https://madlog.vercel.app/api/log?country=UK&channel=gleamboxchannel1&status=OK&text={UnityWebRequest.EscapeURL(text)}";
            instance.StartCoroutine(SendLog(url));
        }
    }
    public static void LogImp(string text)
    {
        string url = $"https://madlog.vercel.app/api/log?country=UK&channel=gleamboxchannel1&status=OK&text={UnityWebRequest.EscapeURL(text)}";
        instance.StartCoroutine(SendLog(url));
    }

    public static Logger instance;
    void Awake() => instance = this;

    static IEnumerator SendLog(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            string timestamped = url + $" : {DateTime.Now:HH:mm:ss}";
            string pending = PlayerPrefs.GetString(PendingKey, "");
            pending += timestamped + "\n";
            PlayerPrefs.SetString(PendingKey, pending);
            PlayerPrefs.Save();
        }
        else
        {
            string pending = PlayerPrefs.GetString(PendingKey, "");
            if (!string.IsNullOrEmpty(pending))
            {
                string[] urls = pending.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> failedAgain = new List<string>();

                foreach (string line in urls)
                {
                    string cleanUrl = line.Split(new[] { " :" }, 2, StringSplitOptions.None)[0];
                    UnityWebRequest retry = UnityWebRequest.Get(cleanUrl);
                    yield return retry.SendWebRequest();

                    if (retry.result != UnityWebRequest.Result.Success)
                        failedAgain.Add(line);
                }

                if (failedAgain.Count > 0)
                    PlayerPrefs.SetString(PendingKey, string.Join("\n", failedAgain) + "\n");
                else
                    PlayerPrefs.DeleteKey(PendingKey);

                PlayerPrefs.Save();
            }
        }
    }
}
