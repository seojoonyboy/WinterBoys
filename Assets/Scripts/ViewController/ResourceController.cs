using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResourceController : MonoBehaviour {
	private SaveManager saveManager;
	public Text crystalText;
	public Text pointText;

	private void Awake() {
		saveManager = SaveManager.Instance;
	}

	public void setData() {
		crystalText.text = saveManager.getCrystalLeft().ToString("00000");
		pointText.text = saveManager.getPointLeft().ToString("00000");
	}
}
