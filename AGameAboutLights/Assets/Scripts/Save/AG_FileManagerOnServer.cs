using UnityEngine;
using System.Collections;

public class AG_FileManagerOnServer : AG_Singleton<AG_FileManagerOnServer>
{
    #if UNITY_WEBGL
    private string _wwwInfo;
    public string GetData(string url)
    {        
        StartCoroutine(GetFileOnServer(url.Remove(0,1)));
        return _wwwInfo;
    }
    
    private IEnumerator GetFileOnServer(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        _wwwInfo = www.text;
    }
    #endif
}
