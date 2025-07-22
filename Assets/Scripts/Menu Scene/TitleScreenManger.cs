using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManger : MonoBehaviour
{
    public static TitleScreenManger Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject titleScreenMainMenu;
    [SerializeField] private GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] private Button loadMenuReturnButton;
    [SerializeField] private Button mainMenuLoadGameButton;
    [SerializeField] private Button mainMenuNewGameButton;
    [SerializeField] private Button deleteCharacterPopUpConfirmButton;

    [Header("Pop Ups")]
    [SerializeField] private GameObject noCharacterSlotsPopUp;
    [SerializeField] private Button noCharacterSlotsOkeyButton;
    [SerializeField] private GameObject deleteCharacterSlotPopUp;

    [Header("Character Slots")]
    [SerializeField] private CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.Instance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);

        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        titleScreenLoadMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);

        mainMenuLoadGameButton.Select();
    }

    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkeyButton.Select();
    }

    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuNewGameButton.Select();
    }

    public void SelectCharacterSlot(CharacterSlot characterSlot)
    {
        currentSelectedSlot = characterSlot;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlot.NO_SLOT;
    }

    public void AttempToDeleteCharacterSlot()
    {
        if (currentSelectedSlot != CharacterSlot.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmButton.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.Instance.DeleteGame(currentSelectedSlot);

        //  REFRESH THE SLOTS
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);

        loadMenuReturnButton.Select();
    }

    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();
    }
}
