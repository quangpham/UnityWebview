using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class MyButtonClick : MonoBehaviour {
	void OnClick() {
		string filename = Application.persistentDataPath + "/sayduck/filename.html";
		addSubWebview(filename);
	}
	
	private void addSubWebview(string webUrl) {
		IOSWebViewObject webViewObject = gameObject.AddComponent<IOSWebViewObject>();
		webViewObject.InitWithMargins(0, 0, 0, 500);
		webViewObject.LoadURL(webUrl);
		webViewObject.SetVisibility(true, (callbackMsg)=>{
			Debug.Log("Callback msg: " + callbackMsg);
			
			if (callbackMsg == "AnimateOn") {
				Debug.Log ("AnimateOn");
			}
		});
	}
	
	
	/*
		private void handlePressedWebview(string webUrl)
	{
#if UNITY_IPHONE
		
		if (gameObject.GetComponent<IOSWebViewObject>() == null) {
			addSubWebview(webUrl);
		} else {
			IOSWebViewObject webViewObject = gameObject.GetComponent<IOSWebViewObject>();
			
			webViewObject.SetVisibility(false, (callbackMsg)=>{
				if (callbackMsg == "AnimateOff") {
					SUBVIEW_ACTIVE = false;
					if (webUrl == currentWebviewUrl) {
						// Same view active, animate off
						currentWebviewUrl = "";
						Destroy(webViewObject);
					} else {
						// different view active, animate it off first
						Destroy(webViewObject);
						addSubWebview(webUrl);
					}
					
				}
			});
		}
#endif
	}
	
	private void addSubWebview(string webUrl) {
		IOSWebViewObject webViewObject = gameObject.AddComponent<IOSWebViewObject>();
		webViewObject.InitWithMargins(0, 0, 0, (int)navTabBg.transform.localScale.y);
		webViewObject.LoadURL(webUrl);
		webViewObject.SetVisibility(true, (callbackMsg)=>{
			if (callbackMsg == "AnimateOn") {
				SUBVIEW_ACTIVE = true;
				currentWebviewUrl = webUrl;
			}
		});
	}
	*/
}
