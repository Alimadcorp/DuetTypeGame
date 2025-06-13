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
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SendWebRequest();
        }
    }
    public static void LogImp(string text)
    {
        string url = $"https://madlog.vercel.app/api/log?country=UK&channel=gleamboxchannel1&status=OK&text={UnityWebRequest.EscapeURL(text)}";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
    }
    public static void LogImpToChannel(string text, string channel)
    {
        string url = $"https://madlog.vercel.app/api/log?country=CU&channel={channel}&status=OK&text={UnityWebRequest.EscapeURL(text)}";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
    }

    public static Logger instance;
    void Awake() => instance = this;
}
