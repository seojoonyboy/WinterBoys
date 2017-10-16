using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DownhillManager : MonoBehaviour {
    public GameObject modal;

    public void mainLoad() {
        SceneManager.LoadScene("Main");
    }

    private void Start() {
        Time.timeScale = 1;
    }
}
