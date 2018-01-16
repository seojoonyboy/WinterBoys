using UnityEngine;

public class AndroidBackOverride : AndroidBack {
	public GameObject beforeModal;
	void OnDisable() {
		Time.timeScale = 1;
		if(!beforeModal.activeSelf) {
			GameManager.Instance.releaseQuitModal();
			return;
		}
		GameManager.Instance.setQuitModal(beforeModal);
		beforeModal.SendMessage("CloseModal", SendMessageOptions.DontRequireReceiver);
		SoundManager.Instance.Play(SoundManager.SoundType.EFX,"gameSelBtn");
	}
}
