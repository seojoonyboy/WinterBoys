using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour {
    public ResolutionController resolution;
    public Sprite[] backgrounds;
    public Image background;
    public GameObject[] dh_paragraph;
    public GameObject[] sj_paragraph;

    private GameManager.tutorialEnum type;
    private GameManager gm;
    private void Start() {
        gm = GameManager.Instance;
        type = GameManager.Instance.tutorialSports;
        resolution.isLandScape = !(type == GameManager.tutorialEnum.DOWNHLL);
        if(resolution.isLandScape) resolution.standard_resolution = new Vector2(1920f,1080f);
        init();
    }

    public void init() {
        switch (type) {
            case GameManager.tutorialEnum.DOWNHLL:
                background.sprite = backgrounds[0];
                dh_paragraph[0].SetActive(true);
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case GameManager.tutorialEnum.SKIJUMP:
                background.sprite = backgrounds[1];
                sj_paragraph[0].SetActive(true);
                Screen.orientation = ScreenOrientation.Landscape;
                break;
        }
    }

    public void nextPage(GameObject nextTarget) {
        nextTarget.SetActive(true);
    }

    public void done() {
        gm.tutorialDone(type);
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
