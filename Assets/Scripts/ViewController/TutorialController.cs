using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {
    public Sprite[] backgrounds;
    public Image background;
    public GameObject[] dh_paragraph;
    public GameObject[] sj_paragraph;

    public void setBackgroundImage(int index, bool isLandscape) {
        if (isLandscape) {

        }
        else {

        }
        background.sprite = backgrounds[index];
    }

    public void nextPage() {

    }
}
