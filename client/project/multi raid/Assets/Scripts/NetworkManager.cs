using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class NetworkManager : Singleton<NetworkManager>
{
    

    [System.Serializable]
    public class LoginData
    {
        public string token;
        public string message;
    }

    protected override void Awake()
    {
        base.Awake();  // 싱글톤 초기화

        Debug.Log("NetworkManager init");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region WEB_POST

    [System.Serializable]
    public class PostData
    {
        public API_TYPE api;
        public string packetName;
        public string packetData;
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
            packetName = packetName,
            packetData = packetData
        };
        string json = JsonUtility.ToJson(data);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);
            Debug.Log("응답: " + request.downloadHandler.text);

            //GameDataManager.Instance.token = res.token;

        }
        else
        {
            Debug.LogError("POST 실패: " + request.error);
        }

    }

    #endregion

    #region WEB_SOCKET

    private WebSocket ws;

    public void ConnectSocket()
    {
        if (ws == null) {
            ws = new WebSocket(CommonDefine.WEB_SOCKET_URL);

            ws.OnOpen += OnWebSocketOpen;
            ws.OnMessage += OnWebSocketMessage;
            ws.OnError += OnWebSocketError;
            ws.OnClose += OnWebSocketClose;

            ws.Connect();
        }
        
    }

    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("서버와 연결되었습니다.");
        ws.Send("Hello from Unity!");
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("서버 메시지 수신: " + e.Data);
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("에러 발생: " + e.Message);
    }

    private void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("서버 연결 종료됨. 이유: " + e.Reason);
    }



    #endregion

    void OnApplicationQuit()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
