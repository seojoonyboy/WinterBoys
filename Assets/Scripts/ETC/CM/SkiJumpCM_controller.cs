using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkiJumpCM_controller : MonoBehaviour {
    public PlayableDirector[] playableDirectors;

    private void Start() {
        playableDirectors[0].Play();
    }

    public void Play(int index) {
        Off(playableDirectors[index]);
        playableDirectors[index].Play();
    }

    private void Off(PlayableDirector target) {
        foreach(PlayableDirector director in playableDirectors) {
            if(director == target) {
                director.gameObject.SetActive(true);
            }
            else {
                director.gameObject.SetActive(false);
            }
        }
    }
}
