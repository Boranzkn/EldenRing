using Unity.Netcode;
using UnityEngine;

public class TitleScreenManger : MonoBehaviour
{
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
