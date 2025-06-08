using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    protected override void Awake()
    {
        base.Awake();  // ΩÃ±€≈Ê √ ±‚»≠

        Debug.Log("GameManager init");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitBtn();
    }

    void InitBtn()
    {
        //canvas.Find("RoomBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        //canvas.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        canvas.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickInventory()
    {
        GameObject InvenPrefab = Resources.Load<GameObject>("prefabs/Inventory");

        GameObject inven = GameObject.Instantiate(InvenPrefab, canvas);

        for (int i = 0; i < 3; i++)
        {
            GameObject InvenItemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");

            GameObject invenItem = GameObject.Instantiate(InvenItemPrefab, inven.transform.Find("Scroll View/Viewport/Content"));
        }

    }
    
}
