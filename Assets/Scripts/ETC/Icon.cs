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

        if(currentTime <= cooltime) {
            GetComponent<Image>().fillAmount = (float)(currentTime / cooltime);
        }
        if(GetComponent<Image>().fillAmount > 0.99) {
            Destroy(transform.parent.gameObject);
        }
    }
}
