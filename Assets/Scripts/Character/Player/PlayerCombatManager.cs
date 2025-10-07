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
        //  PERFORM THE ACTION
        weaponAction.AttempToPerformAction(player, weaponPerformingAction);

        //  NOTIFY THE SERVER WE HAVE PERFORMED THE ACTION, SO WE PERFORM IT FROM THERE PERSPECTIVE ALSO
    }
}
