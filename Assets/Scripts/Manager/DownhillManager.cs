using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;

public class DownhillManager : MonoBehaviour {
    public GameObject modal;
    public void mainLoad() {
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
    }

    private void Start() {
        Time.timeScale = 1;
    }
}
