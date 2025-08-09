using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug Delete Later")]
    [SerializeField] private InstantCharacterEffect effectForTesting;
    [SerializeField] private bool processEffect = false;

    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            InstantCharacterEffect effect = Instantiate(effectForTesting);
            ProcessInstantEffect(effect);
        }
    }
}
