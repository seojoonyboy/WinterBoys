using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    public AudioSource efxSource;
    public AudioSource bgmSource;

    [SerializeField] public Sound[] efx_sounds;
    [SerializeField] public Sound[] bgm_sounds;

    public void Play(SoundType type, string name, float volume = 1) {
        Sound targetRes = searchResource(type, name);
        if (name == null || targetRes == null) {
            Debug.LogError("CAN NOT FIND " + name + " SOUND");
            return;
        }

        switch (type) {
            case SoundType.BGM:
                bgmSource.clip = targetRes.clip;
                bgmSource.volume = volume;
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
                break;
            case SoundType.EFX:
                arr = efx_sounds;
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

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public enum SoundType {
        EFX,
        BGM
    }
}
