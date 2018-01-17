﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour {
    private GameManager gm;
    public ReadyController ready;
    public GameObject tutorialModal;
    public GameObject exitModal;
    private void Awake() {
        gm = GameManager.Instance;
    }

    private void Start() {
        SoundManager.Instance.Play(SoundManager.SoundType.BGM, "selGame");
        gm.setExitModal(exitModal);
        exitModal.GetComponentInChildren<Button>().onClick.AddListener(gm.gameOff);
        if(gm.isTutorial(GameManager.tutorialEnum.SELECT)) return;
        tutorialModal.SetActive(true);
        gm.tutorialDone(GameManager.tutorialEnum.SELECT);
    }

    public void LoadGame(int type) {
        switch (type) {
            case 0:
                ready.open(SportType.DOWNHILL);
                break;
            case 1:
                ready.open(SportType.SKELETON);
                break;
            case 2:
                ready.open(SportType.SKIJUMP);
                break;
        }
    }

    public void rankingShow() {
        UM_GameServiceManager.Instance.ShowLeaderBoardsUI();
    }

    public void efxPlay(string name) {
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, name);
    }
}
