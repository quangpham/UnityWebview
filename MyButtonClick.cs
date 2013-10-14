using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class MyButtonClick : MonoBehaviour {
	void OnClick() {
		string filename = Application.persistentDataPath + "/sayduck/filename.html";
		Debug.Log (filename);
		Debug.Log (ReadContents(filename));
		
		addSubWebview(filename);
	}
	
	private void addSubWebview(string webUrl) {
		IOSWebViewObject webViewObject = gameObject.AddComponent<IOSWebViewObject>();
		webViewObject.InitWithMargins(0, 0, 0, 500);
		webViewObject.LoadURL(webUrl);
		webViewObject.SetVisibility(true, (callbackMsg)=>{
			if (callbackMsg == "AnimateOn") {
				Debug.Log ("AnimateOn");
			}
		});
	}
	
	private string ReadContents(string filename)
    {
        string ret;

        using(FileStream fs = new FileStream(filename, FileMode.Open))
        {
            BinaryReader fileReader = new BinaryReader(fs);

            ret = fileReader.ReadString();

            fs.Close();
        }

        return ret;
    }
}
