# RPG Ability System Technical Report

## 1. Executive Summary

This report defines the **Ability System** for a third-person action/open-world RPG built around custom characters, class identity, backstory identity, real-time combat, skill progression, equipment, enemies, quests, and region-based expansion.

In this project, an **ability** is any intentional action a character can use in gameplay. This includes basic attacks, class skills, spells, defensive actions, movement abilities, utility actions, enemy attacks, and boss mechanics.

The ability system should support:

- Warrior, Mage, and Paladin class abilities
- Soldier, Student, and Peasant backstory interactions
- Stamina and mana costs
- Cooldowns, cast times, channeling, and recovery windows
- Animation, VFX, SFX, and UI integration
- Enemy and boss ability usage
- Equipment and skill tree modifiers
- Ability upgrades
- Save/load persistence
- Future region-based expansion

The system should be **data-driven** wherever possible. A new ability should usually be created by defining data, assigning animation/VFX/SFX references, and configuring its effects rather than writing an entirely new script.

---

## 2. Definition of an Ability

An ability is a gameplay action that performs one or more effects.

Examples:

```text
Light Attack
Heavy Attack
Dodge
Block
Parry
Power Strike
Arc Bolt
Minor Heal
Battle Rush
Mana Shield
Radiant Guard
Shield Bash
Wolf Pounce
Bandit Kick
Captain Varric's Shield Charge
```

Abilities can be used by:

- The player
- Enemies
- Bosses
- NPC allies, if companions are added later
- Scripted quest NPCs
- Traps or world objects, if needed

---

## 3. Core Ability System Goals

## 3.1 Support Class Identity

Each class should feel different because its abilities encourage different combat behavior.

```text
Warrior:
Physical, stamina-based, melee-focused, stagger-heavy, strong blocking, aggressive.

Mage:
Mana-based, ranged, projectile-focused, area damage, shields, utility, fragile.

Paladin:
Hybrid, defensive, healing-focused, holy damage, shields, sustain.
```

## 3.2 Support Build Progression

Abilities should improve through:

- Skill trees
- Leveling
- Class trainers
- Equipment modifiers
- Ability-specific upgrades
- Quest rewards
- Faction rewards
- Rare items
- Region progression

## 3.3 Support Readable Combat

Every ability should communicate:

- Startup
- Activation
- Hit/miss result
- Recovery
- Resource cost
- Cooldown
- Counterplay

Abilities should not feel random or unfair.

## 3.4 Avoid Hardcoding

The system should avoid making every ability a unique hardcoded script.

Most abilities should be composed from reusable parts:

```text
Targeting Mode
Resource Cost
Cooldown
Animation Timing
Effect List
Damage/Healing/Status Logic
VFX/SFX
Upgrade Modifiers
```

---

# 4. Ability Categories

## 4.1 Universal Combat Actions

Universal combat actions are available to most or all player characters.

| Ability | Purpose |
|---|---|
| Light Attack | Fast basic damage |
| Heavy Attack | Slower high-damage attack |
| Block | Reduce incoming damage |
| Parry | Timed defensive counter |
| Dodge | Avoid attacks through movement |
| Sprint Attack | Attack while sprinting |
| Use Consumable | Use potion, food, or item |
| Lock-On | Focus camera/combat on a target |

These actions may not appear in an ability bar, but technically they should still use the ability framework when possible.

---

## 4.2 Warrior Abilities

Warrior abilities should use stamina and focus on physical combat, melee pressure, armor, blocking, and stagger.

Example Warrior abilities:

| Ability | Purpose |
|---|---|
| Power Strike | Heavy melee attack with bonus stagger |
| Iron Guard | Temporary defensive stance |
| Battle Rush | Burst movement toward an enemy |
| Shield Bash | Close-range shield impact |
| Cleave | Wide melee swing |
| War Cry | Short-range intimidation/stagger effect |
| Ground Breaker | Heavy ground impact |
| Counter Blow | Follow-up after successful block/parry |

### Warrior Design Identity

Warrior abilities should be strongest in direct physical fights. They should reward stamina management, timing, and aggressive positioning.

---

## 4.3 Mage Abilities

Mage abilities should use mana and focus on ranged damage, utility, shields, and area control.

Example Mage abilities:

| Ability | Purpose |
|---|---|
| Arc Bolt | Basic ranged magic projectile |
| Flame Wave | Short-range cone spell |
| Mana Shield | Mana-based damage absorption |
| Frost Snare | Slows enemy movement |
| Chain Spark | Hits multiple nearby enemies |
| Arcane Burst | Area burst around caster |
| Teleport Step | Short magical reposition |
| Stone Spike | Ground-targeted projectile/spike |

### Mage Design Identity

Mage abilities should be powerful at range but risky when surrounded. Mages should rely on positioning, mana management, and spacing.

---

## 4.4 Paladin Abilities

Paladin abilities should combine melee, defense, healing, and holy damage.

Example Paladin abilities:

| Ability | Purpose |
|---|---|
| Holy Strike | Melee attack with holy damage |
| Minor Heal | Self-healing spell |
| Radiant Guard | Temporary damage reduction |
| Sanctified Shield | Shield buff |
| Judgment Bolt | Ranged holy attack |
| Blessed Ground | Ground area that heals/protects |
| Purifying Light | Cleanse or anti-corruption effect |
| Oathbreaker Smite | High-damage holy melee strike |

### Paladin Design Identity

Paladins should be strongest in long fights, dangerous regions, and defensive encounters. Their raw damage should usually be lower than Warrior, and their spell variety should be lower than Mage.

---

## 4.5 Weapon Abilities

Weapon abilities depend on equipped weapon type.

| Weapon Type | Ability Examples |
|---|---|
| Sword | Quick Slash, Riposte, Precision Thrust |
| Axe | Cleave, Armor Splitter, Execution Chop |
| Mace | Crushing Blow, Guard Breaker, Hammerfall |
| Spear | Lunge, Brace, Sweeping Thrust |
| Dagger | Backstab, Quickstep Slash, Bleed Cut |
| Staff | Arcane Focus, Staff Sweep, Mana Pulse |
| Wand | Quick Cast, Charged Bolt, Spell Pierce |
| Shield | Shield Bash, Bulwark, Shield Charge |

Weapon abilities make equipment choice more meaningful.

---

## 4.6 Defensive Abilities

Defensive abilities reduce damage, prevent hits, create counterattack opportunities, or improve survivability.

Examples:

```text
Block
Parry
Iron Guard
Radiant Guard
Mana Shield
Bulwark
Counter Stance
Holy Barrier
Emergency Roll
Shield Wall
```

Defensive abilities should interact with:

- Stamina
- Mana
- Armor
- Shields
- Poise/stagger
- Damage reduction
- Parry timing
- Block stability

---

## 4.7 Movement Abilities

Movement abilities reposition the character.

Examples:

```text
Dodge
Sidestep
Battle Rush
Shield Charge
Teleport Step
Leap Strike
Retreating Step
Mounted Dash
```

Movement abilities must be carefully balanced because they can break enemy spacing, traversal, and encounter design if they are too frequent or too long-ranged.

---

## 4.8 Utility Abilities

Utility abilities provide non-damaging or indirect benefits.

Examples:

```text
Detect Hidden Loot
Reveal Weakness
Field Repair
Quick Gather
Camp Recovery
Light Source
Open Simple Seal
Disarm Trap
Calm Animal
Intimidating Presence
```

Utility abilities are useful for backstories, exploration skills, and region-specific progression.

---

## 4.9 Enemy Abilities

Enemies should use the same underlying framework where possible.

Examples:

```text
Bandit Slash
Bandit Kick
Bandit Heavy Swing
Shieldman Guard
Deserter Arrow Shot
Wolf Pounce
Shrine Husk Burst
```

Enemy abilities need:

- Telegraphs
- Cooldowns
- AI selection rules
- Range checks
- Interrupt rules
- Counterplay

---

## 4.10 Boss Abilities

Boss abilities are stronger, more readable, and often phase-based.

Examples:

```text
Shield Charge
Heavy Overhead Strike
Rally Reinforcements
Ground Slam
Desperation Combo
Commanding Shout
Area Denial Burst
```

Boss abilities should feel like mechanics, not random damage.

---

# 5. Ability Data Model

## 5.1 Ability Definition Data

Each ability should have a static data definition.

Recommended fields:

```text
Ability ID
Display Name
Description
Ability Category
Class Affinity
Required Class
Required Level
Required Skill
Required Weapon Type
Required Equipment Tag
Resource Cost Type
Resource Cost Amount
Cooldown
Cast Time
Channel Duration
Recovery Time
Range
Area Size
Targeting Mode
Damage Type
Base Damage
Scaling Stat
Scaling Coefficient
Status Effects
Animation Reference
VFX Reference
SFX Reference
Icon Reference
Input Slot
AI Usage Rules
Upgrade Links
Tags
```

---

## 5.2 Runtime Ability State

Runtime ability state tracks the current gameplay state of an ability.

Recommended fields:

```text
Ability ID
Owner Character ID
Current Cooldown
Is Available
Is Casting
Is Channeling
Current Target
Current Charge Time
Active Hitbox Instance
Active Projectile Instance
Temporary Runtime Modifiers
Last Used Time
```

---

## 5.3 Ability Effect Data

An ability can apply one or more effects.

Common effect types:

```text
Damage Effect
Heal Effect
Shield Effect
Buff Effect
Debuff Effect
Movement Effect
Resource Restore Effect
Stagger Effect
Knockback Effect
Teleport Effect
Cleanse Effect
Spawn Projectile Effect
Spawn Area Effect
Interact Effect
```

Each effect should define:

```text
Effect ID
Effect Type
Target Rules
Value
Duration
Tick Rate
Damage Type
Scaling Stat
Scaling Coefficient
Can Crit
Can Be Blocked
Can Be Parried
Can Be Dodged
Can Be Resisted
Status Applied
VFX
SFX
```

---

# 6. Ability Execution Pipeline

A standard ability should follow this flow:

```text
1. Input or AI request
2. Validate ability
3. Check cooldown
4. Check resource cost
5. Check equipment/weapon requirements
6. Check character state
7. Begin cast or wind-up
8. Trigger animation
9. Trigger startup VFX/SFX
10. Execute effect at activation frame
11. Apply damage/healing/buff/movement/status
12. Spend resource
13. Start cooldown
14. Enter recovery
15. Return to normal state
```

---

## 6.1 Validation Checks

Before an ability can activate, the system should check:

```text
Is the owner alive?
Is the owner stunned, staggered, or unable to act?
Is the ability unlocked?
Is the ability on cooldown?
Does the owner have enough stamina/mana?
Is the required weapon equipped?
Is the target valid?
Is the target in range?
Is the owner in the correct state?
Is the ability blocked by current animation?
```

---

## 6.2 Resource Payment Timing

Abilities may spend resources at different points.

Recommended payment modes:

| Payment Mode | Use Case |
|---|---|
| On Start | Dodge, Battle Rush, Iron Guard |
| On Activation Frame | Arc Bolt, Power Strike |
| Over Time | Mana Shield, channeled spells |
| On Successful Hit | Certain advanced melee skills |

Examples:

```text
Power Strike:
Stamina paid when the attack begins or when the hit frame starts.

Arc Bolt:
Mana paid when projectile is released.

Mana Shield:
Mana paid initially, then drained when absorbing damage.

Dodge:
Stamina paid immediately.
```

---

# 7. Targeting Modes

Abilities should support multiple targeting modes.

| Targeting Mode | Description | Examples |
|---|---|---|
| Self | Applies to the caster | Minor Heal, Mana Shield |
| Melee Forward | Hits in front of caster | Power Strike, Shield Bash |
| Lock-On Target | Uses current lock-on target | Judgment Bolt, Battle Rush |
| Projectile | Spawns projectile | Arc Bolt, Arrow Shot |
| Cone | Area cone in front | Flame Wave, War Cry |
| Area Around Self | Radius around caster | Arcane Burst, Radiant Burst |
| Ground Target | Targets location on ground | Blessed Ground, Fire Rune |
| Directional Movement | Moves caster in direction | Dodge, Battle Rush |

---

# 8. Resource Costs

## 8.1 Supported Resource Types

The ability system should support:

```text
Health
Stamina
Mana
Item Quantity
Durability
Cooldown Only
No Cost
Special Resource, if added later
```

## 8.2 Class Resource Identity

```text
Warrior:
Primarily stamina-based.

Mage:
Primarily mana-based.

Paladin:
Hybrid stamina/mana-based.
```

## 8.3 Suggested Starting Costs

| Ability | Resource | Cost |
|---|---:|---:|
| Light Attack | Stamina | 8 |
| Heavy Attack | Stamina | 18 |
| Dodge | Stamina | 22 |
| Parry | Stamina | 12 |
| Power Strike | Stamina | 25 |
| Iron Guard | Stamina | 20 |
| Battle Rush | Stamina | 30 |
| Arc Bolt | Mana | 12 |
| Flame Wave | Mana | 28 |
| Mana Shield | Mana | 20 start + drain |
| Holy Strike | Stamina + Mana | 12 + 8 |
| Minor Heal | Mana | 18 |
| Radiant Guard | Mana | 22 |

---

# 9. Cooldowns, Cast Time, Channeling, and Recovery

## 9.1 Cooldown

Cooldown prevents ability spam.

Suggested examples:

```text
Arc Bolt: 1.2s
Power Strike: 4s
Minor Heal: 8s
Radiant Guard: 14s
Battle Rush: 10s
```

## 9.2 Cast Time

Cast time is the delay before an ability activates.

Examples:

```text
Power Strike: 0s cast, animation wind-up handles timing
Arc Bolt: 0.25s cast time
Minor Heal: 0.5s cast time
Heavy spell: 1.0s+ cast time
```

## 9.3 Channeling

Channeling means the ability remains active while the character continues using it.

Examples:

```text
Mana Shield
Channeled Beam
Prayer Heal
Flame Stream
```

## 9.4 Recovery Time

Recovery time is the delay after an ability before full control returns.

Examples:

```text
Power Strike: 0.6s recovery
Arc Bolt: 0.25s recovery
Heavy Attack: 0.5s recovery
Dodge: animation-based recovery
```

## 9.5 Interrupt Rules

Abilities should define whether they can be interrupted.

Recommended flags:

```text
Can Be Interrupted By Damage
Can Be Interrupted By Stagger
Can Be Interrupted By Dodge Cancel
Can Be Interrupted By Block Cancel
Can Be Interrupted By Movement
Can Be Interrupted By Death
```

---

# 10. Ability Scaling

## 10.1 Recommended Scaling Stats

| Ability Type | Scaling Stat |
|---|---|
| Physical melee | Strength |
| Blocking/guard | Endurance |
| Fast movement | Agility |
| Arcane spells | Intelligence |
| Healing/holy abilities | Faith |
| Hybrid holy melee | Strength + Faith |
| Hybrid battle magic | Strength + Intelligence |

## 10.2 Damage Formula

Recommended starting formula:

```text
Final Ability Damage =
(Base Damage + Scaling Stat × Scaling Coefficient)
× Damage Bonus Multipliers
× Target Resistance Modifier
```

Example:

```text
Power Strike:
Base Damage = 35
Strength = 13
Scaling Coefficient = 1.2

Raw Damage = 35 + (13 × 1.2)
Raw Damage = 50.6
```

## 10.3 Healing Formula

Recommended starting formula:

```text
Final Heal =
(Base Heal + Faith × Scaling Coefficient)
× Healing Effectiveness
```

Example:

```text
Minor Heal:
Base Heal = 30
Faith = 13
Scaling Coefficient = 1.1

Raw Heal = 30 + (13 × 1.1)
Raw Heal = 44.3
```

---

# 11. Ability Requirements

## 11.1 Unlock Requirements

Abilities may require:

```text
Player Level
Class
Backstory
Skill Tree Node
Trainer
Quest Completion
Faction Reputation
Region Unlock
Item Possession
Weapon Type
Stat Requirement
```

## 11.2 Use Requirements

Abilities may require:

```text
Correct Weapon Equipped
Shield Equipped
Enough Mana
Enough Stamina
Target In Range
Target Visible
Not Silenced
Not Staggered
Not Over-Encumbered
Not In Dialogue/Menu
```

## 11.3 Class Locking Rule

Classes should define starting access and efficiency, not total permanent restriction.

Example:

```text
Warrior can learn basic magic later, but Mage learns it earlier and cheaper.
Mage can use melee abilities, but Warrior gets stronger melee scaling.
Paladin gets earlier access to healing and holy abilities.
```

---

# 12. Ability Slots and Loadouts

## 12.1 Recommended Ability Slots

The player should have:

```text
Basic Attack
Heavy Attack
Block
Parry
Dodge
Ability Slot 1
Ability Slot 2
Ability Slot 3
Consumable Slot
Optional Ultimate/Special Slot later
```

## 12.2 MVP Loadouts

### Warrior

```text
Slot 1: Power Strike
Slot 2: Iron Guard
Slot 3: Battle Rush
```

### Mage

```text
Slot 1: Arc Bolt
Slot 2: Flame Wave
Slot 3: Mana Shield
```

### Paladin

```text
Slot 1: Holy Strike
Slot 2: Minor Heal
Slot 3: Radiant Guard
```

---

# 13. Ability Upgrade System

## 13.1 Upgrade Methods

Abilities can improve through:

```text
Skill tree upgrades
Ability ranks
Class trainers
Equipment modifiers
Quest rewards
Faction rewards
Region-specific unlocks
```

## 13.2 Upgrade Types

Possible upgrades:

```text
Increase damage
Reduce cost
Reduce cooldown
Increase range
Increase area size
Add status effect
Improve scaling
Add secondary effect
Increase duration
Improve stagger
Improve shield strength
Improve healing
Change targeting behavior
```

## 13.3 Example Upgrade Paths

### Power Strike

| Rank | Upgrade |
|---|---|
| 1 | Heavy physical damage and light stagger |
| 2 | +15% damage |
| 3 | Stronger stagger against weak enemies |
| 4 | Successful hit refunds 8 stamina |
| 5 | Can guard-break shielded enemies |

### Arc Bolt

| Rank | Upgrade |
|---|---|
| 1 | Fires a basic arcane projectile |
| 2 | +10% projectile speed |
| 3 | -10% mana cost |
| 4 | Slight tracking against locked target |
| 5 | Critical hits restore 5 mana |

### Minor Heal

| Rank | Upgrade |
|---|---|
| 1 | Restores a small amount of health |
| 2 | +15% healing |
| 3 | -10% mana cost |
| 4 | Briefly increases defense after healing |
| 5 | Removes one minor negative status effect |

---

# 14. Ability Tags

Tags are used for filtering, upgrades, AI, gear bonuses, UI, and skill tree rules.

Recommended tags:

```text
Melee
Ranged
Projectile
Area
Cone
Self
Targeted
Movement
Defensive
Healing
Buff
Debuff
Fire
Arcane
Holy
Physical
Stagger
GuardBreak
Shield
Warrior
Mage
Paladin
Weapon
ClassAbility
EnemyAbility
BossAbility
```

Examples:

```text
Power Strike:
Warrior, Melee, Physical, Stagger, ClassAbility

Arc Bolt:
Mage, Projectile, Arcane, Ranged, ClassAbility

Minor Heal:
Paladin, Healing, Holy, Self, ClassAbility
```

---

# 15. Animation Integration

## 15.1 Required Animation Events

Abilities should support animation events such as:

```text
Ability Start
Resource Spend
Hitbox Enable
Hitbox Disable
Projectile Spawn
VFX Spawn
SFX Play
Damage Apply
Recovery Start
Ability End
```

## 15.2 Movement Lock Modes

Each ability should define how much it restricts player control.

```text
No Lock
Rotation Only
Rooted During Cast
Rooted During Hit
Full Lock
Movement Allowed While Casting
```

Examples:

```text
Arc Bolt:
Movement allowed slowly while casting.

Power Strike:
Rooted during attack swing.

Minor Heal:
Rooted during cast.

Dodge:
Full movement override.

Iron Guard:
Movement slowed while active.
```

---

# 16. VFX and SFX Integration

## 16.1 VFX Types

Abilities should support:

```text
Start VFX
Cast VFX
Projectile VFX
Impact VFX
Area VFX
Buff Aura VFX
Shield VFX
Heal VFX
Cooldown Ready VFX
```

## 16.2 SFX Types

Abilities should support:

```text
Start Sound
Cast Sound
Swing Sound
Projectile Sound
Impact Sound
Buff Loop Sound
Shield Hit Sound
Heal Sound
Failure Sound
Cooldown Ready Sound
```

## 16.3 Feedback Rule

Every ability should clearly communicate:

```text
It started.
It activated.
It hit or missed.
It succeeded or failed.
It ended.
```

---

# 17. UI Integration

## 17.1 HUD Requirements

The HUD should show:

```text
Equipped abilities
Cooldown state
Resource cost availability
Selected target
Ability ready state
Blocked/disabled state
```

## 17.2 Tooltip Requirements

Ability tooltips should show:

```text
Name
Description
Class/Type
Resource Cost
Cooldown
Damage/Healing
Scaling Stat
Range
Status Effects
Requirements
Upgrade Rank
```

Example:

```text
Power Strike
Warrior Ability

A heavy melee attack that deals bonus physical damage and staggers weaker enemies.

Cost: 25 Stamina
Cooldown: 4s
Damage: 35 + Strength Scaling
Type: Physical / Melee / Stagger
Requires: Melee Weapon
```

## 17.3 Error Feedback

When an ability cannot be used, the UI should explain why.

Examples:

```text
Not enough stamina.
Not enough mana.
Ability on cooldown.
Requires shield.
No valid target.
Cannot use while staggered.
```

---

# 18. Enemy AI Ability Usage

## 18.1 Enemy Ability Selection

AI should choose abilities based on:

```text
Distance to player
Line of sight
Current health
Cooldown availability
Group role
Player state
Enemy aggression level
Encounter phase
```

## 18.2 AI Ability Rules

Each AI ability should define:

```text
Preferred Range
Minimum Range
Maximum Range
Cooldown
Weight/Priority
Use Conditions
Retreat Conditions
Combo Followups
Interruptibility
```

Example:

```text
Bandit Kick:
Use when player is blocking and within melee range.
Cooldown: 8s
Purpose: Guard pressure.
```

## 18.3 Boss Ability Phases

Bosses should use phase-based ability logic.

Example:

```text
Captain Varric Phase 1:
Sword Slash
Shield Block
Shield Bash

Captain Varric Phase 2:
Shield Charge
Rally Reinforcements
Heavy Overhead Strike

Captain Varric Low Health:
Desperation Combo
War Cry
```

---

# 19. Balance Rules

## 19.1 Cost vs Power

More powerful abilities should require one or more of:

```text
Higher resource cost
Longer cooldown
Longer cast time
Longer recovery
Shorter range
Higher risk
Specific equipment
Skill tree investment
```

## 19.2 Class Balance

### Warrior

Should dominate:

```text
Melee burst
Stagger
Blocking
Physical pressure
Stamina-based combat
```

Should struggle with:

```text
Long-range magic
Mana efficiency
Large area control
```

### Mage

Should dominate:

```text
Ranged damage
Area spells
Utility
Control
Mana-based burst
```

Should struggle with:

```text
Low health
Surrounded melee pressure
Heavy armor efficiency
```

### Paladin

Should dominate:

```text
Sustain
Defense
Healing
Holy damage
Long fights
```

Should struggle with:

```text
Lower raw damage than Warrior
Less spell variety than Mage
Gear dependence
```

---

## 19.3 Cooldown Guidelines

| Ability Type | Cooldown Range |
|---|---:|
| Basic attack | None or animation-limited |
| Basic projectile | 0.8s–2s |
| Heavy melee ability | 3s–8s |
| Defensive buff | 10s–25s |
| Heal | 6s–20s |
| Movement burst | 8s–20s |
| Strong area spell | 8s–20s |
| Boss ability | 5s–20s |

## 19.4 Resource Cost Guidelines

| Ability Type | Cost Range |
|---|---:|
| Light attack | 5–10 stamina |
| Heavy attack | 15–25 stamina |
| Dodge | 18–30 stamina |
| Basic spell | 8–15 mana |
| Strong spell | 25–45 mana |
| Heal | 15–40 mana |
| Defensive buff | 15–40 mana/stamina |
| Movement burst | 25–40 stamina |

---

# 20. Interrupts, Cancels, and Priority

## 20.1 Ability Priority

Recommended state priority:

```text
Death
Stunned/Staggered
Knockdown
Dodge
Parry
Block
Ability Cast
Basic Attack
Movement
Idle
```

## 20.2 Cancel Rules

Abilities may allow cancels into:

```text
Dodge
Block
Parry
Movement
Another ability
Consumable
Nothing
```

Examples:

```text
Light Attack:
Can cancel into dodge during late recovery.

Power Strike:
Cannot cancel until recovery starts.

Arc Bolt:
Can cancel cast by dodging, but mana is not spent until projectile release.

Minor Heal:
Can be interrupted by stagger.
```

Cancel rules heavily affect combat feel. Generous cancel windows feel responsive but can reduce risk. Strict cancel windows feel weighty but can feel clunky.

---

# 21. Status Effects Applied by Abilities

Abilities may apply status effects.

Recommended MVP statuses:

```text
Staggered
Burning
Poisoned
Bleeding
Shielded
Regenerating
Guard Broken
```

Examples:

```text
Power Strike:
Applies Staggered.

Flame Wave:
Applies Burning.

Mana Shield:
Applies Shielded to self.

Minor Heal:
Applies instant healing and optional Regenerating upgrade.

Shield Bash:
Applies Guard Broken if target is blocking.
```

---

# 22. Equipment Interaction

## 22.1 Weapon Requirements

Examples:

```text
Shield Bash requires shield.
Power Strike requires melee weapon.
Arc Bolt requires staff, wand, empty hand, or spell focus depending on design.
Holy Strike requires melee weapon.
```

## 22.2 Equipment Modifiers

Equipment can modify abilities.

Examples:

```text
+10% Arc Bolt damage
-10% Power Strike stamina cost
+2s Radiant Guard duration
+15% Minor Heal effectiveness
Flame Wave applies burn longer
Shield Bash deals more stagger
```

## 22.3 Durability Interaction

Some abilities should affect durability.

Examples:

```text
Heavy attacks cost more weapon durability.
Shield abilities cost shield durability when hit.
Magic abilities may cost staff/wand durability if desired.
Repair-focused skills reduce ability-related durability loss.
```

---

# 23. Backstory Interaction

## 23.1 Soldier

Possible ability interactions:

```text
Lower stamina cost for weapon abilities
Improved weapon handling
Slightly faster recovery after melee abilities
Access to military combat dialogue/actions
```

## 23.2 Student

Possible ability interactions:

```text
Learns new abilities from books faster
Reduced trainer cost for magic/theory abilities
Can identify ability scrolls earlier
Gains extra upgrade point every 5 levels
```

## 23.3 Peasant

Possible ability interactions:

```text
Better utility abilities
Improved gathering/field repair actions
Food and healing item abilities are stronger
Can unlock practical survival actions
```

---

# 24. Save/Load Requirements

The save system must preserve ability progress.

Required save data:

```text
Unlocked Abilities
Equipped Ability Loadout
Ability Ranks
Ability Cooldown State, if saving mid-combat
Ability Upgrade Choices
Trainer Unlocks
Quest-Locked Abilities
Temporary Ability Buffs, if relevant
```

Recommended structure:

```text
PlayerAbilityData
  UnlockedAbilityIds
  EquippedSlotAbilityIds
  AbilityRanks
  AbilityUpgradeNodeIds
  AbilityCooldownStates
```

---

# 25. Multiplayer Considerations, If Added Later

The RPG is assumed to be single-player first. However, the ability system should be clean enough to support multiplayer later.

For multiplayer, ability activation should be:

```text
Input requested by client
Validated by server
Executed by server
Replicated to clients
VFX/SFX predicted or broadcast
Damage applied server-side
Cooldown/resource state synchronized
```

Avoid designing abilities that only work through local-only assumptions.

---

# 26. MVP Ability List

The first playable version should include a small but complete ability set.

## 26.1 Universal Player Actions

```text
Light Attack
Heavy Attack
Block
Parry
Dodge
Use Consumable
Lock-On
```

## 26.2 Warrior Abilities

```text
Power Strike
Iron Guard
Battle Rush
```

## 26.3 Mage Abilities

```text
Arc Bolt
Flame Wave
Mana Shield
```

## 26.4 Paladin Abilities

```text
Holy Strike
Minor Heal
Radiant Guard
```

## 26.5 Enemy Abilities

```text
Bandit Slash
Bandit Heavy Swing
Bandit Kick
Shieldman Block
Deserter Arrow Shot
Wolf Pounce
Shrine Husk Burst
```

## 26.6 Boss Abilities

Captain Varric should have:

```text
Sword Combo
Shield Bash
Shield Charge
Heavy Overhead Strike
Rally Shout
Desperation Combo
```

---

# 27. Recommended Implementation Order

The ability system should be built in this order:

```text
1. Ability data model
2. Ability runtime component
3. Resource cost validation
4. Cooldown system
5. Basic melee ability execution
6. Animation event hit timing
7. Projectile ability execution
8. Self-target buff/heal ability execution
9. Area/cone effect execution
10. UI cooldown/resource display
11. Enemy AI ability usage
12. Ability upgrades
13. Equipment modifiers
14. Save/load ability data
15. Boss phase ability logic
```

---

# 28. Suggested Technical Components

## 28.1 Character Ability Controller

Responsible for:

```text
Receiving input
Checking equipped abilities
Requesting ability activation
Tracking active ability state
Managing cooldowns
Communicating with animation/combat systems
```

## 28.2 Ability Executor

Responsible for:

```text
Running ability logic
Spawning projectiles
Applying effects
Triggering VFX/SFX
Handling cast/channel/recovery
```

## 28.3 Ability Effect Resolver

Responsible for:

```text
Applying damage
Applying healing
Applying buffs/debuffs
Applying movement effects
Checking resistance/block/parry/dodge
```

## 28.4 Ability Database

Responsible for:

```text
Storing ability definitions
Resolving ability IDs
Providing data to UI/combat/save systems
```

---

# 29. Example Pseudocode

```csharp
public bool TryUseAbility(string abilityId, Character caster)
{
    AbilityDefinition ability = AbilityDatabase.Get(abilityId);

    if (!caster.IsAlive)
        return false;

    if (!caster.Abilities.HasUnlocked(abilityId))
        return false;

    if (caster.Abilities.IsOnCooldown(abilityId))
        return false;

    if (!caster.Resources.HasEnough(ability.ResourceCostType, ability.ResourceCostAmount))
        return false;

    if (!AbilityRequirementChecker.CanUse(ability, caster))
        return false;

    caster.Abilities.BeginAbility(ability);
    return true;
}
```

```csharp
public void ActivateAbility(AbilityDefinition ability, Character caster)
{
    caster.Resources.Spend(ability.ResourceCostType, ability.ResourceCostAmount);
    caster.Abilities.StartCooldown(ability.AbilityId, ability.Cooldown);

    foreach (AbilityEffectDefinition effect in ability.Effects)
    {
        AbilityEffectResolver.Resolve(effect, caster);
    }
}
```

---

# 30. Technical Risk Areas

## 30.1 Hardcoding Too Many Abilities

Risk:

```text
Every ability becomes a custom script with unique logic.
```

Mitigation:

```text
Build generic effect types first.
Use data definitions.
Only create custom scripts for truly unique boss abilities.
```

## 30.2 Too Many Abilities Too Early

Risk:

```text
The game becomes hard to balance before combat feels good.
```

Mitigation:

```text
Start with 3 abilities per class.
Add more only after the base combat loop is fun.
```

## 30.3 Poor Animation Timing

Risk:

```text
Abilities feel disconnected, floaty, or unfair.
```

Mitigation:

```text
Use animation events.
Tune hit windows.
Add strong VFX/SFX feedback.
Test cancel windows carefully.
```

## 30.4 UI Clarity Problems

Risk:

```text
Players do not understand why abilities fail or when they are ready.
```

Mitigation:

```text
Show cooldowns clearly.
Show insufficient mana/stamina clearly.
Show requirement failures clearly.
```

## 30.5 Enemy Ability Fairness

Risk:

```text
Enemy attacks feel cheap or unreadable.
```

Mitigation:

```text
Use clear telegraphs.
Limit enemy ability spam.
Give recovery windows after major attacks.
```

---

# 31. Final Recommendations

For the MVP, the ability system should stay focused and practical.

Build these first:

```text
Universal:
Light Attack
Heavy Attack
Block
Parry
Dodge
Use Consumable

Warrior:
Power Strike
Iron Guard
Battle Rush

Mage:
Arc Bolt
Flame Wave
Mana Shield

Paladin:
Holy Strike
Minor Heal
Radiant Guard

Enemies:
Basic melee attack
Heavy attack
Block
Kick
Ranged shot
Pounce
Burst attack

Boss:
Shield Bash
Shield Charge
Heavy Strike
Rally
Desperation Combo
```

The guiding design rule should be:

> **Every ability should have a clear purpose, clear cost, clear counterplay, and clear class identity.**

A successful first version of the ability system should allow the player to choose a class, equip three active abilities, fight enemies in real time, spend stamina/mana, see cooldowns, upgrade abilities, and experience meaningful differences between Warrior, Mage, and Paladin gameplay.
