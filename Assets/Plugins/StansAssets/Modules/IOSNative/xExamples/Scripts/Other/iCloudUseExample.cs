////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class iCloudUseExample : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	private float v = 1.1f;

	void Awake() {

		//initialize icloud and listed for events
		iCloudManager.OnCloudInitAction += OnCloundInitAction;
		iCloudManager.OnCloudDataReceivedAction += OnCloudDataReceivedAction;
		iCloudManager.OnStoreDidChangeExternally += HandleOnStoreDidChangeExternally;


//		iCloudManager.Instance.init ();
	

	
	}

	void HandleOnStoreDidChangeExternally (System.Collections.Generic.List<iCloudData> changedData) {
		foreach(iCloudData data in changedData) {
			ISN_Logger.Log("Cloud data with key:  " + data.Key + " was chnaged");
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
		if(GUI.Button(new Rect(170, 70, 150, 50), "Set String")) {
            iCloudManager.Instance.SetString ("TestStringKey", "Hello World");
		}

		if(GUI.Button(new Rect(170, 130, 150, 50), "Get String")) {
			iCloudManager.Instance.RequestDataForKey ("TestStringKey");
		}




		if(GUI.Button(new Rect(330, 70, 150, 50), "Set Float")) {
			v += 1.1f;
			iCloudManager.Instance.SetFloat ("TestFloatKey", v);
		}

		if(GUI.Button(new Rect(330, 130, 150, 50), "Get Float")) {
            iCloudManager.Instance.RequestDataForKey ("TestFloatKey" ,(iCloudData obj) => {

                Debug.Log(obj.FloatValue);
            });

		}


		if(GUI.Button(new Rect(490, 70, 150, 50), "Set Bytes")) {
			string msg = "hello world";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);
			iCloudManager.Instance.SetData ("TestByteKey", data);
		}

		if(GUI.Button(new Rect(490, 130, 150, 50), "Get Bytes")) {
			iCloudManager.Instance.RequestDataForKey ("TestByteKey");
		}

		if(GUI.Button(new Rect(170, 500, 150, 50), "TestConnection")) {
            LoadLevel("Peer-To-PeerGameExample");
		}

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------



	private void OnCloundInitAction (SA.Common.Models.Result result) {
		if(result.IsSucceeded) {
			IOSNativePopUpManager.showMessage("iCloud", "Initialization Success!");
		} else {
			IOSNativePopUpManager.showMessage("iCloud", "Initialization Failed!");
		}
	}

	private void OnCloudDataReceivedAction (iCloudData data) {
		if(data.IsEmpty) {
            IOSNativePopUpManager.showMessage(data.Key, "data is empty");
		} else {
			IOSNativePopUpManager.showMessage(data.Key, data.StringValue);
		}
	}	
	
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

	void OnDestroy() {
		iCloudManager.OnCloudInitAction -= OnCloundInitAction;
		iCloudManager.OnCloudDataReceivedAction -= OnCloudDataReceivedAction;
	}

}
