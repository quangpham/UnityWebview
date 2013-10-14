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
		URL = "http://www.sayduck.com";
		System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/sayduck");
		FILE_PATH = Application.persistentDataPath + "/sayduck/filename.html";
	}
	
	public void Start () {
		DownloadAndSave();
	}

    public void DownloadAndSave()
    {
		Debug.Log ("Save file in :" + FILE_PATH);
		Debug.Log ("Download from :"+ URL);
		
        StartCoroutine(DownloadCoroutine());
    }

    public Dictionary<object, object> GetSavedData()
    {
        // Use ReadContents() and do your MiniJSON magic here
        return null;    
    }

    private IEnumerator DownloadCoroutine()
    {
		Debug.Log ( "in DownloadCoroutine ..." );
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
			
            SaveToDocuments(request.text, "filename.html", "sayduck/");
        }
    }
	
	/*
    private string ReadContents()
    {
        string ret;

        using(FileStream fs = new FileStream(FILE_PATH, FileMode.Open))
        {
            BinaryReader fileReader = new BinaryReader(fs);

            ret = fileReader.ReadString();

            fs.Close();
        }

        return ret;
    }

    private void SaveContents(string text)
    {
        using(FileStream fs = new FileStream(FILE_PATH, FileMode.Create))
        {
            BinaryWriter fileWriter = new BinaryWriter(fs);

            fileWriter.Write(text);

            fs.Close();
			
			Debug.Log ("SaveContents ... Close");
        }
		
		Debug.Log ("Read file");
		Debug.Log (ReadContents());
    }
	
	
	*/
	
	
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