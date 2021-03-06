﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    public AudioSource efxSource;
    public AudioSource bgmSource;

    [SerializeField] public Sound[] efx_sounds;
    [SerializeField] public Sound[] bgm_sounds;

    private void Start() {
        setOption();
    }

    public void setOption() {
        GameManager.OptionData option = GameManager.Instance.optionData;
        bgmSource.gameObject.SetActive(option.bgm);        
        efxSource.gameObject.SetActive(option.efx);
    }

    public void Play(SoundType type, string name, float volume = 1) {
        Sound targetRes = searchResource(type, name);
        if (name == null || targetRes == null) {
            //Debug.LogError("CAN NOT FIND " + name + " SOUND");
            return;
        }

        switch (type) {
            case SoundType.BGM:
                bgmSource.clip = targetRes.clip;
                bgmSource.volume = 0.1f;
                bgmSource.Play();
                break;

            case SoundType.EFX:
                efxSource.clip = targetRes.clip;
                efxSource.volume = volume;
                efxSource.Play();
                break;
        }
    }

    //게임내에서 동시에 2개 이상의 effect sound 처리가 있을 수 있어 public으로 선언함 
    public Sound searchResource(SoundType type, string name) {
        Sound[] arr = null;
        Sound targetRes = null;

        switch (type) {
            case SoundType.BGM:
                arr = bgm_sounds;
                if(!bgmSource.gameObject.activeSelf) return null;
                break;
            case SoundType.EFX:
                arr = efx_sounds;
                if(!efxSource.gameObject.activeSelf) return null;
                break;
        }

        for(int i=0; i<arr.Length; i++) {
            if(arr[i].name == name) {
                targetRes = arr[i];
                break;
            }
        }

        return targetRes;
    }

    public void VolumeChange(float value, SoundType type) {
        if(type == SoundType.BGM) {
            bgmSource.volume = value;
        }
        else {
            efxSource.volume = value;
        }
    }

    public void Stop(SoundType type) {
        switch (type) {
            case SoundType.BGM:
                bgmSource.Stop();
                break;
            case SoundType.EFX:
                efxSource.Stop();
                break;
        }
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public enum SoundType {
        EFX,
        BGM
    }
}
