using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManager : Singleton<NetworkManager>
{
    protected override void Awake()
    {
        base.Awake();  // ΩÃ±€≈Ê √ ±‚»≠

        Debug.Log("NetworkManager √ ±‚»≠µ ");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendServer(string api)
    {
        Debug.Log(api);
        //StartCoroutine(ServerCall(api));
    }

    IEnumerator ServerCall(string api)
    {
        string serviceName = "";
        

        string url = CommonDefine.serverURL + api + "?" + serviceName;

        Debug.LogError("before return www");
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        //yield return www;

        Debug.LogError("after return www");

        if (www.error == null && www.isDone)
        {
            if (www.downloadHandler.text != "")
            {
                
                //SceneManager.LoadScene("SampleScene");
            }
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("www error : " + www.error);
        }

    }

}
