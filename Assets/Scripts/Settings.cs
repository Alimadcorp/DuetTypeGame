using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Image volumeImg;
    public Sprite fullImg, noImg;
    public Slider volumeSlider;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI fpsText;
    void Start()
    {
        credits.text = credits.text.Replace("{Version}", Application.version).Replace("{Username}", PlayerPrefs.GetString("myUsername")).Replace("{Score}", PlayerPrefs.GetInt("highScore").ToString()).Replace("{UnityVersion}", Application.unityVersion).Replace("{Name}", Application.productName).Replace("{Company}", Application.companyName).Replace("{SaveFolder}", Application.dataPath).Replace("{FrameRate}", Application.targetFrameRate.ToString()).Replace("{Language}", Application.systemLanguage.ToString()).Replace("{Genuine}", Application.genuine ? "Genuine Build" : "Insecure or Altered Build").Replace("{GUID}", Application.buildGUID).Replace("{Platform}", Application.platform.ToString());
        Adjust();
    }
    private void Update()
    {
        fpsText.text = "Frame Rate: " + (int)(1f / Time.deltaTime);
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
}
