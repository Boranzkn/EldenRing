using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    private PlayerManager player;

    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    public GameObject rightHamdWeaponModel;
    public GameObject leftHamdWeaponModel;

    [SerializeField] private WeaponManager rightWeaponManager;
    [SerializeField] private WeaponManager leftWeaponManager;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }


    //  RIGHT WEAPON

    public void SwitchRightWeapon()
    {
        if (!player.IsOwner) return;

        player.PlayerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, true, true, true);

        WeaponItem selectedWeapon = null;

        //  ADD ONE TO INDEX TO SWITCH TO NEXT WEAPON
        player.PlayerInventoryManager.rightHandWeaponIndex += 1;

        //  IF INDEX IS OUT OF BOUNDS, RESET IT TO 0
        if (player.PlayerInventoryManager.rightHandWeaponIndex < 0 || player.PlayerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.PlayerInventoryManager.rightHandWeaponIndex = 0;

            //  WE CHECK IF WE ARE HOLDING MORE THAN ONE WEAPON
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player.PlayerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player.PlayerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    weaponCount++;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.PlayerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }

            if (weaponCount <= 1)
            {
                player.PlayerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                player.PlayerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.PlayerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                player.PlayerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach (WeaponItem weapon in player.PlayerInventoryManager.weaponsInRightHandSlots)
        {
            //  IF THE NEXT POTENTIAL WEAPON DOES NOT EQUAL UNARMED
            if (player.PlayerInventoryManager.weaponsInRightHandSlots[player.PlayerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.PlayerInventoryManager.weaponsInRightHandSlots[player.PlayerInventoryManager.rightHandWeaponIndex];
                player.PlayerNetworkManager.currentRightHandWeaponID.Value = player.PlayerInventoryManager.weaponsInRightHandSlots[player.PlayerInventoryManager.rightHandWeaponIndex].itemID;
                return;
            }
        }

        if (selectedWeapon == null && player.PlayerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
    }

    public void LoadRightWeapon()
    {
        if (player.PlayerInventoryManager.currentRightHandWeapon != null)
        {
            //  IF WE HAVE A WEAPON IN HAND, UNLOAD IT
            rightHandSlot.UnloadWeapon();

            //  INSTANTIATE THE NEW WEAPON
            rightHamdWeaponModel = Instantiate(player.PlayerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHamdWeaponModel);
            rightWeaponManager = rightHamdWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.PlayerInventoryManager.currentRightHandWeapon);
        }
    }


    //  LEFT WEAPON

    public void SwitchLeftWeapon()
    {

    }

    public void LoadLeftWeapon()
    {
        if (player.PlayerInventoryManager.currentLeftHandWeapon != null)
        {
            //  IF WE HAVE A WEAPON IN HAND, UNLOAD IT
            leftHandSlot.UnloadWeapon();

            //  INSTANTIATE THE NEW WEAPON
            leftHamdWeaponModel = Instantiate(player.PlayerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHamdWeaponModel);
            leftWeaponManager = leftHamdWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.PlayerInventoryManager.currentLeftHandWeapon);
        }
    }
}
