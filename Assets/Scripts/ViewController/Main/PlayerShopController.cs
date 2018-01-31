using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class PlayerShopController : MonoBehaviour {
	private SaveManager sm;
	private CharacterManager cm;
	private SoundManager sound;
	[SerializeField] private CharStat charStat;
	[SerializeField] private ResourceController topPanel;
	[SerializeField] private Button leftBtn;
	[SerializeField] private Button rightBtn;
	[SerializeField] private Sprite[] characterSprite;
	[SerializeField] private Button buyCrystalBtn;
	[SerializeField] private Button buyPointBtn;
	[SerializeField] private Button currentBtn;
	[SerializeField] private Button closeBtn;
	[SerializeField] private GameObject modal;
	[SerializeField] private GameObject tutorialModal;
	private int charNum;


	private void Awake() {
		sm = SaveManager.Instance;
		cm = CharacterManager.Instance;
		sound = SoundManager.Instance;
	}

	private void Start() {
		charNum = cm.currentCharacter;
		setButton();
		if(GameManager.Instance.isTutorial(GameManager.tutorialEnum.CHARACTER)) return;
        tutorialModal.SetActive(true);
        GameManager.Instance.tutorialDone(GameManager.tutorialEnum.CHARACTER);
	}

	private void OnEnable() {
		setCharData();
	}

	private void FixedUpdate() {
		charStat.setTime(cm.getMaxEntry(charNum), cm.getCurrentEntry(charNum), cm.getLeftTime(charNum));
	}

	private void setButton() {
		leftBtn.onClick.AddListener(() => {--charNum; setCharData();});
		rightBtn.onClick.AddListener(() => {++charNum; setCharData();});
		buyCrystalBtn.onClick.AddListener(buyCrystal);
		buyPointBtn.onClick.AddListener(buyPoint);
		closeBtn.onClick.AddListener(closeSelect);
		currentBtn.onClick.AddListener(charSelected);
	}

	private void setCharData() {
		if(charNum >= 3) charNum=0;
		if(charNum < 0) charNum=2;
		setText();
		setBuy();
	}

	private void setText() {
		charStat.setData(characterSprite[charNum], cm.getName(charNum), cm.getSpeed(charNum), cm.getControl(charNum));
	}

	private void setBuy() {
		bool isSold = cm.buyIt(charNum);
		currentBtn.gameObject.SetActive(isSold);
		buyCrystalBtn.gameObject.SetActive(!isSold);
		buyPointBtn.gameObject.SetActive(!isSold);
		if(charNum == cm.currentCharacter)
			currentBtn.GetComponentInChildren<Text>().text = LocalizationManager.GetTranslation("char_selected");
		else
			currentBtn.GetComponentInChildren<Text>().text = LocalizationManager.GetTranslation("char_select");
		buyCrystalBtn.GetComponentInChildren<Text>().text = string.Format("{0} {1}", cm.getPriceCrystal(charNum), translate("shop_buy1"));
		buyPointBtn.GetComponentInChildren<Text>().text = string.Format("{0} {1}", cm.getPricePoint(charNum), translate("shop_buy2"));
	}

	private void buyCrystal() {
		bool canBuy = sm.getCrystalLeft() >= cm.getPriceCrystal(charNum);
		setModal(canBuy, true);
	}

	private void buyPoint() {
		bool canBuy = sm.getPointLeft() >= cm.getPricePoint(charNum);
		setModal(canBuy, false);
	}

	private void setModal(bool canBuy, bool isCrystal) {
		Button btn = modal.transform.Find("Panel/Buttons/YES").GetComponent<Button>();
		Text closeBtnText = modal.transform.Find("Panel/Buttons/NO/Text").GetComponent<Text>();
		Text text = modal.GetComponentInChildren<Text>();
		btn.onClick.RemoveAllListeners();
		btn.gameObject.SetActive(canBuy);
		text.text = canBuy ? translate("modal_buy") : translate("modal_refuse");
		closeBtnText.text = canBuy ? translate("modal_no") : translate("modal_check");
		modal.SetActive(true);
		modal.GetComponent<AndroidBackOverride>().beforeModal = gameObject;
		if(!canBuy) return;
		btn.onClick.AddListener(() => {
			if(isCrystal) {
				if(sm.useCrystal(cm.getPriceCrystal(charNum)))
					sold();
			} else {
				if(sm.usePoint(cm.getPricePoint(charNum)))
					sold();
			}
		});
	}

	private void sold() {
		cm.sold(charNum);
		modal.SetActive(false);
		topPanel.setData();
		setCharData();
	}

	private void charSelected() {
		cm.currentCharacter = charNum;
		closeSelect();
	}

	private void closeSelect() {
		gameObject.SetActive(false);
		transform.parent.GetChild(1).SendMessage("setCharData", SendMessageOptions.DontRequireReceiver);
		Button btn = modal.transform.Find("Panel/Buttons/YES").GetComponent<Button>();
		Text closeBtnText = modal.transform.Find("Panel/Buttons/NO/Text").GetComponent<Text>();
		btn.gameObject.SetActive(true);
		btn.onClick.RemoveAllListeners();
		modal.GetComponentInChildren<Text>().text = translate("modal_buy");
		closeBtnText.text = translate("modal_no");
		//sound.Play(SoundManager.SoundType.EFX, "closeBtn");
	}
	
	private string translate(string str) {
		return LocalizationManager.GetTranslation(str);
	}

}
