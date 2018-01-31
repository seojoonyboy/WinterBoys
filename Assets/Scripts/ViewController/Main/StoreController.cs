using System;
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
    [SerializeField] private Button[] pointButton;
    [SerializeField] private Button staminaButton;
    [SerializeField] private Button modalButton;
    [SerializeField] private GameObject tutorialModal;
	private string[] PRODUCT_ID = {"crystal100", "crystal400", "crystal1000"};
    private int[] crystalAmount = {100, 400, 1000};
    private int buyingItem;
    private int[] pointAmount = {3000, 10000, 30000};
    private int[] pointPrice = {100, 200, 400, 550};

    private void Awake() {
        saveManager = SaveManager.Instance;
        soundManager = SoundManager.Instance;
    }

    private void Start() {
        setToggle();
        setButton();
        if(GameManager.Instance.isTutorial(GameManager.tutorialEnum.SHOP)) return;
        tutorialModal.SetActive(true);
        GameManager.Instance.tutorialDone(GameManager.tutorialEnum.SHOP);
    }

    private void setToggle() {
        toggle[0].onValueChanged.AddListener((value) => toggleChange(toggle[0], value));
        toggle[1].onValueChanged.AddListener((value) => toggleChange(toggle[1], value));
    }

    private void setButton() {
        for(int i = 0; i < crystalButton.Length; i++) {
            int num = i;
            crystalButton[i].onClick.AddListener(() => setModalCrystal(crystalAmount[num]));
            crystalButton[i].transform.GetChild(1).GetComponent<Text>().text = UM_InAppPurchaseManager.InAppProducts[i].LocalizedPrice;
            pointButton[i].onClick.AddListener(() => setModalPoint(num));
        }
        staminaButton.onClick.AddListener(()=>setModalPoint(3));
    }

    private void setModalCrystal(int crystal) {
        modalButton.transform.parent.parent.parent.gameObject.SetActive(true);
        modalButton.GetComponentInParent<AndroidBackOverride>().beforeModal = gameObject;
        soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        modalButton.onClick.RemoveAllListeners();
        modalButton.onClick.AddListener(() => purchaseCrystal(crystal));
    }

	public void purchaseCrystal(int crystal) {
		buyingItem = crystal;
		UM_InAppPurchaseManager.Client.OnPurchaseFinished += OnPurchaseFlowFinishedAction;
        soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        int num = crystal == 100 ? 0 : crystal == 400 ? 1 : 2;
		UM_InAppPurchaseManager.Client.Purchase(PRODUCT_ID[num]);
	}

    private void OnPurchaseFlowFinishedAction(UM_PurchaseResult result) {
        UM_InAppPurchaseManager.Client.OnPurchaseFinished -= OnPurchaseFlowFinishedAction;
        if(!result.isSuccess) return;
        purchaseDone();
    }

    private void purchaseDone() {
        saveManager.addCrystal(buyingItem);
        resourceController.setData();
        modalButton.transform.parent.parent.parent.gameObject.SetActive(false);
    }

    private void setModalPoint(int num) {
        modalButton.transform.parent.parent.parent.gameObject.SetActive(true);
        Text noBtn = modalButton.transform.parent.GetChild(1).GetChild(0).GetComponent<Text>();
        Text info = modalButton.transform.parent.parent.GetComponentInChildren<Text>();
        modalButton.GetComponentInParent<AndroidBackOverride>().beforeModal = gameObject;
        soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        modalButton.onClick.RemoveAllListeners();

        if(saveManager.getCrystalLeft() < pointPrice[num]) {
            modalButton.gameObject.SetActive(false);
            noBtn.text = I2.Loc.LocalizationManager.GetTranslation("modal_check");
            info.text = I2.Loc.LocalizationManager.GetTranslation("modal_refuse");
        } 
        else {
            modalButton.gameObject.SetActive(true);
            noBtn.text = I2.Loc.LocalizationManager.GetTranslation("modal_no");
            info.text = I2.Loc.LocalizationManager.GetTranslation("modal_buy");
            modalButton.onClick.AddListener(() => purchasePoint(num));
        }
        
    }

    private void purchasePoint(int num) {
        modalButton.transform.parent.parent.parent.gameObject.SetActive(false);
        if(num == 3) {chargeStamina(num); return;}
        if(!saveManager.useCrystal(pointPrice[num])) return;
        saveManager.addPoint(pointAmount[num]);
        resourceController.setData();
    }

    private void chargeStamina(int num) {
        if(!saveManager.useCrystal(pointPrice[num])) return;
        CharacterManager.Instance.purchaseCharacter();
        resourceController.setData();
    }

    private void toggleChange(Toggle toggle, bool value) {
        if(value) {
            toggle.GetComponent<Image>().sprite = toggleSprite[0];
            soundManager.Play(SoundManager.SoundType.EFX, "gameSelBtn");
        }
        else toggle.GetComponent<Image>().sprite = toggleSprite[1];
    }
}
