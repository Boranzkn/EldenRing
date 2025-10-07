using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager PlayerAnimatorManager { private set; get; }
    [HideInInspector] public PlayerLocalmotionManager PlayerLocalmotionmanager { private set; get; }
    [HideInInspector] public PlayerNetworkManager PlayerNetworkManager { private set; get; }
    [HideInInspector] public PlayerStatsManager PlayerStatsManager { private set; get; }
    [HideInInspector] public PlayerInventoryManager PlayerInventoryManager { private set; get; }
    [HideInInspector] public PlayerEquipmentManager PlayerEquipmentManager { private set; get; }
    [HideInInspector] public PlayerCombatManager PlayerCombatManager { private set; get; }

    [Header("DEBUG MENU")]
    [SerializeField] private bool respawnCharacter = false;
    [SerializeField] private bool switchRightWeapon = false;

    protected override void Awake()
    {
        base.Awake();

        PlayerLocalmotionmanager = GetComponent<PlayerLocalmotionManager>();
        PlayerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        PlayerNetworkManager = GetComponent<PlayerNetworkManager>();
        PlayerStatsManager = GetComponent<PlayerStatsManager>();
        PlayerInventoryManager = GetComponent<PlayerInventoryManager>();
        PlayerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        PlayerCombatManager = GetComponent<PlayerCombatManager>();
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

        DebugMenu();
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
            PlayerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.Instance.PlayerUIHudManager.SetNewHealthValue;
            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.PlayerUIHudManager.SetNewStaminaValue;
            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerStatsManager.ResetStaminaRegenerationTimer;
        }

        //  CHECKS IF THE PLAYER DIED WHEN HEALTH CHANGES
        PlayerNetworkManager.currentHealth.OnValueChanged += PlayerNetworkManager.CheckHP;

        //  UPDATES THE EQUIPPED WEAPON WHEN THE WEAPON ID CHANGES
        PlayerNetworkManager.currentRightHandWeaponID.OnValueChanged += PlayerNetworkManager.OnCurrentRightHandWeaponIDChange;
        PlayerNetworkManager.currentLeftHandWeaponID.OnValueChanged += PlayerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        PlayerNetworkManager.currentWeaponBeingUsed.OnValueChanged += PlayerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        //  UPON CONNECTING, IF WE ARE THE OWNER OF THIS CHARACTER, BUT NOT THE SERVER, RELOAD OUR CHARACTER DATA TO THIS NEWLY INSTANTIATED CHARACTER
        if (IsOwner && !IsServer)
        {
            LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.Instance.currentCharacterData);
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            PlayerUIManager.Instance.PlayerUIPopUpManager.SendYouDiedPopUp();
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            PlayerNetworkManager.currentHealth.Value = PlayerNetworkManager.maxHealth.Value;
            PlayerNetworkManager.currentStamina.Value = PlayerNetworkManager.maxStamina.Value;

            PlayerAnimatorManager.PlayTargetActionAnimation("Empty", false);
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
        PlayerUIManager.Instance.PlayerUIHudManager.SetMaxStaminaValue(PlayerNetworkManager.maxStamina.Value);
        PlayerUIManager.Instance.PlayerUIHudManager.SetMaxHealthValue(PlayerNetworkManager.maxHealth.Value);
    }

    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            PlayerEquipmentManager.SwitchRightWeapon();
        }
    }
}
