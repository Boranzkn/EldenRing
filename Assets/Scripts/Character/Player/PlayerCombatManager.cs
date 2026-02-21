using Unity.Netcode;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;
    
    public WeaponItem currentWeaponBeingUsed;

    override protected void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if (player.IsOwner)
        {
            //  PERFORM THE ACTION
            weaponAction.AttempToPerformAction(player, weaponPerformingAction);

            //  NOTIFY THE SERVER WE HAVE PERFORMED THE ACTION, SO WE PERFORM IT FROM THERE PERSPECTIVE ALSO
            player.PlayerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner)
            return;

        if (currentWeaponBeingUsed == null)
            return;

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultipler;
                break;
            default:
                break;
        }

        player.PlayerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }
}
