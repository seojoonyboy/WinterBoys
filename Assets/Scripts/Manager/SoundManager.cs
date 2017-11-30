using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    public SoundType soundType;
    public AudioSource efxSource;

    public enum SoundType {
        ITEM_GET,
        BUTTON_CLICK,
        LEVEL_UP
    }

    public AudioClip[] 
        dh_effects,
        sj_effects,
        st_effects;

    public void Play(SportType type, SoundType subtype) {
        AudioClip[] myLists = selectedLists(type);
        //efxSource.clip = myLists[0];

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
                arr = dh_effects;
                break;
            case SportType.SKIJUMP:
                arr = sj_effects;
                break;
            case SportType.SKELETON:
                arr = st_effects;
                break;
        }
        return arr;
    }
}
