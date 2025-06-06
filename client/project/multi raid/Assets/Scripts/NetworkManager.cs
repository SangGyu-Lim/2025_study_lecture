using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManager : Singleton<NetworkManager>
{
    [System.Serializable]
    public class PostData
    {
        public API_TYPE api;
        public string packetName;
        public string packetData;
    }

    [System.Serializable]
    public class LoginData
    {
        public string token;
        public string message;
    }

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

    public void SendServer(API_TYPE api, string packetName, string packetData)
    {
        Debug.Log(api);
        StartCoroutine(ServerCall(api, packetName, packetData));
    }

    IEnumerator ServerCall(API_TYPE api, string packetName, string packetData)
    {
        PostData data = new PostData
        {
            api = api,
            packetName = "test_user",
            packetData = "123456"
        };
        string json = JsonUtility.ToJson(data);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.serverURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);
            Debug.Log("¿¿¥‰: " + request.downloadHandler.text);

            //GameDataManager.Instance.token = res.token;

        }
        else
        {
            Debug.LogError("POST Ω«∆–: " + request.error);
        }

    }

}
