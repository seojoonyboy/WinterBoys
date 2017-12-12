using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    public SoundType soundType;
    private AudioSource efxSource;
    private AudioSource bgmSource;

    public enum SoundType {
        ITEM_GET,
        BUTTON_CLICK,
        LEVEL_UP
    }

    public AudioClip[] 
        scene_dh_effects,
        scene_sj_effects,
        scene_st_effects,
        scene_main_effects,
        scene_charchange_effects,
        bgms;

    private void Start() {
        bgmSource.loop = true;
        efxSource.loop = false;
    }

    public void Play(SportType type, SoundType subtype) {
        AudioClip[] myLists = selectedLists(type);
        switch (subtype) {
            case SoundType.BUTTON_CLICK:

                break;
            case SoundType.ITEM_GET:

                break;
            case SoundType.LEVEL_UP:

                break;
        }
        efxSource.Play();
    }

    private AudioClip[] selectedLists(SportType type) {
        AudioClip[] arr = null;
        switch (type) {
            case SportType.DOWNHILL:
                arr = scene_dh_effects;
                break;
            case SportType.SKIJUMP:
                arr = scene_sj_effects;
                break;
            case SportType.SKELETON:
                arr = scene_st_effects;
                break;
        }
        return arr;
    }
}
