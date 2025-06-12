using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Image volumeImg;
    public Sprite fullImg, noImg;
    public Slider volumeSlider;
    public Toggle consntToPolicy;
    public Toggle consntToSharing;
    public Toggle consntToSharingI;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI fpsText;
    public GameObject blackbelt;
    public GameObject consent, panel;
    public GameObject shop, quit;
    public Button quitButton;
    public Button consentButton, exitButton;
    public Color unlocked, purchased, equipped;
    public Button[] trailButtons;
    public TextMeshProUGUI[] trailTexts;
    public TextMeshProUGUI currentAmt;
    public enum ItemState { Locked, Unlocked, Purchased, Equipped };
    [SerializedDictionary("ID", "Price")]
    public SerializedDictionary<string, int> prices;
    [SerializedDictionary("ID", "State")]
    public SerializedDictionary<string, ItemState> states;
    public void SaveStates()
    {
        List<string> parts = new();
        foreach (var kv in states)
        {
            string key = UnityWebRequest.EscapeURL(kv.Key);
            string val = UnityWebRequest.EscapeURL(JsonUtility.ToJson(kv.Value));
            parts.Add(key + "=" + val);
        }
        PlayerPrefs.SetString("States", string.Join("&", parts));
        PlayerPrefs.Save();
    }

    public void LoadStates()
    {
        string data = PlayerPrefs.GetString("States");
        if (string.IsNullOrEmpty(data)) return;

        states.Clear();
        var pairs = data.Split('&');
        foreach (var pair in pairs)
        {
            var kv = pair.Split('=');
            if (kv.Length != 2) continue;

            string key = UnityWebRequest.UnEscapeURL(kv[0]);
            string val = UnityWebRequest.UnEscapeURL(kv[1]);
            states[key] = JsonUtility.FromJson<ItemState>(val);
        }
    }

    public void CancelExit()
    {
        Application.Quit();
    }
    public void onPolicyToggle(bool did)
    {
        consentButton.interactable = did;
    }
    public void Consent()
    {
        Logger.Enabled = consntToSharing.isOn ? 1 : 0;
        PlayerPrefs.SetInt("Consent", consntToSharing.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void ConsentAgain(bool what)
    {
        Logger.Enabled = what ? 1 : 0;
        PlayerPrefs.SetInt("Consent", what ? 1 : 0);
        PlayerPrefs.Save();
    }
    void Start()
    {
        consntToSharingI.isOn = PlayerPrefs.GetInt("Consent") == 1;
        if (PlayerPrefs.GetInt("Consent", -1) == -1)
        {
            consent.SetActive(true);
            panel.SetActive(true);
        }
        if (Global.graphEnabled)
        {
            Tayx.Graphy.GraphyManager.Instance.Enable();
        }
        else
        {
            Tayx.Graphy.GraphyManager.Instance.Disable();
        }
        credits.text = credits.text.Replace("{Version}", Application.version).Replace("{Username}", PlayerPrefs.GetString("myUsername")).Replace("{Score}", PlayerPrefs.GetInt("highScore").ToString()).Replace("{UnityVersion}", Application.unityVersion).Replace("{Name}", Application.productName).Replace("{Company}", Application.companyName).Replace("{SaveFolder}", Application.dataPath).Replace("{FrameRate}", Application.targetFrameRate.ToString()).Replace("{Language}", Application.systemLanguage.ToString()).Replace("{Genuine}", Application.genuine ? "Genuine Build" : "Insecure or Altered Build").Replace("{GUID}", Application.buildGUID).Replace("{Platform}", Application.platform.ToString());
        Adjust();
    }
    private void Update()
    {
        fpsText.text = "Frame Rate: " + (int)(1f / Time.deltaTime);
    }
    public void OpenCredits()
    {
        blackbelt.SetActive(Random.Range(0, 10) == 5 ? true : false);
    }
    public void ToggleFPS()
    {
        Global.graphEnabled = !Global.graphEnabled;
        if (Global.graphEnabled)
        {
            Tayx.Graphy.GraphyManager.Instance.Enable();
        }
        else
        {
            Tayx.Graphy.GraphyManager.Instance.Disable();
        }
    }
    void Adjust()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("prefs_volume", 1f);
        volumeSlider.value = AudioListener.volume;
        if (volumeSlider.value < 0.0001f)
        {
            volumeImg.sprite = noImg;
        }
    }
    public void SetVolume(float volume)
    {
        volumeImg.sprite = volumeSlider.value < 0.0001f ? noImg : fullImg;
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("prefs_volume", volume);
        PlayerPrefs.Save();
    }
    public void ToggleShop()
    {
        shop.SetActive(!shop.activeInHierarchy);
    }
    public void Quit()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Logger.Log("Attempt to exit a web build: " + PlayerPrefs.GetString("myUsername"));
            StartCoroutine(ExitCoroutine());
        }
        else
        {
            Application.Quit();
        }
    }
    private IEnumerator ExitCoroutine()
    {
        float t = 0;
        Color initial = quitButton.GetComponent<Image>().color;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            quitButton.GetComponent<Image>().color = Color.Lerp(initial, new Color(1, 0.2f, 0.2f), t);
            yield return null;
        }
        quit.SetActive(true);
    }
    public void Logging(bool enable)
    {
        Logger.Enabled = enable ? 1 : 0;
        PlayerPrefs.SetInt("LogEnabled", enable ? 1 : 0);
        PlayerPrefs.SetInt("Consent", 1);
        PlayerPrefs.Save();
    }
    public void Redirect(string url)
    {
        Application.OpenURL(url);
    }

    public void UpdateShopUI()
    {
        LoadStates();
        currentAmt.text = $"{GameManager.Instance.Blobs}";

        for (int i = 0; i < trailButtons.Length; i++)
        {
            if (states.ElementAt(i).Value == ItemState.Unlocked)
            {
                trailButtons[i].interactable = GameManager.Instance.Blobs >= prices.ElementAt(i).Value;
                trailButtons[i].GetComponent<Image>().color = unlocked;
                trailTexts[i].text = prices.ElementAt(i).Value.ToString();
            }
            else if (states.ElementAt(i).Value == ItemState.Purchased)
            {
                trailButtons[i].interactable = true;
                trailTexts[i].text = "Equip";
                trailButtons[i].GetComponent<Image>().color = purchased;
            }
            else if (states.ElementAt(i).Value == ItemState.Equipped)
            {
                trailButtons[i].interactable = true;
                trailButtons[i].GetComponent<Image>().color = equipped;
                trailTexts[i].text = "Equipped";
            }
        }
    }
    public void buyItem(string itemId)
    {
        if (states[itemId] == ItemState.Unlocked)
        {
            if (prices[itemId] <= GameManager.Instance.Blobs)
            {
                Debug.Log("Bought " + itemId);
                GameManager.Instance.Blobs -= prices[itemId];
                for (int i = 0; i < states.Count; i++)
                {
                    if (states.ElementAt(i).Value == ItemState.Equipped)
                    {
                        states[states.ElementAt(i).Key] = ItemState.Purchased;
                        break;
                    }
                }
                states[itemId] = ItemState.Equipped;
                PlayerPrefs.SetInt("Blobs", GameManager.Instance.Blobs);
                PlayerPrefs.Save();
                GameManager.Instance.SetTrail(itemId);
            }
            else
            {
                Debug.Log("Cant buy");
            }
        }
        else if (states[itemId] == ItemState.Purchased)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states.ElementAt(i).Value == ItemState.Equipped)
                {
                    states[states.ElementAt(i).Key] = ItemState.Purchased;
                    break;
                }
            }
            states[itemId] = ItemState.Equipped;
            GameManager.Instance.SetTrail(itemId);
        }
        SaveStates();
        UpdateShopUI();
    }
}
