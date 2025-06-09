using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Image volumeImg;
    public Sprite fullImg, noImg;
    public Slider volumeSlider;
    void Start()
    {
        Adjust();
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
