using UnityEngine;
using UnityEngine.UI;

public class UI_Option : MonoBehaviour, ScrollPanel
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Button clearButton;

    protected void Awake()
    {
        SoundManager.Instance.LoadVolume();

        clearButton.onClick.AddListener(() =>
        {
            SaveManager.Instance.DeleteAll();
            GameManager.Instance.isTutorialAlreadySeen = false;
            GameManager.Instance.LoadScene("KSM_Lobby");
        });

        masterSlider.value = SoundManager.Instance.GetMasterVolume();
        BGMSlider.value = SoundManager.Instance.GetBGMVolume();
        SFXSlider.value = SoundManager.Instance.GetSFXVolume();

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        BGMSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void ResetPanel()
    {

    }

    void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
