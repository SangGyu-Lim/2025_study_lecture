using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager>
{

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

        // todo REGISTER_URL에서 왜 ProtocolError?
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답: " + request.downloadHandler.text);

            HandleResponse(api, request.downloadHandler.text);
           
            onResult?.Invoke(true);

        }
        else
        {
            Debug.LogError("POST 실패: " + request.error);
            onResult?.Invoke(false);
        }

    }



    public void SendServerPost(string api, object packet, Action<bool> onResult)
    {
        Debug.Log(api);
        StartCoroutine(ServerCallPost(api, packet, onResult));
    }

    IEnumerator ServerCallPost(string api, object packet, Action<bool> onResult)
    {
        string json = JsonUtility.ToJson(packet);

        Debug.LogError("before return www");
        UnityWebRequest request = new UnityWebRequest(CommonDefine.WEB_POST_BASE_URL + api, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("authorization", GameDataManager.Instance.loginData.sessionId);

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답: " + request.downloadHandler.text);

            HandleResponse(api, request.downloadHandler.text);
            //GameDataManager.Instance.token = res.token;
            onResult?.Invoke(true);
        }
        else
        {
            Debug.LogError("POST 실패: " + request.error);
            onResult?.Invoke(false);
        }

    }

    public void SendServerGet(string api, List<ServerPacket> packetList, Action<bool> onResult)
    {
        Debug.Log(api);
        StartCoroutine(ServerCallGet(api, packetList, onResult));
    }

    IEnumerator ServerCallGet(string api, List<ServerPacket> packetList, Action<bool> onResult)
    {
        string packetStr = "";
        if(packetList != null)
        {
            for (int i = 0; i < packetList.Count; ++i)
            {
                if (packetStr.Length > 0)
                    packetStr += "&";

                ServerPacket packet = packetList[i];
                packetStr += packet.packetType + "=" + packet.packetValue;
            }
        }

        Debug.LogError("before return www");
        string url = CommonDefine.WEB_POST_BASE_URL + api;
        if (packetStr.Length > 0)
        {
            url += "?" + packetStr;
        }

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("authorization", GameDataManager.Instance.loginData.sessionId);

        yield return request.SendWebRequest();

        Debug.LogError("after return www");

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답: " + request.downloadHandler.text);

            HandleResponse(api, request.downloadHandler.text);
            //GameDataManager.Instance.token = res.token;
            onResult?.Invoke(true);
        }
        else
        {
            Debug.LogError("GET 실패: " + request.error);
            onResult?.Invoke(false);
        }

    }

    void HandleResponse(string api, string data)
    {
        if (string.IsNullOrEmpty(api) || string.IsNullOrEmpty(data))
            return;

        switch(api)
        {
            case CommonDefine.LOGIN_URL:
                {
                    GameDataManager.Instance.loginData = JsonUtility.FromJson<LoginData>(data);
                }
                break;
            case CommonDefine.GET_MY_POKEMON_URL:
                {
                    GameDataManager.Instance.myPokemonList = JsonHelper.FromJson<MyPokemon>(data);
                }
                break;
            case CommonDefine.SHOP_LIST_URL:
                {
                    GameDataManager.Instance.pokemonShopList = JsonHelper.FromJson<PokemonShop>(data);
                }
                break;
            case CommonDefine.MAKE_ROOM_URL:
                {
                    GameDataManager.Instance.myRoomInfo = JsonUtility.FromJson<Room>(data);
                }
                break;
            case CommonDefine.ROOM_LIST_URL:
                {
                    GameDataManager.Instance.roomList = JsonHelper.FromJson<Room>(data);
                }
                break;
                
        }
    }

    #endregion

    #region WEB_SOCKET

    private SocketIO client = null;
    public string roomId = "room123";

    public async Task ConnectSocket()
    {
        //string packetStr = "?sessionId=" + GameDataManager.Instance.loginData.sessionId;
        string packetStr = "";

        if (client == null)
        {
            var payload = new Dictionary<string, string>
            {
                { "sessionId", GameDataManager.Instance.loginData.sessionId },
            };

            client = new SocketIO(CommonDefine.WEB_SOCKET_URL + packetStr, new SocketIOOptions
            {
                Query = payload,
                Reconnection = true,
                ReconnectionAttempts = 5,
                ReconnectionDelay = 1000,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            // 이벤트 등록
            client.OnConnected += OnConnected;
            client.On("roomUpdate", OnRoomUpdate);
            
            await client.ConnectAsync();
        }
    }

    private async void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected to Socket.IO server");
        Debug.Log("Connected : " + client.Connected);

        await client.EmitAsync("getRooms");
    }

    public async void JoinRoom(string roomId)
    {
        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId },
        };

        await client.EmitAsync("joinRoom", payload);
    }

    private void OnRoomUpdate(SocketIOResponse response)
    {
        try
        {
            // todo 다른 유저들이 update되지 않음
            string json = response.GetValue().ToString();
            GameDataManager.Instance.myRoomInfo = JsonUtility.FromJson<Room>(json);
            Debug.Log($"RoomUpdate: {json}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"RoomUpdate error: {ex.Message}");
        }
    }

    public async Task SendMessageToRoom(string messageText)
    {
        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId },
            { "message", messageText }
        };

        await client.EmitAsync("message", payload);
    }

    public async void LeaveRoom(string roomId)
    {
        var payload = new Dictionary<string, string>
        {
            { "roomId", roomId }
        };

        await client.EmitAsync("leaveRoom", payload);
    }



    #endregion

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    async void OnApplicationQuit()
    {
        if (client != null)
            await client.DisconnectAsync();
    }
}
