using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShopController : MonoBehaviour {
	private SaveManager saveManager;
	private CharacterManager characterManager;
	private SoundManager soundManager;
	[SerializeField] private ResourceController topPanel;
	[SerializeField] private Button leftBtn;
	[SerializeField] private Button rightBtn;
	[SerializeField] private Image characterImage;
	[SerializeField] private Sprite[] characterSprite;
	[SerializeField] private Text nameText;
	[SerializeField] private Text statText;
	[SerializeField] private Button buyCrystalBtn;
	[SerializeField] private Button buyPointBtn;
	[SerializeField] private Button currentBtn;
	[SerializeField] private Button closeBtn;
	[SerializeField] private GameObject modal;
	private int charNum;


	private void Awake() {
		saveManager = SaveManager.Instance;
		characterManager = CharacterManager.Instance;
		soundManager = SoundManager.Instance;
	}

	private void Start() {
		charNum = characterManager.currentCharacter;
		setButton();
	}

	private void OnEnable() {
		setCharData();
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
		characterImage.sprite = characterSprite[charNum];

		nameText.text = characterManager.getName(charNum);

		int speed = characterManager.getSpeed(charNum);
		int control = characterManager.getControl(charNum);
		do {
			if(speed != 0 && control != 0) {
				statText.text = string.Format("속도 + {0}%, 조작 + {1}%", speed, control);
				break;
			}
			if(speed == 0) {
				statText.text = string.Format("조작 + {0}%", control);
				break;
			}
			if(control == 0) {
				statText.text = string.Format("속도 + {0}%", speed);
				break;
			}
		} while(false);
		
		//출전기회 코딩 예정
	}

	private void setBuy() {
		bool isSold = characterManager.buyIt(charNum);
		currentBtn.gameObject.SetActive(isSold);
		buyCrystalBtn.gameObject.SetActive(!isSold);
		buyPointBtn.gameObject.SetActive(!isSold);
		if(charNum == characterManager.currentCharacter)
			currentBtn.GetComponentInChildren<Text>().text = "출전중...";
		else
			currentBtn.GetComponentInChildren<Text>().text = "출전";
		buyCrystalBtn.GetComponentInChildren<Text>().text = string.Format("{0} 보석", characterManager.getPriceCrystal(charNum));
		buyPointBtn.GetComponentInChildren<Text>().text = string.Format("{0} 포인트", characterManager.getPricePoint(charNum));
	}

	private void buyCrystal() {
		Button btn = modal.GetComponentInChildren<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => {
			if(saveManager.useCrystal(characterManager.getPriceCrystal(charNum)))
				sold();
			else
				Debug.Log("구입 불가");
		});
		modal.SetActive(true);
	}

	private void buyPoint() {
		Button btn = modal.GetComponentInChildren<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => {
			if(saveManager.usePoint(characterManager.getPricePoint(charNum)))
				sold();
			else 
				Debug.Log("구입 불가");
		});
		modal.SetActive(true);
	}

	private void sold() {
		characterManager.sold(charNum);
		modal.SetActive(false);
		gameObject.SetActive(false);
	}

	private void charSelected() {
		characterManager.currentCharacter = charNum;
		closeSelect();
	}

	private void closeSelect() {
		gameObject.SetActive(false);
		//soundManager.Play(SoundManager.SoundType.EFX, "closeBtn");
	}
	
}
