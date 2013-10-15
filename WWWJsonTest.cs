using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WWWJsonTest : MonoBehaviour
{
    private const float SECONDS_BEFORE_TIMEOUT = 10;

    private string URL;

    private string FILE_PATH;
	
	public void OnEnable () {
		URL = "https://dl.dropboxusercontent.com/u/14181582/_temp/bin/index.html";
		System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/sayduck");
	}
	
	public void Start () {
		DownloadAndSave();
	}

    public void DownloadAndSave()
    {
        StartCoroutine(DownloadCoroutine());
    }

    public Dictionary<object, object> GetSavedData()
    {
        // Use ReadContents() and do your MiniJSON magic here
        return null;    
    }

    private IEnumerator DownloadCoroutine()
    {
        var requestHeaders = new Hashtable()
        {
			/*
            { "Connection", "close"},
            { "Accept", "application/json"}
            */
        };

        using(var request = new WWW(URL, null, requestHeaders))
        {
            float timeStarted = Time.realtimeSinceStartup;

            while(!request.isDone)
            {
                // Check if the download times out
                if(Time.realtimeSinceStartup - timeStarted > SECONDS_BEFORE_TIMEOUT)
                {
                    Debug.Log("Download timed out");
                    yield break;
                }

                yield return null;
            }

            // Check for other errors
            if(request.error != null)
            {
                Debug.Log(request.error);

                yield break;
            }
			
			Debug.Log (request.text);
			
            SaveToDocuments(request.text, "index.html", "sayduck/");
        }
    }
	
	void SaveToDocuments(string savedData, string fileName, string directory = "")
	{
		if(directory != ""){
			if(!Directory.Exists(Application.persistentDataPath + "/" +  directory)){
				Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
			}
			fileName = directory + fileName;
		}
		
		string fullPath = Application.persistentDataPath + "/" + fileName;
		
		StreamWriter sw = File.CreateText(fullPath);
		sw.Write(savedData);
		sw.Flush();
		sw.Close();
	}
}