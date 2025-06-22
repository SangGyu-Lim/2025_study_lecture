using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    public string sessionId = "8335e46c-4900-45ca-ad25-63c41598269e";

    protected override void Awake()
    {
        base.Awake();  // ΩÃ±€≈Ê √ ±‚»≠

        Debug.Log("GameDataManager init");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
