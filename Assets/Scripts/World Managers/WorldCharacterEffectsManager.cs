using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager Instance {  get; private set; }

    [Header("Damage")]
    [SerializeField] public TakeDamageEffect takeDamageEffect;

    [SerializeField] private List<InstantCharacterEffect> instantEffects;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("WorldCharacterEffectsManager already has a Instance!");
        }

        Instance = this;

        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectID = i;
        }
    }
}
