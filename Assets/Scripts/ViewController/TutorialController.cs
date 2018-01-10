﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialController : MonoBehaviour {
    public Sprite[] backgrounds;
    public Image background;
    public GameObject[] dh_paragraph;
    public GameObject[] sj_paragraph;

    private GameManager.tutorialEnum type;

    public void init(GameManager.tutorialEnum type) {
        switch (type) {
            case GameManager.tutorialEnum.DOWNHLL:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case GameManager.tutorialEnum.SKIJUMP:
                Screen.orientation = ScreenOrientation.Landscape;
                break;
        }
        this.type = type;
    }

    public void nextPage(GameObject nextTarget) {
        nextTarget.SetActive(true);
    }

    public void done() {
        GameManager.Instance.tutorialDone(type);
        switch (type) {
            case GameManager.tutorialEnum.DOWNHLL:
                loadScene("DownHill");
                break;
            case GameManager.tutorialEnum.SKIJUMP:
                loadScene("SkiJump");
                break;
        }
    }

    private void loadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
