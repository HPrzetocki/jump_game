using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        // Ustaw zakres (na wszelki wypadek)
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // Wczytaj zapisany poziom głośności (domyślnie 1)
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        slider.value = savedVolume;

        // Podłącz event
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Zapisz do PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        // Jeśli manager istnieje (np. z DontDestroyOnLoad) – zaktualizuj od razu
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.SetMasterVolume(value);
        }
    }
}
