using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager Instance;

    public PlayerManager player;

    [Header("World Scene Index")]
    [SerializeField] private int worldSceneIndex = 1;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("SAVE/LOAD")]
    [SerializeField] private bool saveGame;
    [SerializeField] private bool loadGame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
        
        //AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        if (AttemptToCreateNewGameForGivenCharacterSlot(CharacterSlot.CharacterSlot01)) return;
        if (AttemptToCreateNewGameForGivenCharacterSlot(CharacterSlot.CharacterSlot02)) return;
        if (AttemptToCreateNewGameForGivenCharacterSlot(CharacterSlot.CharacterSlot03)) return;
        if (AttemptToCreateNewGameForGivenCharacterSlot(CharacterSlot.CharacterSlot04)) return;

        //  IF THERE ARE NO FREE SLOTS, NOTIFY PLAYER
        TitleScreenManger.Instance.DisplayNoFreeCharacterSlotsPopUp();


        bool AttemptToCreateNewGameForGivenCharacterSlot(CharacterSlot characterSlot)
        {
            //  CHECK TO SEE IF WE CAN CREATE A NEW SAVE FILE (CHECK FOR OTHER EXISTING FILES FIRST)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            if (!saveFileDataWriter.CheckToSeeIfFileExist())
            {
                //  IF THIS PROFILE SLOT IS NOT TAKEN, MAKE A NEW ONE USING THIS SLOT
                currentCharacterSlotBeingUsed = characterSlot;
                currentCharacterData = new CharacterSaveData();
                StartCoroutine(LoadWorldScene());
                return true;
            }

            return false;
        }
    }

    public void LoadGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    //  LOAD ALL CHARACTER PROFILES ON DEVICE WHEN STARTING GAME
    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot01);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot02);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot03);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot04);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();
    }

    public void SaveGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void DeleteGame(CharacterSlot characterSlot)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
        
        saveFileDataWriter.DeleteSaveFile();
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
    {
        string fileName = "";

        switch (characterSlot)
        {
            case CharacterSlot.CharacterSlot01:
                fileName = "characterSlot01";
                break;
            case CharacterSlot.CharacterSlot02:
                fileName = "characterSlot02";
                break;
            case CharacterSlot.CharacterSlot03:
                fileName = "characterSlot03";
                break;
            case CharacterSlot.CharacterSlot04:
                fileName = "characterSlot04";
                break;
            default:
                break;
        }

        return fileName;
    }
}
