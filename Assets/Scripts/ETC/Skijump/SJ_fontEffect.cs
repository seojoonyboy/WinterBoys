using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SJ_fontEffect : MonoBehaviour {
    Text textComp;

    [HideInInspector] public string text;
    [HideInInspector] public bool isNegative;

    public float
        moveAmount,
        coolTime;

    public byte alphaAmount;
    Vector3 originPos;
    float leftTime;

    private Color32 positiveColor = new Color32(255, 255, 255, 255);
    private Color32 negativeColor = new Color32(245, 40, 69, 255);

    public GameObject target;
    Canvas canvas;
    RectTransform rect;
    RectTransform canvasRect;

    private void Awake() {
        textComp = transform.Find("Text").GetComponent<Text>();
        rect = GetComponent<RectTransform>();
        canvas = transform.parent.GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

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
        leftTime = coolTime;
    }

    private void Update() {
        Color32 color = textComp.color;
        if (color.a > 5) {
            color.a -= alphaAmount;
        }
        else {
            Destroy(gameObject);
        }

        leftTime -= Time.deltaTime;

        if (leftTime < 0) {
            Destroy(gameObject);
        }

        textComp.color = color;

        Vector2 viewPos = Camera.main.WorldToViewportPoint(target.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((viewPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
        );

        rect.anchoredPosition = WorldObject_ScreenPosition;
    }
}
