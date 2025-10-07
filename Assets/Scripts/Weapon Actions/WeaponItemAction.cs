using UnityEngine;

[CreateAssetMenu(menuName ="Character Actions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformingAction.IsOwner)
        {
            playerPerformingAction.PlayerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;
        }

        Debug.Log("THE ACTION HAS FIRED");
    }
}
