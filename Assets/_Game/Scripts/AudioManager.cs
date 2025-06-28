using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


[Serializable]
public class AudioMap
{
    public SFXType Type;
    public AudioClip Clip;
}

[Serializable]
public class AudioPlaying
{
    public AudioSource Source;
    public Coroutine Coroutine;
}

public class AudioManager : Singleton<AudioManager>
{
    const string MIXER_BGM = "BGMVolume";
    const string MIXER_SFX = "SFXVolume";

    [SerializeField] AudioMixer audioMixer;
    public AudioSource bgmAudioSource;

    [SerializeField] AudioSource sfxAudioSourceShell;

    //[SerializeField] SerializedDictionary<BGMType, AudioClip> bgmClipMap = new();
    //[SerializeField] SerializedDictionary<SFXType, AudioClip> sfxClipMap;
    [SerializeField] List<AudioMap> sfxClipMap;
    private List<AudioSource> sfxShellPool = new List<AudioSource>();
    //private BGMType currentBGM;

    private Dictionary<SFXType, bool> playingSfx = new Dictionary<SFXType, bool>();
    private Dictionary<SFXType, bool> cooldownSfx = new Dictionary<SFXType, bool>();

    //Serialized for debugging
    //[SerializeField] SerializedDictionary<AudioSource, Coroutine> waitingToClaimSFXSources;
    private List<AudioPlaying> waitingToClaimSFXSources = new List<AudioPlaying>();
    //public void PlayBGM(BGMType type)
    //{
    //    if (currentBGM == type)
    //        return;

    //    bgmAudioSource.clip = bgmClipMap[type];
    //    bgmAudioSource.Play();

    //    currentBGM = type;
    //}
    public void CheckBGM()
    {
        if (!bgmAudioSource.isPlaying)
            bgmAudioSource.Play();
        else
            bgmAudioSource.Stop();
    }

    public void StopBGM()
    {
        if (bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();
    }

    public AudioSource PlaySFX(SFXType type, bool isLoop = false, Transform target = null, bool setParent = false, bool canPlayWhenThereIsDuplicatedSFX = true,
        float delayBeforeCanPlayAgain = 0, bool isBGM = false)
    {
        if (!canPlayWhenThereIsDuplicatedSFX)
        {
            if (!playingSfx.ContainsKey(type))
            {
                playingSfx.Add(type, true);
            }
            else
            {
                if (playingSfx[type])
                {
                    return null;
                }
                else
                {
                    playingSfx[type] = true;
                }
            }
        }

        if (delayBeforeCanPlayAgain > 0)
        {
            if (!cooldownSfx.ContainsKey(type))
            {
                cooldownSfx.Add(type, true);
                DOVirtual.DelayedCall(delayBeforeCanPlayAgain, () =>
                {
                    cooldownSfx[type] = false;
                });
            }
            else
            {
                if (cooldownSfx[type])
                {
                    return null;
                }
                else
                {
                    cooldownSfx[type] = true;
                    DOVirtual.DelayedCall(delayBeforeCanPlayAgain, () =>
                    {
                        cooldownSfx[type] = false;
                    });
                }
            }
        }
        AudioSource sfxAudioSource;
        if (sfxShellPool.Count > 0)
        {
            sfxAudioSource = sfxShellPool[0];

            sfxShellPool.Remove(sfxAudioSource);
        }
        else
        {
            sfxAudioSource = Instantiate(sfxAudioSourceShell, transform);
        }

        if (target != null)
        {
            sfxAudioSource.transform.position = target.position;
        }
        if (setParent)
        {
            sfxAudioSource.transform.parent = target;
        }

        sfxAudioSource.loop = isLoop;
        sfxAudioSource.clip = GetAudioClip(type);
        //sfxAudioSource.spatialBlend = (target == null) ? 0 : 1;
        sfxAudioSource.gameObject.SetActive(true);
        sfxAudioSource.Play();

        if (!isLoop)
        {
            Coroutine cor = StartCoroutine(IECollectSFXShell(sfxAudioSource, GetAudioClip(type).length, canPlayWhenThereIsDuplicatedSFX, type));
            var audioPlaying = new AudioPlaying();
            audioPlaying.Source = sfxAudioSource;
            audioPlaying.Coroutine = cor;
            waitingToClaimSFXSources.Add(audioPlaying);
        }

        if (isBGM)
        {
            sfxAudioSource.loop = true;
            if (bgmAudioSource != null)
                Destroy(bgmAudioSource.gameObject);
            bgmAudioSource = sfxAudioSource;
        }

        return sfxAudioSource;
    }

    public AudioClip GetAudioClip(SFXType type)
    {
        foreach (var item in sfxClipMap)
        {
            if (item.Type == type)
            {
                return item.Clip;
            }
        }
        return null;
    }

    public bool ValidateAudioPlaying(AudioSource audioSource)
    {
        foreach (var item in waitingToClaimSFXSources)
        {
            if (item.Source == audioSource)
            {
                return true;
            }
        }
        return false;
    }

    public AudioPlaying GetAudioPlay(AudioSource source)
    {
        foreach (var item in waitingToClaimSFXSources)
        {
            if (item.Source == source)
            {
                return item;
            }
        }

        return null;
    }

    public void StopSFX(AudioSource audioSource)
    {
        if (audioSource == null)
            return;

        if (!ValidateAudioPlaying(audioSource))
        {
            return;
        }

        audioSource.clip = null;
        audioSource.gameObject.SetActive(false);
        audioSource.transform.parent = transform;
        sfxShellPool.Add(audioSource);

        waitingToClaimSFXSources.Remove(GetAudioPlay(audioSource));
    }

    private IEnumerator IECollectSFXShell(AudioSource audioSource, float delay, bool canDuplicatedPlaying = true, SFXType nonDuplicatedSFXType = 0)
    {
        yield return new WaitForSeconds(delay);

        StopSFX(audioSource);

        if (!canDuplicatedPlaying)
        {
            if (playingSfx.ContainsKey(nonDuplicatedSFXType))
            {
                playingSfx[nonDuplicatedSFXType] = false;
            }
        }
    }

    #region Better way to control volume
    private void SetMusicVolume(float value)
    {
        //value range from 0.0001 to 1
        audioMixer.SetFloat(MIXER_BGM, Mathf.Log10(value) * 20f);
    }

    private void SetSFXVolume(float value)
    {
        //value range from 0.0001 to 1
        audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
    }

    public void MuteMusicVolume()
    {
        audioMixer.SetFloat(MIXER_BGM, -80f);
    }

    public void MuteSFXVolume()
    {
        audioMixer.SetFloat(MIXER_SFX, -80f);
    }

    public void UnmuteMusicVolume()
    {
        audioMixer.SetFloat(MIXER_BGM, 0f);
    }

    public void UnmuteSFXVolume()
    {
        audioMixer.SetFloat(MIXER_SFX, 0f);
    }
    #endregion
}