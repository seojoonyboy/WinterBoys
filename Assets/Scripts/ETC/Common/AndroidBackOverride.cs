using UnityEngine;

public class AndroidBackOverride : AndroidBack {
	[SerializeField]
	private GameObject beforeModal;
	void OnDisable() {
		Time.timeScale = 1;
		if(!beforeModal.activeSelf) {
			GameManager.Instance.releaseQuitModal();
			return;
		}
		GameManager.Instance.setQuitModal(beforeModal);
		beforeModal.SendMessage("CloseModal", SendMessageOptions.DontRequireReceiver);
		//AudioManager.Instance.playSound("start_button");
	}
}
