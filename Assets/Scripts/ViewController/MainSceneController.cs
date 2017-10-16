using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour {
    public void downhillGameLoad() {
        SceneManager.LoadScene("Ski");
    }

    public void skeletonGameLoad() {
        SceneManager.LoadScene("Skeleton");
    }
}
