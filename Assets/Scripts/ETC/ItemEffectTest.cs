using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectTest : MonoBehaviour {
    public SkiJumpPlayerController controller;

    public void OnClick() {
        controller.itemCheck(gameObject);
    }
}
