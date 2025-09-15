using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    private CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealt = 0;

    [Header("Poise")]
    private float poiseDamage = 0;
    private bool poiseIsBroken = false;

    [Header("Animation")]
    private bool playDamageAnimation = true;
    private bool manuallySelectDamageAnimation = false;
    private string damageAnimation;

    [Header("Sound FX")]
    private bool willPlayDamageSFX = true;
    private AudioClip elementalDamageSoundFX;

    [Header("Direction of Damage Taken From")]
    private float angleHitFrom;
    public Vector3 contactPoint;

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        if (character.isDead.Value) return;

        CalculateDamage(character);
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (characterCausingDamage != null)
        {

        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealt <= 0) finalDamageDealt = 1;

        character.CharacterNetworkManager.currentHealth.Value -= finalDamageDealt;
    }
}
