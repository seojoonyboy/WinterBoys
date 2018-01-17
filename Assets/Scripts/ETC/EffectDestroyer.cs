using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class EffectDestroyer : MonoBehaviour {
    private void Start() {
        Invoke("AnimEnd", 0.8f);
    }

    private void AnimEnd() {
        Destroy(gameObject);
    }
}
