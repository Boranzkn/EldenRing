using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager PlayerAnimatorManager { private set; get; }
    [HideInInspector] public PlayerLocalmotionManager PlayerLocalmotionmanager { private set; get; }
    [HideInInspector] public PlayerNetworkManager PlayerNetworkManager { private set; get; }
    [HideInInspector] public PlayerStatsManager PlayerStatsManager { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        PlayerLocalmotionmanager = GetComponent<PlayerLocalmotionManager>();
        PlayerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        PlayerNetworkManager = GetComponent<PlayerNetworkManager>();
        PlayerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner)
        {
            return;
        }

        PlayerLocalmotionmanager.HandleAllMovement();

        PlayerStatsManager.RegenerateStamina();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.Instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.Instance.player = this;
            PlayerInputManager.Instance.player = this;
            WorldSaveGameManager.Instance.player = this;

            //  UPDATES THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES
            PlayerNetworkManager.vitality.OnValueChanged += PlayerNetworkManager.SetNewMaxHealthValue;
            PlayerNetworkManager.endurance.OnValueChanged += PlayerNetworkManager.SetNewMaxStaminaValue;

            //  UPDATES UI STAT BAR WHEN A STAT CHANGES
            PlayerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.Instance.playerUIHudManager.SetNewHealthValue;
            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.playerUIHudManager.SetNewStaminaValue;
            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerStatsManager.ResetStaminaRegenerationTimer;
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.characterName = PlayerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;
        currentCharacterData.vitality = PlayerNetworkManager.vitality.Value;
        currentCharacterData.endurance = PlayerNetworkManager.endurance.Value;
        currentCharacterData.currentHealth = PlayerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = PlayerNetworkManager.currentStamina.Value;
    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        PlayerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        PlayerNetworkManager.vitality.Value = currentCharacterData.vitality;
        PlayerNetworkManager.endurance.Value = currentCharacterData.endurance;

        PlayerNetworkManager.maxHealth.Value = PlayerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);
        PlayerNetworkManager.maxStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
        PlayerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
        PlayerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
        PlayerUIManager.Instance.playerUIHudManager.SetMaxStaminaValue(PlayerNetworkManager.maxStamina.Value);
        PlayerUIManager.Instance.playerUIHudManager.SetMaxHealthValue(PlayerNetworkManager.maxHealth.Value);
    }
}
