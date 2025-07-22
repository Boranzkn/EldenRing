using TMPro;
using UnityEngine;

public class UI_Character_Save_Slot : MonoBehaviour
{
    [Header("Game Slot")]
    [SerializeField] private CharacterSlot characterSlot;

    [Header("Character Info")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        SaveFileDataWriter saveFileWriter = new SaveFileDataWriter();
        saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

        if (characterSlot == CharacterSlot.CharacterSlot01)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            if (saveFileWriter.CheckToSeeIfFileExist())
            {
                characterName.text = WorldSaveGameManager.Instance.characterSlot01.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot02)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            if (saveFileWriter.CheckToSeeIfFileExist())
            {
                characterName.text = WorldSaveGameManager.Instance.characterSlot02.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot03)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            if (saveFileWriter.CheckToSeeIfFileExist())
            {
                characterName.text = WorldSaveGameManager.Instance.characterSlot03.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlot.CharacterSlot04)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            if (saveFileWriter.CheckToSeeIfFileExist())
            {
                characterName.text = WorldSaveGameManager.Instance.characterSlot04.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.Instance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.Instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManger.Instance.SelectCharacterSlot(characterSlot);
    }
}
