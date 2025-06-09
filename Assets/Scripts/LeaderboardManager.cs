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
        if (!gameObject.activeInHierarchy)
        {
            bg.SetActive(true);
            gameObject.SetActive(true);
            parent.SetActive(false);
            loading.gameObject.SetActive(true);
            error.gameObject.SetActive(false);
        }
        Leaderboards.Main.GetEntries(OnLoaded, OnError);
    }
    private void OnLoaded(Entry[] entries)
    {
        parent.SetActive(true);
        bool foundMine = false;
        loading.gameObject.SetActive(false);
        for (int i = 0; i < (entries.Length <= 14 ? entries.Length : 14); i++)
        {
            ldarray[i].SetEntry(entries[i].Username, entries[i].Rank, entries[i].Score, entries[i].IsMine());
            if (entries[i].IsMine())
            {
                foundMine = true;
                if (entries[i].Score != PlayerPrefs.GetInt("highScore"))
                {
                    if (!submiting)
                    {
                        SubmitEntry(PlayerPrefs.GetInt("highScore"));
                    }
                }
            }
        }
        if (!foundMine)
        {
            if (!submiting)
            {
                SubmitEntry(PlayerPrefs.GetInt("highScore"));
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
        if (PlayerPrefs.GetString("myUsername") == "")
        {
            PlayerPrefs.SetString("myUsername", username.text);
            PlayerPrefs.Save();
            SubmitEntry(-1);
            UIParent.SetActive(false);
        }
        else
        {
            string lastName = PlayerPrefs.GetString("myUsername");
            PlayerPrefs.SetString("myUsername", username.text);
            PlayerPrefs.Save();
            if (PlayerPrefs.GetString("myUsername") != lastName){
                DeleteEntry();
            }
            else
            {
                SubmitEntry(PlayerPrefs.GetInt("highScore"));
            }
            UIParent.SetActive(false);
        }
    }
    public void SubmitEntry(int score)
    {
        Debug.Log(PlayerPrefs.GetString("myUsername") + " : " + PlayerPrefs.GetInt("highScore"));
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
    private void SES()
    {
        SubmitEntry(scorer);
    }
    public void OpenEdit()
    {
        UIParent.SetActive(true);
        username.text = PlayerPrefs.GetString("myUsername");
    }
    public void Grs(bool done)
    {
        if (!done)
        {
            Invoke("SES", 1f);
        }
        else
        {
            submiting = false;
            OpenLeaderboard(true);
        }
    }

    public void DeleteEntry()
    {
        Leaderboards.Main.DeleteEntry(OnDelete);
    }
    private void OnDelete(bool yes)
    {
        SubmitEntry(PlayerPrefs.GetInt("highScore"));
    }
}