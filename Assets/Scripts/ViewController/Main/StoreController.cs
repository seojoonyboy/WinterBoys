using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour {
    private SaveManager saveManager;
    private SoundManager soundManager;
    [SerializeField] private ResourceController resourceController;
    [SerializeField] private Toggle[] toggle;
    [SerializeField] private Sprite[] toggleSprite;
    [SerializeField] private Button[] crystalButton;
    [SerializeField] private Button staminaButton;
    [SerializeField] private Button modalButton;

    private void Awake() {
        saveManager = SaveManager.Instance;
        soundManager = SoundManager.Instance;
    }

    private void Start() {
        setToggle();
        setButton();
    }

    private void setToggle() {
        toggle[0].onValueChanged.AddListener((value) => toggleChange(toggle[0], value));
        toggle[1].onValueChanged.AddListener((value) => toggleChange(toggle[1], value));
    }

    private void setButton() {
        crystalButton[0].onClick.AddListener(() => setModalButton(100));
        crystalButton[1].onClick.AddListener(() => setModalButton(400));
        crystalButton[2].onClick.AddListener(() => setModalButton(1000));
    }

    private void setModalButton(int crystal) {
        modalButton.transform.parent.parent.gameObject.SetActive(true);
        soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        modalButton.onClick.RemoveAllListeners();
        modalButton.onClick.AddListener(() => purchaseDone(crystal));
    }

    private void purchaseDone(int crystal) {
        saveManager.addCrystal(crystal);
        soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        resourceController.setData();
        modalButton.transform.parent.parent.gameObject.SetActive(false);
    }

    private void toggleChange(Toggle toggle, bool value) {
        if(value) {
            toggle.GetComponent<Image>().sprite = toggleSprite[0];
            soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        }
        else toggle.GetComponent<Image>().sprite = toggleSprite[1];
    }

}
