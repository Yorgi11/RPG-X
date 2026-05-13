using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityCategory
{
    Universal,
    Warrior,
    Mage,
    Paladin,
    Weapon,
    Defensive,
    Movement,
    Utility,
    Enemy,
    Boss
}

public enum AbilityResourceType
{
    None,
    Health,
    Stamina,
    Mana,
    ItemQuantity,
    Durability,
    CooldownOnly,
    Special
}

public enum AbilityPaymentTiming
{
    OnStart,
    OnActivation,
    OverTime,
    OnSuccessfulHit
}

public enum AbilityTargetingMode
{
    Self,
    MeleeForward,
    LockOnTarget,
    Projectile,
    Cone,
    AreaAroundSelf,
    GroundTarget,
    DirectionalMovement
}

public enum AbilityEffectType
{
    Damage,
    Heal,
    Shield,
    Buff,
    Debuff,
    Movement,
    ResourceRestore,
    Stagger,
    Knockback,
    Teleport,
    Cleanse,
    SpawnProjectile,
    SpawnArea,
    Interact
}

public enum AbilityDamageType
{
    Physical,
    Fire,
    Arcane,
    Holy,
    Poison,
    Bleed
}

public enum AbilityScalingStat
{
    None,
    Strength,
    Endurance,
    Intelligence,
    Faith,
    Agility
}

public enum AbilityInputSlot
{
    BasicAttack,
    HeavyAttack,
    Block,
    Parry,
    Dodge,
    AbilitySlot1,
    AbilitySlot2,
    AbilitySlot3,
    Consumable,
    Ultimate
}

public enum AbilityMovementLockMode
{
    None,
    RotationOnly,
    RootedDuringCast,
    RootedDuringHit,
    FullLock,
    MovementAllowedWhileCasting
}

public enum AbilityResourceSpendResult
{
    Success,
    NoOwner,
    NotEnoughHealth,
    NotEnoughStamina,
    NotEnoughMana,
    MissingItem,
    MissingDurability,
    Unsupported
}

public enum AbilityUseFailureReason
{
    None,
    MissingDefinition,
    MissingOwner,
    NotUnlocked,
    OnCooldown,
    NotEnoughResource,
    InvalidTarget,
    OutOfRange,
    Busy,
    Interrupted
}

[Serializable]
public class AbilityDefinition
{
    public string _abilityId;
    public string _displayName;
    [TextArea] public string _description;
    public AbilityCategory _category;
    public string _classAffinity;
    public string _requiredClass;
    public int _requiredLevel = 1;
    public string _requiredSkill;
    public string _requiredWeaponType;
    public string _requiredEquipmentTag;
    public AbilityResourceType _resourceCostType = AbilityResourceType.None;
    public float _resourceCostAmount;
    public AbilityPaymentTiming _paymentTiming = AbilityPaymentTiming.OnActivation;
    public float _cooldown;
    public float _castTime;
    public float _channelDuration;
    public float _recoveryTime;
    public float _range = 2f;
    public float _areaSize = 1f;
    public AbilityTargetingMode _targetingMode = AbilityTargetingMode.Self;
    public AbilityDamageType _damageType = AbilityDamageType.Physical;
    public float _basePower;
    public AbilityScalingStat _scalingStat = AbilityScalingStat.None;
    public float _scalingCoefficient;
    public AbilityMovementLockMode _movementLockMode = AbilityMovementLockMode.None;
    public bool _canBeInterruptedByDamage = true;
    public bool _canBeInterruptedByStagger = true;
    public bool _canBeDodgeCancelled;
    public bool _canBeBlockCancelled;
    public AnimationClip _animation;
    public GameObject _startVfx;
    public GameObject _castVfx;
    public GameObject _impactVfx;
    public AudioClip _startSfx;
    public AudioClip _castSfx;
    public AudioClip _impactSfx;
    public Sprite _icon;
    public AbilityInputSlot _inputSlot = AbilityInputSlot.AbilitySlot1;
    public AbilityAIUsageRule _aiUsage;
    public string[] _upgradeAbilityIds;
    public string[] _tags;
    public AbilityEffectDefinition[] _effects;
}

[Serializable]
public class AbilityEffectDefinition
{
    public string _effectId;
    public AbilityEffectType _effectType = AbilityEffectType.Damage;
    public AbilityTargetingMode _targetRules = AbilityTargetingMode.Self;
    public float _value;
    public float _duration;
    public float _tickRate;
    public AbilityDamageType _damageType = AbilityDamageType.Physical;
    public AbilityScalingStat _scalingStat = AbilityScalingStat.None;
    public float _scalingCoefficient;
    public bool _canCrit;
    public bool _canBeBlocked = true;
    public bool _canBeParried = true;
    public bool _canBeDodged = true;
    public bool _canBeResisted = true;
    public string _statusApplied;
    public BonusDefinition[] _bonuses;
    public float _radius = 1f;
    public float _angle = 60f;
    public float _force;
    public float _projectileSpeed = 20f;
    public int _projectilePenetration;
    public Mesh _projectileMesh;
    public Material _projectileMaterial;
    public LayerMask _targetMask = ~0;
    public GameObject _vfx;
    public AudioClip _sfx;
}

[Serializable]
public class AbilityAIUsageRule
{
    public float _minimumRange;
    public float _preferredRange = 2f;
    public float _maximumRange = 10f;
    public int _weight = 1;
    public string[] _useConditions;
    public string[] _comboFollowupAbilityIds;
}

[Serializable]
public class AbilityLoadoutEntry
{
    public AbilityInputSlot _slot;
    public AbilityDefinition _ability;
}

public sealed class AbilityRuntimeState
{
    public readonly string _abilityId;
    public float _currentCooldown;
    public bool _isCasting;
    public bool _isChanneling;
    public Transform _currentTarget;
    public float _currentChargeTime;
    public GameObject _activeHitboxInstance;
    public GameObject _activeProjectileInstance;
    public readonly List<ActiveBonus> _temporaryRuntimeModifiers = new();
    public float _lastUsedTime;

    public bool IsAvailable => _currentCooldown <= 0f && !_isCasting && !_isChanneling;

    public AbilityRuntimeState(string abilityId)
    {
        _abilityId = abilityId;
    }
}

public sealed class AbilityUseContext
{
    public Transform _target;
    public Vector3 _origin;
    public Vector3 _direction = Vector3.forward;
    public Vector3 _targetPoint;

    public static AbilityUseContext FromTransform(Transform casterTransform)
    {
        Vector3 position = casterTransform != null ? casterTransform.position : Vector3.zero;
        Vector3 direction = casterTransform != null ? casterTransform.forward : Vector3.forward;
        return new AbilityUseContext
        {
            _origin = position,
            _direction = direction,
            _targetPoint = position + direction
        };
    }
}

[RequireComponent(typeof(Character))]
public class CharacterAbilityController : MonoBehaviour
{
    [SerializeField] private AbilityDefinition[] _unlockedAbilities;
    [SerializeField] private AbilityLoadoutEntry[] _loadout;

    private readonly Dictionary<string, AbilityDefinition> _unlockedById = new();
    private readonly Dictionary<AbilityInputSlot, AbilityDefinition> _loadoutBySlot = new();
    private readonly Dictionary<string, AbilityRuntimeState> _statesByAbilityId = new();
    private Character _owner;
    private Coroutine _activeRoutine;

    public Character Owner => _owner;
    public IReadOnlyDictionary<string, AbilityRuntimeState> StatesByAbilityId => _statesByAbilityId;
    public event Action<AbilityDefinition> OnAbilityStarted;
    public event Action<AbilityDefinition> OnAbilityActivated;
    public event Action<AbilityDefinition> OnAbilityFinished;
    public event Action<AbilityDefinition, AbilityUseFailureReason> OnAbilityFailed;

    private void Awake()
    {
        _owner = GetComponent<Character>();
        LoadStartingAbilities();
        RebuildLookup();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (AbilityRuntimeState state in _statesByAbilityId.Values)
            if (state._currentCooldown > 0f)
                state._currentCooldown = Mathf.Max(0f, state._currentCooldown - dt);
    }

    public void LoadStartingAbilities()
    {
        if (_owner == null || _owner.Class == null) return;

        Class characterClass = _owner.Class;
        if (characterClass._startingAbilities != null && characterClass._startingAbilities.Length > 0)
            _unlockedAbilities = MergeAbilities(_unlockedAbilities, characterClass._startingAbilities);

        if ((_loadout == null || _loadout.Length == 0) && characterClass._defaultLoadout != null)
            _loadout = characterClass._defaultLoadout;
    }

    public bool HasUnlocked(string abilityId)
    {
        return !string.IsNullOrWhiteSpace(abilityId) && _unlockedById.ContainsKey(abilityId);
    }

    public AbilityRuntimeState GetState(string abilityId)
    {
        if (string.IsNullOrWhiteSpace(abilityId)) return null;
        return _statesByAbilityId.TryGetValue(abilityId, out AbilityRuntimeState state) ? state : null;
    }

    public bool TryGetEquippedAbility(AbilityInputSlot slot, out AbilityDefinition ability)
    {
        return _loadoutBySlot.TryGetValue(slot, out ability) && ability != null;
    }

    public bool TryUseAbility(AbilityInputSlot slot, AbilityUseContext context = null)
    {
        if (!_loadoutBySlot.TryGetValue(slot, out AbilityDefinition ability))
        {
            OnAbilityFailed?.Invoke(null, AbilityUseFailureReason.MissingDefinition);
            return false;
        }

        return TryUseAbility(ability, context);
    }

    public bool TryUseAbility(string abilityId, AbilityUseContext context = null)
    {
        if (!_unlockedById.TryGetValue(abilityId, out AbilityDefinition ability))
        {
            OnAbilityFailed?.Invoke(null, AbilityUseFailureReason.NotUnlocked);
            return false;
        }

        return TryUseAbility(ability, context);
    }

    public bool TryUseAbility(AbilityDefinition ability, AbilityUseContext context = null)
    {
        AbilityUseFailureReason failureReason = ValidateAbility(ability, context);
        if (failureReason != AbilityUseFailureReason.None)
        {
            OnAbilityFailed?.Invoke(ability, failureReason);
            return false;
        }

        AbilityRuntimeState state = GetOrCreateState(ability);
        _activeRoutine = StartCoroutine(ExecuteAbilityRoutine(ability, state, context ?? AbilityUseContext.FromTransform(transform)));
        return true;
    }

    public void UnlockAbility(AbilityDefinition ability)
    {
        if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) return;

        List<AbilityDefinition> abilities = _unlockedAbilities != null ?
            new List<AbilityDefinition>(_unlockedAbilities) :
            new List<AbilityDefinition>();

        for (int i = 0; i < abilities.Count; i++)
            if (abilities[i] != null && abilities[i]._abilityId == ability._abilityId)
                return;

        abilities.Add(ability);
        _unlockedAbilities = abilities.ToArray();
        RebuildLookup();
    }

    public void EquipAbility(AbilityInputSlot slot, AbilityDefinition ability)
    {
        if (ability == null) return;
        UnlockAbility(ability);

        List<AbilityLoadoutEntry> loadout = _loadout != null ?
            new List<AbilityLoadoutEntry>(_loadout) :
            new List<AbilityLoadoutEntry>();

        for (int i = 0; i < loadout.Count; i++)
        {
            if (loadout[i]._slot != slot) continue;
            loadout[i]._ability = ability;
            _loadout = loadout.ToArray();
            RebuildLookup();
            return;
        }

        loadout.Add(new AbilityLoadoutEntry { _slot = slot, _ability = ability });
        _loadout = loadout.ToArray();
        RebuildLookup();
    }

    private AbilityUseFailureReason ValidateAbility(AbilityDefinition ability, AbilityUseContext context)
    {
        if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) return AbilityUseFailureReason.MissingDefinition;
        if (_owner == null) return AbilityUseFailureReason.MissingOwner;
        if (!HasUnlocked(ability._abilityId)) return AbilityUseFailureReason.NotUnlocked;
        if (_activeRoutine != null) return AbilityUseFailureReason.Busy;

        AbilityRuntimeState state = GetOrCreateState(ability);
        if (state._currentCooldown > 0f) return AbilityUseFailureReason.OnCooldown;
        if (!HasResources(ability)) return AbilityUseFailureReason.NotEnoughResource;
        if (!HasValidTarget(ability, context)) return AbilityUseFailureReason.InvalidTarget;
        if (!IsTargetInRange(ability, context)) return AbilityUseFailureReason.OutOfRange;

        return AbilityUseFailureReason.None;
    }

    private IEnumerator ExecuteAbilityRoutine(AbilityDefinition ability, AbilityRuntimeState state, AbilityUseContext context)
    {
        state._isCasting = ability._castTime > 0f;
        state._currentTarget = context._target;
        state._lastUsedTime = Time.time;
        OnAbilityStarted?.Invoke(ability);

        if (ability._paymentTiming == AbilityPaymentTiming.OnStart && !SpendResources(ability))
        {
            state._isCasting = false;
            _activeRoutine = null;
            OnAbilityFailed?.Invoke(ability, AbilityUseFailureReason.NotEnoughResource);
            yield break;
        }

        if (ability._castTime > 0f)
            yield return new WaitForSeconds(ability._castTime);

        state._isCasting = false;

        if (ability._paymentTiming == AbilityPaymentTiming.OnActivation && !SpendResources(ability))
        {
            _activeRoutine = null;
            OnAbilityFailed?.Invoke(ability, AbilityUseFailureReason.NotEnoughResource);
            yield break;
        }

        OnAbilityActivated?.Invoke(ability);
        ResolveEffects(ability, context);

        if (ability._channelDuration > 0f)
        {
            state._isChanneling = true;
            float elapsed = 0f;
            while (elapsed < ability._channelDuration)
            {
                elapsed += Time.deltaTime;
                if (ability._paymentTiming == AbilityPaymentTiming.OverTime)
                {
                    float tickCost = ability._resourceCostAmount * Time.deltaTime / Mathf.Max(ability._channelDuration, 0.001f);
                    if (!SpendResource(ability._resourceCostType, tickCost))
                        break;
                }

                yield return null;
            }
            state._isChanneling = false;
        }

        state._currentCooldown = Mathf.Max(0f, ability._cooldown * (1f - _owner.CurrentBonuses.GetModifierValue(BonusStat.AbilityCooldownReduction)));

        if (ability._recoveryTime > 0f)
            yield return new WaitForSeconds(ability._recoveryTime);

        _activeRoutine = null;
        OnAbilityFinished?.Invoke(ability);
    }

    private void ResolveEffects(AbilityDefinition ability, AbilityUseContext context)
    {
        if (ability._effects == null) return;

        for (int i = 0; i < ability._effects.Length; i++)
            ResolveEffect(ability, ability._effects[i], context);
    }

    private void ResolveEffect(AbilityDefinition ability, AbilityEffectDefinition effect, AbilityUseContext context)
    {
        if (effect == null) return;

        switch (effect._effectType)
        {
            case AbilityEffectType.Damage:
            case AbilityEffectType.Stagger:
                ApplyDamageEffect(ability, effect, context);
                break;
            case AbilityEffectType.Heal:
                _owner.ApplyHealing(CalculateEffectValue(effect));
                break;
            case AbilityEffectType.Buff:
            case AbilityEffectType.Shield:
            case AbilityEffectType.Debuff:
                ApplyBonusEffect(effect);
                break;
            case AbilityEffectType.ResourceRestore:
                RestoreResource(ability._resourceCostType, CalculateEffectValue(effect));
                break;
            case AbilityEffectType.Movement:
                ApplyMovementEffect(effect, context);
                break;
            case AbilityEffectType.Knockback:
                ApplyKnockbackEffect(effect, context);
                break;
            case AbilityEffectType.Teleport:
                transform.position = context._targetPoint;
                break;
            case AbilityEffectType.SpawnProjectile:
                SpawnProjectileEffect(effect, context);
                break;
        }
    }

    private void ApplyDamageEffect(AbilityDefinition ability, AbilityEffectDefinition effect, AbilityUseContext context)
    {
        float damage = CalculateEffectValue(effect);
        if (Mathf.Approximately(damage, 0f))
            damage = CalculateAbilityPower(ability);

        if (context._target != null)
        {
            IProjectileDamageable damageable = context._target.GetComponentInParent<IProjectileDamageable>();
            damageable?.TakeProjectileDamage(new ProjectileDamageInfo(
                damage,
                ToProjectileDamageType(effect._damageType),
                gameObject,
                context._target.position,
                -context._direction.normalized));
            return;
        }

        Collider[] hits = Physics.OverlapSphere(context._origin + context._direction.normalized * ability._range, Mathf.Max(0.1f, effect._radius), effect._targetMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            IProjectileDamageable damageable = hits[i].GetComponentInParent<IProjectileDamageable>();
            damageable?.TakeProjectileDamage(new ProjectileDamageInfo(
                damage,
                ToProjectileDamageType(effect._damageType),
                gameObject,
                hits[i].ClosestPoint(context._origin),
                -context._direction.normalized));
        }
    }

    private void ApplyBonusEffect(AbilityEffectDefinition effect)
    {
        if (effect._bonuses == null) return;

        for (int i = 0; i < effect._bonuses.Length; i++)
            _owner.AddBonus(effect._bonuses[i], effect._effectId, this, effect._duration);
    }

    private void ApplyMovementEffect(AbilityEffectDefinition effect, AbilityUseContext context)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = context._direction.sqrMagnitude > 0.0001f ? context._direction.normalized : transform.forward;
        if (rb != null)
            rb.AddForce(direction * effect._force, ForceMode.VelocityChange);
        else
            transform.position += direction * effect._force;
    }

    private void ApplyKnockbackEffect(AbilityEffectDefinition effect, AbilityUseContext context)
    {
        if (context._target == null) return;
        Rigidbody rb = context._target.GetComponentInParent<Rigidbody>();
        if (rb == null) return;

        Vector3 direction = context._direction.sqrMagnitude > 0.0001f ? context._direction.normalized : transform.forward;
        rb.AddForce(direction * effect._force, ForceMode.VelocityChange);
    }

    private void SpawnProjectileEffect(AbilityEffectDefinition effect, AbilityUseContext context)
    {
        if (LocalProjectilePool.Instance == null) return;

        Vector3 direction = context._direction.sqrMagnitude > 0.0001f ? context._direction.normalized : transform.forward;
        float damage = CalculateEffectValue(effect);
        LocalProjectilePool.Instance.SpawnProjectile(
            context._origin,
            direction * Mathf.Max(0.01f, effect._projectileSpeed),
            damage,
            effect._projectilePenetration,
            ToProjectileDamageType(effect._damageType),
            effect._projectileMesh,
            effect._projectileMaterial,
            effect._targetMask,
            false,
            gameObject);
    }

    private float CalculateAbilityPower(AbilityDefinition ability)
    {
        float value = ability._basePower + GetScalingStatValue(ability._scalingStat) * ability._scalingCoefficient;
        return ApplyDamageBonuses(value, ability._damageType);
    }

    private float CalculateEffectValue(AbilityEffectDefinition effect)
    {
        float value = effect._value + GetScalingStatValue(effect._scalingStat) * effect._scalingCoefficient;
        if (effect._effectType == AbilityEffectType.Damage || effect._effectType == AbilityEffectType.SpawnProjectile)
            value = ApplyDamageBonuses(value, effect._damageType);
        else if (effect._effectType == AbilityEffectType.Heal)
            value *= 1f + _owner.CurrentBonuses.GetModifierValue(BonusStat.HealingEffectiveness);

        return Mathf.Max(0f, value);
    }

    private float ApplyDamageBonuses(float value, AbilityDamageType damageType)
    {
        switch (damageType)
        {
            case AbilityDamageType.Physical:
                value *= 1f + _owner.CurrentBonuses.GetModifierValue(BonusStat.MeleeDamage);
                break;
            case AbilityDamageType.Arcane:
            case AbilityDamageType.Fire:
                value *= 1f + _owner.CurrentBonuses.GetModifierValue(BonusStat.SpellDamage);
                break;
            case AbilityDamageType.Holy:
                value *= 1f + _owner.CurrentBonuses.GetModifierValue(BonusStat.HolyDamage);
                break;
        }

        return value;
    }

    private float GetScalingStatValue(AbilityScalingStat stat)
    {
        if (_owner.CurrentStats == null) return 0f;

        return stat switch
        {
            AbilityScalingStat.Strength => _owner.CurrentStats._strength,
            AbilityScalingStat.Endurance => _owner.CurrentStats._endurance,
            AbilityScalingStat.Intelligence => _owner.CurrentStats._intelligence,
            AbilityScalingStat.Faith => _owner.CurrentStats._faith,
            AbilityScalingStat.Agility => _owner.CurrentStats._agility,
            _ => 0f
        };
    }

    private bool HasResources(AbilityDefinition ability)
    {
        return ability._resourceCostType switch
        {
            AbilityResourceType.None => true,
            AbilityResourceType.CooldownOnly => true,
            AbilityResourceType.Health => _owner.HasHealth(ability._resourceCostAmount),
            AbilityResourceType.Stamina => _owner.HasStamina(GetAdjustedResourceCost(ability)),
            AbilityResourceType.Mana => _owner.HasMana(GetAdjustedResourceCost(ability)),
            _ => false
        };
    }

    private bool SpendResources(AbilityDefinition ability)
    {
        return SpendResource(ability._resourceCostType, GetAdjustedResourceCost(ability));
    }

    private bool SpendResource(AbilityResourceType resourceType, float amount)
    {
        return resourceType switch
        {
            AbilityResourceType.None => true,
            AbilityResourceType.CooldownOnly => true,
            AbilityResourceType.Health => _owner.TrySpendHealth(amount),
            AbilityResourceType.Stamina => _owner.TrySpendStamina(amount),
            AbilityResourceType.Mana => _owner.TrySpendMana(amount),
            _ => false
        };
    }

    private void RestoreResource(AbilityResourceType resourceType, float amount)
    {
        switch (resourceType)
        {
            case AbilityResourceType.Health:
                _owner.ApplyHealing(amount);
                break;
            case AbilityResourceType.Stamina:
                _owner.RestoreStamina(amount);
                break;
            case AbilityResourceType.Mana:
                _owner.RestoreMana(amount);
                break;
        }
    }

    private float GetAdjustedResourceCost(AbilityDefinition ability)
    {
        return ability._resourceCostType switch
        {
            AbilityResourceType.Stamina => Mathf.Max(0f, _owner.ApplyReduction(BonusStat.AttackStaminaCostReduction, ability._resourceCostAmount)),
            AbilityResourceType.Mana => _owner.GetManaCost(ability._resourceCostAmount),
            _ => Mathf.Max(0f, ability._resourceCostAmount)
        };
    }

    private bool HasValidTarget(AbilityDefinition ability, AbilityUseContext context)
    {
        return ability._targetingMode switch
        {
            AbilityTargetingMode.LockOnTarget => context != null && context._target != null,
            _ => true
        };
    }

    private bool IsTargetInRange(AbilityDefinition ability, AbilityUseContext context)
    {
        if (context == null || context._target == null) return true;
        return Vector3.Distance(transform.position, context._target.position) <= Mathf.Max(0f, ability._range);
    }

    private AbilityRuntimeState GetOrCreateState(AbilityDefinition ability)
    {
        if (!_statesByAbilityId.TryGetValue(ability._abilityId, out AbilityRuntimeState state))
        {
            state = new AbilityRuntimeState(ability._abilityId);
            _statesByAbilityId[ability._abilityId] = state;
        }

        return state;
    }

    private void RebuildLookup()
    {
        _unlockedById.Clear();
        _loadoutBySlot.Clear();

        if (_unlockedAbilities != null)
        {
            for (int i = 0; i < _unlockedAbilities.Length; i++)
            {
                AbilityDefinition ability = _unlockedAbilities[i];
                if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) continue;
                _unlockedById[ability._abilityId] = ability;
                GetOrCreateState(ability);
            }
        }

        if (_loadout == null) return;
        for (int i = 0; i < _loadout.Length; i++)
        {
            AbilityDefinition ability = _loadout[i]._ability;
            if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) continue;
            if (_unlockedById.TryGetValue(ability._abilityId, out AbilityDefinition unlockedAbility))
                ability = unlockedAbility;
            _loadoutBySlot[_loadout[i]._slot] = ability;
            if (!_unlockedById.ContainsKey(ability._abilityId))
                _unlockedById[ability._abilityId] = ability;
            GetOrCreateState(ability);
        }
    }

    private static AbilityDefinition[] MergeAbilities(AbilityDefinition[] existing, AbilityDefinition[] incoming)
    {
        List<AbilityDefinition> merged = existing != null ?
            new List<AbilityDefinition>(existing) :
            new List<AbilityDefinition>();

        for (int i = 0; i < incoming.Length; i++)
        {
            AbilityDefinition ability = incoming[i];
            if (ability == null || string.IsNullOrWhiteSpace(ability._abilityId)) continue;

            bool alreadyExists = false;
            for (int m = 0; m < merged.Count; m++)
            {
                if (merged[m] == null || merged[m]._abilityId != ability._abilityId) continue;
                alreadyExists = true;
                break;
            }

            if (!alreadyExists) merged.Add(ability);
        }

        return merged.ToArray();
    }

    private static ProjectileDamageType ToProjectileDamageType(AbilityDamageType damageType)
    {
        return damageType == AbilityDamageType.Physical ? ProjectileDamageType.Physical : ProjectileDamageType.Magic;
    }
}
