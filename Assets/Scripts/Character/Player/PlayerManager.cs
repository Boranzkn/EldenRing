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

            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            PlayerNetworkManager.currentStamina.OnValueChanged += PlayerStatsManager.ResetStaminaRegenerationTimer;

            PlayerNetworkManager.maxStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(PlayerNetworkManager.endurance.Value);
            PlayerNetworkManager.currentStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(PlayerNetworkManager.endurance.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(PlayerNetworkManager.maxStamina.Value);
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.characterName = PlayerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;
    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        PlayerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;
    }
}
