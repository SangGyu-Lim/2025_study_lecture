using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class NetworkManager : Singleton<NetworkManager>
{
    [System.Serializable]
    public class LoginPostData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginData
    {
        public string sessionId;
        public string username;
    }

    [System.Serializable]
    public class PostData
    {
        public API_TYPE api;
        public string packetName;
        public string packetData;
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

    

    

    public void SendLoginServer(string api, string username, string password, Action<bool> onResult)
    {
        Debug.Log(api);
        StartCoroutine(ServerLoginCall(api, username, password, onResult));
    }

    IEnumerator ServerLoginCall(string api, string username, string password, Action<bool> onResult)
    {
        LoginPostData data = new LoginPostData
        {
            username = username,
            password = password
        };
        string json = JsonUtility.ToJson(data);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_BASE_URL + api, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답: " + request.downloadHandler.text);

            if (api == CommonDefine.LOGIN_URL)
            {
                LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);

                GameDataManager.Instance.sessionId = res.sessionId;
            }

            onResult?.Invoke(true);

        }
        else
        {
            Debug.LogError("POST 실패: " + request.error);
            onResult?.Invoke(false);
        }

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
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_BASE_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            //LoginData res = JsonUtility.FromJson<LoginData>(request.downloadHandler.text);
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

    private SocketIO client;
    public string roomId = "room123";

    public async Task ConnectSocket()
    {
        client = new SocketIO(CommonDefine.WEB_SOCKET_URL, new SocketIOOptions
        {
            Reconnection = true,
            ReconnectionAttempts = 5,
            ReconnectionDelay = 1000,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        // 이벤트 등록
        client.OnConnected += OnConnected;
        client.On("RoomUpdate", OnRoomUpdate);
        client.On("MessageResponse", OnMessageResponse);

        await client.ConnectAsync();
    }

    private async void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected to Socket.IO server");

        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId }
        };

        await client.EmitAsync("joinRoom", payload);
    }

    private void OnRoomUpdate(SocketIOResponse response)
    {
        try
        {
            string json = response.GetValue().ToString();
            Debug.Log($"RoomUpdate: {json}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomUpdate error: {ex.Message}");
        }
    }

    private void OnMessageResponse(SocketIOResponse response)
    {
        try
        {
            JsonElement json = response.GetValue();

            string from = json.GetProperty("from").GetString();
            string message = json.GetProperty("message").GetString();

            Debug.Log($"Message from {from}: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"MessageResponse error: {ex.Message}");
        }
    }

    public async void SendMessageToRoom(string messageText)
    {
        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId },
            { "message", messageText }
        };

        await client.EmitAsync("message", payload);
    }

    public async void LeaveRoom()
    {
        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId }
        };

        await client.EmitAsync("leaveRoom", payload);
    }



    #endregion

    async void OnApplicationQuit()
    {
        if (client != null)
            await client.DisconnectAsync();
    }
}
