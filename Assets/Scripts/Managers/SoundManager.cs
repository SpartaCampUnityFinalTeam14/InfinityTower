using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGM//BGM 폴더의 파일 이름과 동일하게
{

}

public enum SFX//SFX 폴더의 파일 이름과 동일하게
{

}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private Dictionary<BGM, AudioClip> bgmDict = new();//배경음악 모음
    [SerializeField] private Dictionary<SFX, AudioClip> sfxDict = new();//효과음 모음

    [SerializeField] private AudioSource bgmPlayer = new AudioSource();//배경음악 재생기
    [SerializeField] private List<AudioSource> sfxPlayers = new List<AudioSource>();//효과음 재생기
    private Transform sfxPlayerGroup;
    private int _maxPoolSize = 10;

    [SerializeField] private AudioMixer audioMixer;
    readonly string MIXER_MASTER = "MASTER";//모든 볼륨
    readonly string MIXER_BGM = "BGM";//배경 볼륨
    readonly string MIXER_SFX = "SFX";//효과음 볼륨

    protected override void Awake()
    {
        base.Awake();

        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in bgmClips)
        {
            if(Enum.TryParse(clip.name, out BGM bgmType))
            {
                bgmDict[bgmType] = clip;
            }
        }

        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (AudioClip clip in sfxClips)
        {
            if(Enum.TryParse(clip.name, out SFX sfxType))
            {
                sfxDict[sfxType] = clip;
            }
        }

        audioMixer = Resources.Load<AudioMixer>("Sounds/AudioMixer");

        ResetPlayers();
    }

    private void Start()
    {
        LoadVolume();
    }

    void ResetPlayers()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.SetParent(transform);
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MIXER_BGM)[0];
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        sfxPlayerGroup = new GameObject("SFXPlayerGroup").transform;
        sfxPlayerGroup.SetParent(transform);

        for (int i = 0; i < _maxPoolSize; i++)
        {
            GameObject sfxObject = new GameObject("SFXPlayer");
            sfxObject.transform.SetParent(sfxPlayerGroup);
            AudioSource sfxPlayer = sfxObject.AddComponent<AudioSource>();
            sfxPlayer.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MIXER_SFX)[0];
            sfxPlayer.playOnAwake = false;
            sfxPlayer.loop = false;
            sfxPlayers.Add(sfxPlayer);
        }

        sfxPlayers = new List<AudioSource>(sfxPlayerGroup.GetComponentsInChildren<AudioSource>());
    }

    public void PlayBGM(BGM bgm)//배경음악 재생
    {
        if(bgmDict.TryGetValue(bgm, out AudioClip clip))
        {
            bgmPlayer.clip = clip;
            bgmPlayer.Play();
        }
        else throw new InvalidOperationException($"There's No BGM: {name} in SoundManager");
    }

    public void StopBGM()//배경음악 종료
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(SFX sfx)//효과음 재생
    {
        if(sfxDict.TryGetValue(sfx, out AudioClip clip))
        {
            for (int j = 0; j < sfxPlayers.Count; j++)
            {
                if (!sfxPlayers[j].isPlaying)
                {
                    sfxPlayers[j].clip = clip;
                    sfxPlayers[j].Play();
                    return;
                }
            }

            GameObject sfxGo = new GameObject(name);
            sfxGo.transform.SetParent(sfxPlayerGroup);
            AudioSource newSource = sfxGo.AddComponent<AudioSource>();
            newSource.clip = clip;
            newSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MIXER_SFX)[0];
            newSource.playOnAwake = false;
            newSource.loop = false;
            newSource.Play();
            sfxPlayers.Add(newSource);
            StartCoroutine(DestoryAudiosource(newSource));
        }
        else throw new InvalidOperationException($"There's No SFX: {name} in SoundManager");
    }

    public void StopAllSFX()//모든 효과음 종료
    {
        foreach(AudioSource s in sfxPlayers)
        {
            s.Stop();
        }
    }

    IEnumerator DestoryAudiosource(AudioSource source)//효과음 재생 끝나면 오디오소스 삭제
    {
        yield return new WaitWhile(() => source.isPlaying);

        sfxPlayers.Remove(source);
        Destroy(source.gameObject);
    }

    public void StopAllSound()//모든 음악 종료
    {
        StopBGM();
        StopAllSFX();
    }

    public float GetMasterVolume()
    {
        audioMixer.GetFloat(MIXER_MASTER, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public void SetMasterVolume(float volume)//전체 볼륨 조절(0~1)
    {
        volume = Mathf.Max(volume, 0.0001f);
        audioMixer.SetFloat(MIXER_MASTER, Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", GetMasterVolume());
    }

    public float GetBGMVolume()
    {
        audioMixer.GetFloat(MIXER_BGM, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public void SetBGMVolume(float volume)//배경음악 볼륨 조절(0~1)
    {
        volume = Mathf.Max(volume, 0.0001f);
        audioMixer.SetFloat(MIXER_BGM, Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("BGMVolume", GetBGMVolume());
    }

    public float GetSFXVolume()
    {
        audioMixer.GetFloat(MIXER_SFX, out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public void SetSFXVolume(float volume)//효과음 볼륨 조절(0~1)
    {
        volume = Mathf.Max(volume, 0.0001f);
        audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("SFXVolume", GetSFXVolume());
    }

    public void LoadVolume()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }
}
