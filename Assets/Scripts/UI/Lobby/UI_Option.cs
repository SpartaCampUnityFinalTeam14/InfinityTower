using UnityEngine;
using UnityEngine.UI;

public class UI_Option : UI, ScrollPanel
{
    [SerializeField] private Button backgroundButton;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Button closeButton;

    protected override void Awake()
    {
        base.Awake();

        SoundManager.Instance.LoadVolume();

        masterSlider.value = SoundManager.Instance.GetMasterVolume();
        BGMSlider.value = SoundManager.Instance.GetBGMVolume();
        SFXSlider.value = SoundManager.Instance.GetSFXVolume();

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        BGMSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        backgroundButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Option>());
        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Option>());
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
