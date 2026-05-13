using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Database", menuName = "RPG/Ability Database")]
public class AbilityDatabase : ScriptableObject
{
    [SerializeField] private AbilityDefinition[] _abilityDefinitions;

    private Dictionary<string, AbilityDefinition> _abilitiesById;

    public AbilityDefinition[] Abilities => _abilityDefinitions;

    public bool TryGetAbility(string abilityId, out AbilityDefinition ability)
    {
        EnsureLookup();
        return _abilitiesById.TryGetValue(abilityId, out ability);
    }

    public AbilityDefinition GetAbility(string abilityId)
    {
        TryGetAbility(abilityId, out AbilityDefinition ability);
        return ability;
    }

    private void EnsureLookup()
    {
        if (_abilitiesById != null) return;

        _abilitiesById = new Dictionary<string, AbilityDefinition>();
        if (_abilityDefinitions == null) return;

        for (int i = 0; i < _abilityDefinitions.Length; i++)
        {
            AbilityDefinition ability = _abilityDefinitions[i];
            if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) continue;
            _abilitiesById[ability._abilityId] = ability;
        }
    }
}
