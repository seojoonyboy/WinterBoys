using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour {
    public float cooltime;

    private float currentTime = 0;
    private void Start() {
        GetComponent<Image>().fillAmount = 0;
        currentTime = 0;
    }

    private void Update() {
        currentTime += Time.deltaTime;
        float val = currentTime / cooltime;
        GetComponent<Image>().fillAmount = val;

        if(val > 1) {
            Destroy(transform.parent.gameObject);
        }
    }
}
