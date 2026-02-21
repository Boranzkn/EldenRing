using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] private string light_Attack_01 = "Main_Light_Attack_01";

    public override void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner)
            return;

        //  CHECK FOR STOPS
        if (playerPerformingAction.PlayerNetworkManager.currentStamina.Value <= 0)
            return;

        if (!playerPerformingAction.isGrounded)
            return;

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformingAction.PlayerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.PlayerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
        }

        if (playerPerformingAction.PlayerNetworkManager.isUsingLeftHand.Value)
        {

        }
    }
}
