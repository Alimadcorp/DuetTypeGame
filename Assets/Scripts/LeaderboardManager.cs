using UnityEngine;
using Dan.Main;
using TMPro;
using Dan.Models;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Button relButton;
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
        relButton.interactable = false;
        Leaderboards.Main.GetEntries(OnLoaded, OnError);
    }
    private void OnLoaded(Entry[] entries)
    {
        relButton.interactable = true;
        parent.SetActive(true);
        bool foundMine = false, foundMineInTop10 = false; int myEntryI = 0;
        loading.gameObject.SetActive(false);
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].IsMine())
            {
                foundMine = true;
                myEntryI = i;
                if (entries[i].Score != PlayerPrefs.GetInt("highScore"))
                {
                    if (!submiting)
                    {
                        SubmitEntry(PlayerPrefs.GetInt("highScore"));
                    }
                }
            }
        }
        if (foundMine)
        {
            if (PlayerPrefs.GetInt("highScore") < entries[myEntryI].Score)
            {
                PlayerPrefs.SetInt("highScore", entries[myEntryI].Score);
                PlayerPrefs.SetString("myUsername", entries[myEntryI].Username);
                PlayerPrefs.Save();
            }
        }
        for (int i = 0; i < (entries.Length <= 11 ? entries.Length : 11); i++)
        {
            ldarray[i].SetEntry(entries[i].Username, entries[i].Rank, entries[i].Score, entries[i].IsMine());
            if (entries[i].IsMine())
            {
                foundMineInTop10 = true;
            }
        }
        int myEntryIndex = 10;
        if (!foundMineInTop10 && foundMine)
        {
            ldarray[myEntryIndex].SetEntry(entries[myEntryI].Username, entries[myEntryI].Rank, entries[myEntryI].Score, true);
        }
        if (!foundMine)
        {
            if (!submiting && PlayerPrefs.GetInt("highScore") != 0 && PlayerPrefs.GetString("myUsername") != "")
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
            if (PlayerPrefs.GetInt("highScore") > 0) SubmitEntry(-1);
            UIParent.SetActive(false);
        }
        else
        {
            string lastName = PlayerPrefs.GetString("myUsername");
            if (username.text.ToUpper() == "RESETDATA")
            {
                DeleteEntry();
                PlayerPrefs.DeleteKey("highScore");
                PlayerPrefs.DeleteKey("myUsername");
                PlayerPrefs.Save();
            }
            PlayerPrefs.SetString("myUsername", username.text);
            PlayerPrefs.Save();
            if (PlayerPrefs.GetString("myUsername") != lastName)
            {
                if (PlayerPrefs.GetInt("highScore") > 0) SubmitEntry(PlayerPrefs.GetInt("highScore"));
            }
            else
            {
                if (PlayerPrefs.GetInt("highScore") > 0) SubmitEntry(PlayerPrefs.GetInt("highScore"));
            }
            UIParent.SetActive(false);
        }
    }
    public void SubmitEntry(int score)
    {
        if (PlayerPrefs.GetInt("highScore") == 0) return;
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
            if (username.text.ToUpper() == "RESETDATA") { PlayerPrefs.DeleteAll(); PlayerPrefs.Save(); SceneManager.LoadSceneAsync(0); return; }
            if (usnm.ToLower() == "delete entry") { DeleteEntry(); return; }
            if (usnm.ToLower() == "dont upload") return;
            Leaderboards.Main.UploadNewEntry(usnm, score, PlayerPrefs.GetString("history"), Grs);
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
        OpenLeaderboard(true);
        SceneManager.LoadSceneAsync(0); return;
    }
}