using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkiJumpCM_controller : MonoBehaviour {
    public PlayableDirector[] playableDirectors;

    private void Start() {
        playableDirectors[0].Play();
    }

    public void PlayMain() {
        playableDirectors[1].Play();
    }
}
