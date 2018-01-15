using UnityEngine;

public class AndroidBackOverride : AndroidBack {
	[SerializeField]
	private GameObject beforeModal;
	void OnDisable() {
		if(!beforeModal) return;
		GameManager.Instance.setQuitModal(beforeModal);
		beforeModal.SendMessage("CloseModal", SendMessageOptions.DontRequireReceiver);
		//AudioManager.Instance.playSound("start_button");
	}
}
