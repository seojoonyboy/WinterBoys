using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    public AudioSource efxSource;
    public AudioSource bgmSource;

    public AudioClip[] 
        scene_dh_effects,
        scene_sj_effects,
        scene_st_effects,
        scene_main_effects,
        scene_charchange_effects,
        bgms;

    public SoundType soundType;

    public void Play(SoundType type, int index = -1) {
        if (index == -1) return;

        switch (type) {
            case SoundType.BGM:
                bgmSource.clip = bgms[index];
                break;
            case SoundType.DOWNHILL:
                efxSource.clip = scene_dh_effects[index];
                break;
            case SoundType.SKELETON:
                efxSource.clip = scene_st_effects[index];
                break;
            case SoundType.SKIJUMP:
                efxSource.clip = scene_sj_effects[index];
                break;
            case SoundType.MAIN_SCENE:
                efxSource.clip = scene_main_effects[index];
                break;
            case SoundType.CHARCHANGE_SCENE:
                efxSource.clip = scene_charchange_effects[index];
                break;
        }

        efxSource.Play();
        bgmSource.Play();
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public enum SoundType {
        DOWNHILL,
        SKIJUMP,
        SKELETON,
        MAIN_SCENE,
        CHARCHANGE_SCENE,
        BGM
    }
}
