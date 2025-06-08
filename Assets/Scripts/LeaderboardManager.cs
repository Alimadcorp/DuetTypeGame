using UnityEngine;
using Dan.Main;
using System;
using TMPro;
using Dan.Models;
using UnityEngine.UI;
public class LeaderboardManager : MonoBehaviour
{
    public TextMeshProUGUI loading;
    public TextMeshProUGUI error;
    public LeaderEntry[] ldarray;
    public GameObject parent;

    public TMP_InputField username;
    public Button ubutton;
    public GameObject UIParent, bg;
    private int scorer = 0;
    private bool submiting = false;
    public void OpenLeaderboard(bool hi)
    {
        if (!hi)
        {
            SubmitEntry(scorer);
        }
        bg.SetActive(true);
        gameObject.SetActive(true);
        parent.SetActive(false);
        loading.gameObject.SetActive(true);
        error.gameObject.SetActive(false);
        Leaderboards.Main.GetEntries(OnLoaded, OnError);
    }
    private void OnLoaded(Entry[] entries)
    {
        parent.SetActive(true);
        loading.gameObject.SetActive(false);
        for (int i = 0; i < (entries.Length <= 14 ? entries.Length : 14); i++)
        {
            ldarray[i].SetEntry(entries[i].Username, entries[i].Rank, entries[i].Score, entries[i].IsMine());
            if (entries[i].IsMine())
            {
                if(entries[i].Score != PlayerPrefs.GetInt("highScore"))
                {
                    if (!submiting)
                    {
                        SubmitEntry(PlayerPrefs.GetInt("highScore"));
                    }
                }
            }
        }
    }
    private void OnError(string _error)
    {
        loading.gameObject.SetActive(false);
        parent.gameObject.SetActive(false);
        error.gameObject.SetActive(true);
        error.text = _error;
    }
    public void ProcessInput(string input)
    {
        ubutton.interactable = input.Trim() != "";
    }
    public void SetUsername()
    {
        PlayerPrefs.SetString("myUsername", username.text);
        PlayerPrefs.Save();
        SubmitEntry(-1);
        UIParent.SetActive(false);
    }
    public void SubmitEntry(int score)
    {
        submiting = true;
        if (score == -1) score = scorer;
        scorer = score;
        string usnm = PlayerPrefs.GetString("myUsername");
        if (usnm == null || usnm == "")
        {
            UIParent.SetActive(true);
        }
        else
        {
            Leaderboards.Main.UploadNewEntry(usnm, score, Grs);
        }
    }
    public void Grs(bool done)
    {
        if (!done) {
            SubmitEntry(scorer);
        }
        else
        {
            submiting = false;
            OpenLeaderboard(true);
        }
    }
}