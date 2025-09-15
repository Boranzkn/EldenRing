using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { private set; get; }

    [HideInInspector] public PlayerUIHudManager PlayerUIHudManager { private set; get; }
    [HideInInspector] public PlayerUIPopUpManager PlayerUIPopUpManager { private set; get; }

    [Header("NETWORK JOIN")]
    [SerializeField] private bool startGameAsClient;

    private void Awake()
    {
        Instance = this;

        PlayerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        PlayerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            // FIRST SHUT DOWN, BECAUSE WE HAVE STARTED AS A HOST DURING THE TITLE SCREEN
            NetworkManager.Singleton.Shutdown();
            // THEN RESTART, AS A CLIENT
            NetworkManager.Singleton.StartClient();
        }
    }
}
