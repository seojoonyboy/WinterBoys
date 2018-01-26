using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontEffect : MonoBehaviour {
    RectTransform rect;
    Text textComp;

    [HideInInspector] public string text;
    [HideInInspector] public bool isNegative;

    public float 
        moveAmount,
        coolTime;

    public byte alphaAmount;
    Vector3 originPos;
    float leftTime;

    private Color32 positiveColor = new Color32(44, 0, 152, 255);
    private Color32 negativeColor = new Color32(245, 40, 69, 255);

    private void Awake() {
        rect = GetComponent<RectTransform>();
        textComp = GetComponent<Text>();

        originPos = rect.localPosition;
        leftTime = coolTime;
    }

    private void OnEnable() {
        textComp.text = text;
        if (isNegative) {
            textComp.color = negativeColor;
        }
        else {
            textComp.color = positiveColor;
        }
    }


    private void OnDisable() {
        rect.localPosition = originPos;
        leftTime = coolTime;
    }

    private void Update() {
        rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + moveAmount);
        Color32 color = textComp.color;
        if(color.a > 1) {
            color.a -= alphaAmount;
        }
        textComp.color = color;

        leftTime -= Time.deltaTime;
        if(leftTime < 0) {
            Destroy(gameObject);
        }
    }
}
