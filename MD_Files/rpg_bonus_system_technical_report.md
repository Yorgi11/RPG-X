# RPG Bonus System Technical Report

## 1. Executive Summary

This report defines a complete bonus system taxonomy for a third-person action/open-world RPG built around custom character creation, class selection, backstory selection, real-time combat, limited inventory, equipment durability, repair, merchants, quests, reputation, and region-based world progression.

The purpose of this document is to provide a structured list of possible bonuses that can be applied through:

- Classes
- Backstories
- Equipment
- Skills
- Consumables
- Buffs
- Enchantments
- Quest rewards
- Reputation rewards
- Faction rewards
- Region-specific effects

The system should be designed so that bonuses are modular, data-driven, stackable where appropriate, and easy to balance. The player-facing goal is to make every class, backstory, equipment choice, and progression path feel meaningful without creating one obviously superior build.

---

## 2. Core Design Goals

### 2.1 Support Build Identity

Bonuses should help define different playstyles.

Examples:

- Warrior bonuses should improve melee damage, blocking, stamina, armor, and weapon handling.
- Mage bonuses should improve mana, spell damage, cast speed, spell range, and mana efficiency.
- Paladin bonuses should improve defense, healing, holy damage, shields, and sustain.
- Soldier bonuses should support combat readiness and weapon handling.
- Student bonuses should support learning, XP gain, lore, and long-term progression.
- Peasant bonuses should support survival, carry weight, gathering, food, economy, and village interactions.

### 2.2 Avoid One Best Build

No single bonus category should dominate the game.

Bad design example:

```text
Student gives so many upgrade points that every optimal build must choose Student.
```

Better design:

```text
Student scales better over time, but Soldier is stronger early and Peasant has better survival/economy value.
```

### 2.3 Keep Bonuses Readable

A player should understand what a bonus does from the name and tooltip.

Good examples:

```text
+10% Melee Damage
+15 Max Stamina
-10% Spell Mana Cost
+5% Better Sell Prices
```

Avoid unclear bonuses such as:

```text
+3 Combat Efficiency
+7 Utility Value
+4 Survival Rating
```

### 2.4 Make Bonuses Data-Driven

Bonuses should be stored as data instead of hardcoded individually. This allows designers to attach the same bonus system to classes, items, skills, buffs, food, quests, and reputation rewards.

---

## 3. Bonus System Architecture

### 3.1 Bonus Source Types

Every bonus should come from a source.

Recommended source categories:

```text
Class
Backstory
Skill
Equipment
Consumable
Temporary Buff
Permanent Quest Reward
Faction Reward
Region Effect
Status Effect
Enchantment
Difficulty Modifier
```

### 3.2 Bonus Duration Types

Bonuses should have a defined duration model.

Recommended duration types:

```text
Permanent
While Equipped
Timed
Until Rest
Until Region Exit
While In Region
While Buff Active
While Health Below Threshold
While Mana Below Threshold
While Over/Under Weight Threshold
```

### 3.3 Bonus Operation Types

Each bonus should define how it modifies the target value.

Recommended operation types:

```text
Flat Add
Flat Subtract
Percentage Add
Percentage Subtract
Multiplier
Override
Conditional Modifier
Triggered Effect
```

Examples:

```text
+20 Max Health                       = Flat Add
+10% Melee Damage                    = Percentage Add
Damage Taken × 0.9                   = Multiplier
Sell Price becomes 45% instead of 40% = Override
Restore 10 Stamina after parry       = Triggered Effect
```

### 3.4 Recommended Bonus Data Shape

A generic bonus definition should include:

```text
Bonus ID
Display Name
Description
Affected Stat
Operation Type
Value
Source Type
Duration Type
Stacking Rule
Condition
Priority
Tags
```

Example:

```text
Bonus ID: warrior_melee_damage_01
Display Name: Warrior Melee Training
Affected Stat: MeleeDamage
Operation Type: Percentage Add
Value: 10%
Source Type: Class
Duration Type: Permanent
Stacking Rule: Additive
Condition: None
Tags: Warrior, Melee, ClassBonus
```

---

## 4. Core Stat Bonuses

Core stat bonuses directly improve character attributes.

| Bonus | Example |
|---|---|
| Max Health | `+20 Max Health` |
| Max Stamina | `+15 Max Stamina` |
| Max Mana | `+30 Max Mana` |
| Strength | `+2 Strength` |
| Endurance | `+2 Endurance` |
| Intelligence | `+3 Intelligence` |
| Faith | `+3 Faith` |
| Agility | `+2 Agility` |
| Carry Weight | `+10 Carry Weight` |

### Design Notes

Core stat bonuses are useful for classes, backstories, permanent rewards, equipment, and skill trees.

Recommended early use:

- Warrior: Health, Stamina, Strength, Endurance
- Mage: Mana, Intelligence
- Paladin: Health, Mana, Endurance, Faith
- Soldier: Stamina, Strength
- Student: Intelligence
- Peasant: Health, Endurance, Carry Weight

---

## 5. Resource Regeneration Bonuses

Resource regeneration bonuses affect how quickly Health, Stamina, or Mana recover.

| Bonus | Example |
|---|---|
| Health Regeneration | `+1 Health per second` |
| Stamina Regeneration | `+10% Stamina Regeneration` |
| Mana Regeneration | `+10% Mana Regeneration` |
| Out-of-Combat Healing | `Recover health slowly outside combat` |
| Faster Stamina Recovery Delay | `Stamina begins recovering 0.5s sooner` |
| Faster Mana Recovery Delay | `Mana begins recovering 0.5s sooner` |
| Increased Potion Recovery | `+10% healing from potions` |
| Increased Food Recovery | `+10% healing from food` |

### Design Notes

Resource regeneration should be handled carefully. Too much regeneration can remove tension from combat and exploration.

Recommended restrictions:

- Health regeneration should usually be slow or conditional.
- Stamina regeneration can be more common.
- Mana regeneration should be strong enough for Mage identity but not so strong that mana management disappears.

---

## 6. Melee Combat Bonuses

Melee bonuses improve close-range physical combat.

| Bonus | Example |
|---|---|
| Melee Damage | `+10% Melee Damage` |
| Light Attack Damage | `+8% Light Attack Damage` |
| Heavy Attack Damage | `+12% Heavy Attack Damage` |
| Charged Attack Damage | `+15% Charged Attack Damage` |
| Sprint Attack Damage | `+10% Sprint Attack Damage` |
| Backstab Damage | `+20% Backstab Damage` |
| Finisher Damage | `+15% Finisher Damage` |
| Stagger Damage | `+10% Stagger Damage` |
| Guard Break Damage | `+15% Guard Break Damage` |
| Critical Hit Chance | `+5% Critical Chance` |
| Critical Hit Damage | `+20% Critical Damage` |
| Weapon Handling | `+5% faster melee recovery` |
| Attack Speed | `+5% Attack Speed` |
| Combo Damage | `+5% damage per successful combo hit` |

### Design Notes

Melee bonuses should primarily support Warrior and Soldier identities, but some can appear on equipment and general combat skills.

Avoid stacking too many percentage damage bonuses early.

---

## 7. Weapon-Specific Bonuses

Weapon-specific bonuses encourage build specialization.

| Bonus | Example |
|---|---|
| Sword Damage | `+10% Sword Damage` |
| Axe Damage | `+10% Axe Damage` |
| Mace Damage | `+10% Mace Damage` |
| Spear Damage | `+10% Spear Damage` |
| Dagger Damage | `+10% Dagger Damage` |
| Staff Damage | `+10% Staff Damage` |
| Wand Damage | `+10% Wand Damage` |
| Shield Bash Damage | `+10% Shield Bash Damage` |
| Unarmed Damage | `+10% Unarmed Damage` |
| Heavy Weapon Damage | `+10% Heavy Weapon Damage` |
| One-Handed Damage | `+10% One-Handed Damage` |
| Two-Handed Damage | `+10% Two-Handed Damage` |

### Design Notes

Weapon-specific bonuses should come mostly from skill trees, equipment perks, trainers, and weapon mastery systems.

Example use:

```text
Warrior skill: +10% damage with swords and axes.
Paladin skill: +10% damage with maces and shields.
Mage skill: +10% staff and wand damage.
```

---

## 8. Ranged Combat Bonuses

Ranged combat bonuses apply if the game includes bows, crossbows, thrown weapons, or physical ranged tools.

| Bonus | Example |
|---|---|
| Ranged Damage | `+10% Ranged Damage` |
| Bow Damage | `+10% Bow Damage` |
| Crossbow Damage | `+10% Crossbow Damage` |
| Throwing Weapon Damage | `+10% Throwing Weapon Damage` |
| Projectile Speed | `+10% Projectile Speed` |
| Reload Speed | `+10% Reload Speed` |
| Draw Speed | `+10% Bow Draw Speed` |
| Headshot Damage | `+20% Headshot Damage` |
| Ammo Recovery Chance | `15% chance to recover arrows` |
| Ranged Critical Chance | `+5% Ranged Critical Chance` |

### Design Notes

Ranged physical combat can easily compete with Mage ranged combat. Balance physical ranged damage around ammo, reload/draw timing, durability, and positioning.

---

## 9. Magic Bonuses

Magic bonuses support spellcasting, mana efficiency, utility, and Mage progression.

| Bonus | Example |
|---|---|
| Spell Damage | `+15% Spell Damage` |
| Mana Cost Reduction | `-10% Mana Cost` |
| Cast Speed | `+10% Cast Speed` |
| Spell Range | `+10% Spell Range` |
| Spell Projectile Speed | `+10% Projectile Speed` |
| Spell Area Size | `+10% Area of Effect` |
| Spell Duration | `+15% Spell Duration` |
| Spell Critical Chance | `+5% Spell Critical Chance` |
| Spell Critical Damage | `+20% Spell Critical Damage` |
| Channeling Stability | `Less interruption while casting` |
| Magic Resistance Penetration | `Ignore 10% enemy magic resistance` |

### Design Notes

Magic bonuses should support multiple Mage playstyles:

- Direct damage caster
- Area damage caster
- Control caster
- Defensive caster
- Utility caster

Mana cost reduction should be capped or carefully stacked.

---

## 10. Elemental Damage Bonuses

Elemental bonuses improve specific damage types.

| Bonus | Example |
|---|---|
| Fire Damage | `+10% Fire Damage` |
| Frost Damage | `+10% Frost Damage` |
| Lightning Damage | `+10% Lightning Damage` |
| Arcane Damage | `+10% Arcane Damage` |
| Poison Damage | `+10% Poison Damage` |
| Bleed Damage | `+10% Bleed Damage` |
| Holy Damage | `+10% Holy Damage` |
| Shadow/Dark Damage | `+10% Dark Damage` |
| Earth Damage | `+10% Earth Damage` |
| Wind Damage | `+10% Wind Damage` |

### Design Notes

For the MVP, only a few damage types are needed.

Recommended MVP damage types:

```text
Physical
Fire
Arcane
Holy
Poison
Bleed
```

More elemental types can be added after the combat system is stable.

---

## 11. Healing and Support Bonuses

Healing and support bonuses are important for Paladin-style gameplay and defensive builds.

| Bonus | Example |
|---|---|
| Healing Effectiveness | `+15% Healing Done` |
| Self-Healing Bonus | `+10% Healing Received from own spells` |
| Healing Received | `+10% Healing Received` |
| Revive Speed | `+15% faster revive`, if companions exist |
| Buff Duration | `+15% Buff Duration` |
| Aura Range | `+20% Aura Radius` |
| Shield Strength | `+10% Magic Shield Strength` |
| Barrier Duration | `+10% Barrier Duration` |
| Cleanse Effectiveness | `Remove stronger negative effects` |
| Holy Healing Bonus | `+10% healing from holy abilities` |

### Design Notes

Healing can trivialize combat if it is too cheap. Balance healing with mana cost, cooldowns, cast time, limited consumables, or vulnerability during use.

---

## 12. Defensive Bonuses

Defensive bonuses improve survival.

| Bonus | Example |
|---|---|
| Defense | `+10% Defense` |
| Armor Effectiveness | `+10% Armor Effectiveness` |
| Physical Resistance | `+10% Physical Resistance` |
| Magic Resistance | `+10% Magic Resistance` |
| Fire Resistance | `+10% Fire Resistance` |
| Frost Resistance | `+10% Frost Resistance` |
| Lightning Resistance | `+10% Lightning Resistance` |
| Poison Resistance | `+10% Poison Resistance` |
| Bleed Resistance | `+10% Bleed Resistance` |
| Holy Resistance | `+10% Holy Resistance` |
| Critical Damage Reduction | `-15% Critical Damage Taken` |
| Backstab Resistance | `-20% Backstab Damage Taken` |
| Stagger Resistance | `+10% Stagger Resistance` |
| Knockdown Resistance | `+10% Knockdown Resistance` |

### Design Notes

Defense should not make the player invincible. Prefer partial reductions and situational resistances over extreme global damage reduction.

---

## 13. Blocking and Parrying Bonuses

Blocking and parrying bonuses support shield users, Warriors, Paladins, and skill-based defensive play.

| Bonus | Example |
|---|---|
| Block Damage Reduction | `+10% Blocked Damage Reduction` |
| Block Stamina Efficiency | `-15% Stamina Loss While Blocking` |
| Shield Stability | `+10% Shield Stability` |
| Parry Window | `+0.1s Parry Window` |
| Parry Stamina Refund | `Recover 10 Stamina on successful parry` |
| Parry Damage Bonus | `+20% damage after parry` |
| Guard Break Resistance | `+10% Guard Break Resistance` |
| Shield Bash Stagger | `+15% Shield Bash Stagger` |
| Perfect Block Bonus | `Perfect block costs no stamina` |

### Design Notes

Parry bonuses should be powerful but skill-dependent. Block bonuses should be reliable but still consume stamina.

---

## 14. Dodge and Mobility Bonuses

Dodge and mobility bonuses improve movement, evasion, and action responsiveness.

| Bonus | Example |
|---|---|
| Dodge Distance | `+10% Dodge Distance` |
| Dodge Stamina Cost | `-10% Dodge Cost` |
| Dodge Recovery Speed | `+10% faster recovery after dodge` |
| Sprint Speed | `+5% Sprint Speed` |
| Movement Speed | `+5% Movement Speed` |
| Climb Speed | `+10% Climb Speed` |
| Jump Height | `+5% Jump Height` |
| Fall Damage Reduction | `-25% Fall Damage` |
| Lock-On Strafe Speed | `+10% Strafe Speed` |
| Evasion Chance | `Small chance to avoid glancing hits` |

### Design Notes

Movement speed bonuses should stay modest. Large movement bonuses can break encounter design, navigation, and enemy attack balance.

---

## 15. Stamina Efficiency Bonuses

Stamina efficiency bonuses reduce the stamina cost of physical actions.

| Bonus | Example |
|---|---|
| Attack Stamina Cost Reduction | `-10% stamina cost for attacks` |
| Sprint Stamina Cost Reduction | `-10% sprint stamina drain` |
| Dodge Stamina Cost Reduction | `-10% dodge stamina cost` |
| Block Stamina Cost Reduction | `-10% block stamina drain` |
| Climbing Stamina Cost Reduction | `-10% climbing stamina drain` |
| Heavy Weapon Stamina Reduction | `-10% stamina cost with heavy weapons` |
| Stamina Recovery After Kill | `Restore 10 Stamina after kill` |
| Stamina Recovery After Parry | `Restore 15 Stamina after parry` |

### Design Notes

Stamina efficiency is very important for Warrior and Soldier builds. Avoid letting stamina become irrelevant too early.

---

## 16. Mana Efficiency Bonuses

Mana efficiency bonuses reduce mana costs or improve mana recovery through gameplay triggers.

| Bonus | Example |
|---|---|
| Spell Mana Cost Reduction | `-10% spell mana cost` |
| Healing Mana Cost Reduction | `-10% healing spell cost` |
| Buff Mana Cost Reduction | `-10% buff spell cost` |
| Mana Refund Chance | `10% chance to refund mana on spell hit` |
| Mana on Kill | `Restore 5 Mana after killing enemy` |
| Mana on Critical Hit | `Restore 3 Mana on spell critical` |
| Mana Shield Efficiency | `Mana Shield absorbs more per mana spent` |
| Low Mana Bonus | `+10% mana regen below 25% mana` |

### Design Notes

Mana refund and mana-on-kill bonuses encourage active combat. Passive mana regeneration is easier to balance but less interesting.

---

## 17. Durability and Repair Bonuses

Durability and repair bonuses support equipment management and resource economy.

| Bonus | Example |
|---|---|
| Weapon Durability Loss Reduction | `-15% weapon durability loss` |
| Armor Durability Loss Reduction | `-15% armor durability loss` |
| Shield Durability Loss Reduction | `-15% shield durability loss` |
| Repair Cost Reduction | `-10% repair cost` |
| Repair Material Reduction | `-1 required repair material` |
| Field Repair Bonus | `Can repair at camps` |
| Repair Quality Bonus | `Repairs restore 10% more durability` |
| Broken Gear Penalty Reduction | `Broken gear loses only 30% effectiveness instead of 50%` |

### Design Notes

These bonuses are especially useful for Peasant, crafting, survival, and blacksmith-related skill trees.

---

## 18. Inventory and Carry Bonuses

Inventory and carry bonuses support looting, exploration, crafting, and survival.

| Bonus | Example |
|---|---|
| Carry Weight | `+10 Carry Weight` |
| Inventory Slots | `+4 Inventory Slots` |
| Stack Size | `+5 max stack size for materials` |
| Potion Stack Size | `+2 potion stack size` |
| Material Stack Size | `+10 material stack size` |
| Reduced Armor Weight | `-10% equipped armor weight` |
| Reduced Weapon Weight | `-10% weapon weight` |
| Reduced Material Weight | `-20% crafting material weight` |
| Over-Encumbrance Tolerance | `Can move normally up to 80% weight instead of 70%` |

### Design Notes

Inventory bonuses should reduce frustration without eliminating meaningful inventory decisions.

---

## 19. Loot and Gathering Bonuses

Loot and gathering bonuses reward exploration and resource collection.

| Bonus | Example |
|---|---|
| Gathering Yield | `+10% more gathered materials` |
| Herb Yield | `+1 extra herb when gathering` |
| Ore Yield | `+1 extra ore when mining` |
| Hide/Pelt Yield | `+10% more animal materials` |
| Rare Loot Chance | `+5% rare loot chance` |
| Gold Find | `+10% coins found` |
| Container Loot Bonus | `Better loot from chests` |
| Enemy Drop Chance | `+5% enemy drop chance` |
| Boss Loot Bonus | `Bosses drop one extra material` |
| Quest Reward Bonus | `+5% quest coin rewards` |

### Design Notes

Peasant and survival/crafting builds should benefit from these bonuses.

---

## 20. Economy Bonuses

Economy bonuses affect buying, selling, repairs, bribes, and merchant relationships.

| Bonus | Example |
|---|---|
| Better Sell Prices | `Sell items for 45% instead of 40% value` |
| Merchant Discount | `-10% buy prices` |
| Repair Discount | `-10% blacksmith repair prices` |
| Crafting Discount | `-10% crafting station fee` |
| Barter Bonus | `Improved dialogue trade options` |
| Common Merchant Bonus | `Better prices with village merchants` |
| Blacksmith Bonus | `Better prices with blacksmiths` |
| Alchemist Bonus | `Better prices with alchemists` |
| Magic Vendor Bonus | `Better prices with magic vendors` |
| Bribe Reduction | `-10% cost to bribe NPCs` |

### Design Notes

Economy bonuses are useful but should not allow infinite money exploits. Sell-price bonuses and merchant discounts should have clear caps.

---

## 21. XP and Progression Bonuses

XP and progression bonuses affect leveling speed, upgrade points, skill costs, and training.

| Bonus | Example |
|---|---|
| XP Gain | `+5% XP Gain` |
| Combat XP Gain | `+5% XP from enemies` |
| Quest XP Gain | `+5% XP from quests` |
| Discovery XP Gain | `+10% XP from discovering locations` |
| Lore XP Gain | `+10% XP from reading ancient texts` |
| Bonus Upgrade Points | `+1 upgrade point every 5 levels` |
| Skill Cost Reduction | `Certain skills cost 1 less point` |
| Faster Skill Unlocks | `Unlock skill tier one level earlier` |
| Trainer Discount | `-10% training cost` |

### Design Notes

Progression bonuses are powerful. The Student backstory should scale better over time, but the bonus should not become mandatory for all optimal builds.

---

## 22. Dialogue and Social Bonuses

Dialogue and social bonuses affect role-playing options and NPC interactions.

| Bonus | Example |
|---|---|
| Intimidation Bonus | `Easier aggressive checks` |
| Persuasion Bonus | `Easier friendly checks` |
| Barter Dialogue Bonus | `Unlocks better trade dialogue` |
| Guard Dialogue Bonus | `More options with guards` |
| Villager Trust Bonus | `More options with peasants/workers` |
| Scholar Dialogue Bonus | `More options with mages/scholars` |
| Temple Dialogue Bonus | `More options with priests/paladins` |
| Bandit Dialogue Bonus | `Can negotiate with criminals more easily` |
| Reputation Gain Bonus | `+10% reputation gains with villages` |
| Reputation Loss Reduction | `-10% reputation loss from minor crimes` |

### Design Notes

Dialogue bonuses should not always be numeric. Some should simply unlock unique options.

---

## 23. Reputation and Faction Bonuses

Reputation and faction bonuses affect how groups respond to the player.

| Bonus | Example |
|---|---|
| Village Reputation Gain | `+10% village reputation gains` |
| Merchant Reputation Gain | `+10% merchant reputation gains` |
| Military Reputation Gain | `+10% military reputation gains` |
| Temple Reputation Gain | `+10% temple reputation gains` |
| Mage Faction Reputation Gain | `+10% mage reputation gains` |
| Bandit Fear Gain | `Bandits fear you faster` |
| Bounty Reduction | `Lower bounty from minor crimes` |
| Faction Discount | `Faction merchants offer lower prices` |
| Faction Access | `Can enter faction-restricted areas sooner` |
| Faction Quest Bonus | `Extra rewards from faction quests` |

### Design Notes

Faction bonuses should make the world feel reactive. They are best used as rewards for quest outcomes and reputation thresholds.

---

## 24. Quest Bonuses

Quest bonuses modify quest rewards, options, and outcomes.

| Bonus | Example |
|---|---|
| Extra Quest Rewards | `+5% coins from quests` |
| Bonus Quest XP | `+5% quest XP` |
| Hidden Objective Detection | `Reveals optional objectives` |
| Better Negotiation Outcomes | `Unlock peaceful quest resolution` |
| Class Quest Bonus | `Extra reward from class-specific quests` |
| Backstory Quest Bonus | `Extra reward from backstory-related quests` |
| More Quest Dialogue | `Unlock extra context or shortcuts` |
| Reduced Failure Penalty | `Minor mistakes reduce rewards less` |

### Design Notes

Quest bonuses should support roleplay without making non-optimized characters feel punished.

---

## 25. Exploration Bonuses

Exploration bonuses help the player navigate, discover, and survive the world.

| Bonus | Example |
|---|---|
| Discover Range | `Discover locations from farther away` |
| Map Detail | `Reveals more local map information` |
| Hidden Loot Detection | `Highlights nearby hidden containers` |
| Trap Detection | `Detect traps from farther away` |
| Secret Door Detection | `Chance to notice hidden entrances` |
| Landmark XP Bonus | `More XP from discovering landmarks` |
| Faster Fast Travel Unlock | `Unlock fast travel points more easily` |
| Safer Road Travel | `Reduced chance of road ambushes` |
| Better Camp Rest | `Recover more from resting at camps` |

### Design Notes

Exploration bonuses are ideal for Peasant, Student, survival, and region-specific rewards.

---

## 26. Crafting Bonuses

Crafting bonuses improve crafting efficiency, quality, and recipe access.

| Bonus | Example |
|---|---|
| Crafting Cost Reduction | `-10% required materials` |
| Potion Crafting Bonus | `Potions restore 10% more` |
| Food Crafting Bonus | `Cooked food gives stronger effects` |
| Weapon Crafting Bonus | `Crafted weapons start with +10 durability` |
| Armor Crafting Bonus | `Crafted armor has +5% defense` |
| Material Refining Bonus | `More refined materials from raw materials` |
| Rare Crafting Chance | `Small chance to create higher-quality item` |
| Recipe Discovery | `Can learn recipes from found materials` |
| Faster Crafting | `Craft items faster at stations` |

### Design Notes

Crafting bonuses are useful for Peasant, survival, and economy builds. Keep the MVP crafting tree small.

---

## 27. Consumable Bonuses

Consumable bonuses improve potions, food, tonics, and temporary item use.

| Bonus | Example |
|---|---|
| Potion Healing Bonus | `+10% potion healing` |
| Mana Potion Bonus | `+10% mana restored` |
| Food Effectiveness | `+10% food effects` |
| Consumable Duration | `+15% consumable effect duration` |
| Faster Item Use | `Use consumables 20% faster` |
| Item Use While Moving | `Can use some items while walking` |
| Potion Cooldown Reduction | `-10% potion cooldown` |
| Chance Not to Consume Item | `5% chance potion is not consumed` |

### Design Notes

Consumable bonuses are effective for survival and resource management but can reduce difficulty if stacked too heavily.

---

## 28. Stealth Bonuses

Stealth bonuses are optional and only required if stealth becomes a major system.

| Bonus | Example |
|---|---|
| Sneak Speed | `+10% crouch movement speed` |
| Detection Reduction | `Enemies detect you 10% slower` |
| Noise Reduction | `Footsteps make less sound` |
| Backstab Damage | `+20% backstab damage` |
| Lockpicking Bonus | `Easier locks` |
| Pickpocket Bonus | `Higher chance to steal successfully` |
| Trap Disarm Bonus | `Easier trap disarming` |
| Ambush Damage | `+15% damage against unaware enemies` |

### Design Notes

Stealth should be deferred unless it is part of the core combat/exploration loop.

---

## 29. Survival Bonuses

Survival bonuses apply if the game includes food, weather, rest, disease, camping, or wilderness pressure.

| Bonus | Example |
|---|---|
| Food Efficiency | `Food restores 10% more` |
| Rest Recovery | `Recover more when resting` |
| Cold Resistance | `Reduced cold/weather penalties` |
| Heat Resistance | `Reduced heat penalties` |
| Poison Environment Resistance | `Reduced damage from toxic areas` |
| Disease Resistance | `Lower chance of sickness`, if used |
| Campfire Bonus | `Resting at camp gives longer buffs` |
| Water/Food Consumption Reduction | `Need supplies less often`, if survival meters exist |

### Design Notes

For this RPG, survival should be light unless the region specifically requires it.

---

## 30. Companion Bonuses

Companion bonuses are only needed if party members or followers exist.

| Bonus | Example |
|---|---|
| Companion Damage | `+10% companion damage` |
| Companion Health | `+10% companion health` |
| Companion Healing | `Companions receive 10% more healing` |
| Companion Revive Speed | `Revive companions faster` |
| Companion Morale | `Companions resist fear or flee less often` |
| Shared XP | `Companions level faster` |
| Command Cooldown Reduction | `Companion commands recharge faster` |

### Design Notes

Companions should be deferred unless they are a planned core feature.

---

## 31. Mount and Travel Bonuses

Mount and travel bonuses are useful for later open-region expansion.

| Bonus | Example |
|---|---|
| Mount Speed | `+10% mount speed` |
| Mount Stamina | `+10% mount stamina` |
| Road Travel Speed | `+10% movement on roads` |
| Fast Travel Cost Reduction | `-10% carriage cost` |
| Ambush Chance Reduction | `Lower chance of random road encounters` |
| Boat Travel Discount | `Cheaper boat travel` |
| Carry Bonus While Mounted | `+20 carry weight while mounted` |

### Design Notes

Travel bonuses should become more important once multiple regions are connected.

---

## 32. Detection and Awareness Bonuses

Detection and awareness bonuses improve information-gathering during exploration and combat.

| Bonus | Example |
|---|---|
| Enemy Awareness | `Enemies appear on compass from farther away` |
| Loot Awareness | `Nearby loot glows faintly` |
| Trap Awareness | `Traps highlight when close` |
| Weak Point Detection | `Reveal enemy weaknesses after observing them` |
| Combat Read Bonus | `Longer warning before enemy heavy attacks` |
| Ambush Detection | `Chance to detect ambush before it triggers` |

### Design Notes

These bonuses are useful for Student, exploration skills, and utility equipment.

---

## 33. Status Effect Bonuses

Status effect bonuses affect application, duration, damage, and resistance.

### 33.1 Offensive Status Bonuses

| Bonus | Example |
|---|---|
| Burn Chance | `+10% chance to burn` |
| Poison Chance | `+10% chance to poison` |
| Bleed Chance | `+10% chance to bleed` |
| Slow Chance | `+10% chance to slow` |
| Stun Chance | `+5% chance to stun` |
| Status Duration | `+15% status duration on enemies` |
| Status Damage | `+10% status damage` |

### 33.2 Defensive Status Bonuses

| Bonus | Example |
|---|---|
| Burn Resistance | `+10% burn resistance` |
| Poison Resistance | `+10% poison resistance` |
| Bleed Resistance | `+10% bleed resistance` |
| Stun Resistance | `+10% stun resistance` |
| Slow Resistance | `+10% slow resistance` |
| Status Duration Reduction | `Negative effects last 15% less time` |
| Cleanse Chance | `Chance to remove negative effect when healed` |

### Design Notes

For MVP, implement only a few statuses first.

Recommended MVP statuses:

```text
Burning
Poisoned
Bleeding
Staggered
Shielded
Regenerating
```

---

## 34. Enemy-Type Bonuses

Enemy-type bonuses make certain builds stronger against certain enemy groups.

| Bonus | Example |
|---|---|
| Damage vs Bandits | `+10% damage against bandits` |
| Damage vs Soldiers | `+10% damage against soldiers` |
| Damage vs Beasts | `+10% damage against beasts` |
| Damage vs Undead | `+10% damage against undead` |
| Damage vs Mages | `+10% damage against mages` |
| Damage vs Armored Enemies | `+10% damage against armored enemies` |
| Damage vs Bosses | `+5% boss damage` |
| Defense vs Beasts | `-10% damage taken from beasts` |

### Design Notes

Enemy-type bonuses are useful for region-specific rewards, faction gear, and specialist skills.

---

## 35. Region-Specific Bonuses

Region-specific bonuses give identity to each region.

| Bonus | Example |
|---|---|
| Greyreach Reputation Bonus | `+10% reputation gains in Greyreach` |
| Forest Survival Bonus | `+10% movement/gathering in forests` |
| Mountain Travel Bonus | `Reduced stamina cost in mountains` |
| Dungeon Loot Bonus | `+5% rare loot in ruins` |
| Shrine Bonus | `Holy abilities stronger near shrines` |
| Road Bonus | `Move faster on cleared roads` |
| Town Trade Bonus | `Better prices in friendly towns` |

### Design Notes

Region-specific bonuses are useful for making each area feel mechanically distinct.

---

## 36. Ability Cooldown Bonuses

Ability cooldown bonuses are useful if class abilities have cooldowns.

| Bonus | Example |
|---|---|
| Ability Cooldown Reduction | `-10% ability cooldowns` |
| Warrior Ability Cooldown | `-10% Warrior skill cooldowns` |
| Mage Ability Cooldown | `-10% spell cooldowns` |
| Paladin Ability Cooldown | `-10% holy ability cooldowns` |
| Cooldown Refund on Kill | `Kills reduce cooldowns by 1s` |
| Cooldown Refund on Parry | `Parries reduce combat ability cooldowns` |

### Design Notes

Cooldown reduction should have a cap to prevent ability spam.

---

## 37. Ability-Specific Bonuses

Ability-specific bonuses modify individual class abilities.

### 37.1 Warrior Ability Bonuses

| Bonus | Example |
|---|---|
| Power Strike Damage | `+15% Power Strike Damage` |
| Power Strike Stagger | `+20% Power Strike Stagger` |
| Iron Guard Duration | `+2s Iron Guard Duration` |
| Battle Rush Range | `+20% Battle Rush Distance` |

### 37.2 Mage Ability Bonuses

| Bonus | Example |
|---|---|
| Arc Bolt Damage | `+10% Arc Bolt Damage` |
| Arc Bolt Mana Cost | `-10% Arc Bolt Cost` |
| Flame Wave Size | `+15% Flame Wave Cone Size` |
| Mana Shield Strength | `+10% Mana Shield Absorption` |

### 37.3 Paladin Ability Bonuses

| Bonus | Example |
|---|---|
| Holy Strike Damage | `+10% Holy Strike Damage` |
| Minor Heal Strength | `+15% Minor Heal` |
| Radiant Guard Duration | `+2s Radiant Guard Duration` |
| Holy Damage vs Undead | `+20% Holy Damage vs Undead` |

### Design Notes

Ability-specific bonuses are excellent for skill tree upgrades and unique equipment perks.

---

## 38. Negative Bonuses and Trade-Offs

Negative bonuses are useful for balancing powerful equipment, risky builds, curses, heavy armor, or class identity.

| Penalty | Example |
|---|---|
| Reduced Health | `-10 Max Health` |
| Reduced Stamina | `-10 Max Stamina` |
| Reduced Mana | `-10 Max Mana` |
| Reduced Movement Speed | `-5% Movement Speed` |
| Increased Mana Cost | `+10% Mana Costs` |
| Increased Stamina Cost | `+10% Stamina Costs` |
| Lower Armor Efficiency | `-10% Heavy Armor Efficiency` |
| Reduced Carry Weight | `-5 Carry Weight` |
| Reduced Healing Received | `-10% Healing Received` |
| Weakness to Fire | `+10% Fire Damage Taken` |
| Weakness to Poison | `+10% Poison Damage Taken` |
| Lower Sell Prices | `-5% Sell Prices` |

### Design Notes

Trade-offs are useful for making powerful bonuses more interesting.

Example:

```text
Scholar's Robe:
+15% Spell Damage
+10% Mana Regeneration
-10% Physical Resistance
```

---

## 39. Recommended MVP Bonus Set

The MVP should not include every bonus type. The first version should use a limited, readable subset.

Recommended MVP bonuses:

```text
Max Health
Max Stamina
Max Mana
Strength
Endurance
Intelligence
Faith
Agility
Carry Weight
Melee Damage
Spell Damage
Holy Damage
Healing Effectiveness
Defense
Armor Effectiveness
Mana Regeneration
Stamina Regeneration
Weapon Handling
XP Gain
Upgrade Point Bonus
Sell Price Bonus
Gathering Yield
Food/Healing Item Bonus
Repair Cost Reduction
Durability Loss Reduction
Block Efficiency
Parry Window
Dodge Cost Reduction
```

This set is enough to support:

- Warrior class
- Mage class
- Paladin class
- Soldier backstory
- Student backstory
- Peasant backstory
- Early equipment
- Basic skill trees
- Simple economy
- Basic crafting/repair
- Early RPG progression

---

## 40. Recommended Class Bonus Implementation

### 40.1 Warrior

Recommended Warrior bonuses:

```text
+20 Max Health
+15 Max Stamina
-10 Max Mana
+3 Strength
+2 Endurance
-2 Intelligence
+10 Carry Weight
+10% Melee Damage
+10% Armor Effectiveness
```

### 40.2 Mage

Recommended Mage bonuses:

```text
-15 Max Health
+40 Max Mana
-2 Strength
-1 Endurance
+4 Intelligence
+1 Faith
-5 Carry Weight
+15% Spell Damage
+10% Mana Regeneration
-10% Heavy Armor Efficiency
```

### 40.3 Paladin

Recommended Paladin bonuses:

```text
+10 Max Health
+5 Max Stamina
+15 Max Mana
+1 Strength
+2 Endurance
+3 Faith
-1 Agility
+5 Carry Weight
+15% Healing Effectiveness
+10% Defense
+10% Holy Damage
```

---

## 41. Recommended Backstory Bonus Implementation

### 41.1 Soldier

Recommended Soldier bonuses:

```text
+10 Max Stamina
+1 Strength
+5% Weapon Handling
+5% Melee Damage
```

### 41.2 Student

Recommended Student bonuses:

```text
+1 Intelligence
+5% XP Gain
+1 Bonus Upgrade Point every 5 levels
Small bonus to lore/knowledge checks
```

### 41.3 Peasant

Recommended Peasant bonuses:

```text
+5 Max Health
+1 Endurance
+10 Carry Weight
+5% Better Sell Prices
+10% Gathering Yield
+5% Food/Healing Item Effectiveness
```

---

## 42. Stacking Rules

### 42.1 Recommended Stacking Categories

Bonuses should not all stack freely. Use clear stacking rules.

Recommended categories:

```text
Additive Percentage
Multiplicative Percentage
Flat Additive
Highest Value Only
Unique Effect
Conditional Trigger
Capped Total
```

### 42.2 Example Stacking Rules

Melee damage bonuses can be additive:

```text
+10% Warrior Melee Damage
+5% Soldier Melee Damage
= +15% Total Melee Damage
```

Damage reduction should usually be multiplicative or capped:

```text
10% physical resistance and 10% armor reduction should not always become 20% total reduction.
```

Cooldown reduction should be capped:

```text
Maximum cooldown reduction: 40%
```

Sell price bonuses should be capped:

```text
Base sell price: 40%
Peasant sell price: 45%
Possible maximum through perks/reputation: 60%
```

---

## 43. Suggested Bonus Caps

Recommended caps prevent broken scaling.

| Bonus Type | Suggested Cap |
|---|---:|
| Global Damage Bonus | +50% to +75% |
| Specific Damage Bonus | +100% |
| Cooldown Reduction | 40% |
| Mana Cost Reduction | 50% |
| Stamina Cost Reduction | 50% |
| Movement Speed Bonus | 20% |
| Dodge Cost Reduction | 40% |
| Block Efficiency | 60% |
| Sell Price Multiplier | 60% of item value |
| Buy Price Discount | 30% |
| XP Gain | 25% |
| Rare Loot Chance | 25% |
| Durability Loss Reduction | 50% |
| Repair Cost Reduction | 50% |

These caps can be adjusted after playtesting.

---

## 44. Example Bonus Data Records

### 44.1 Class Bonus Example

```json
{
  "bonusId": "class_warrior_melee_damage",
  "displayName": "Warrior Melee Training",
  "affectedStat": "MeleeDamage",
  "operationType": "PercentAdd",
  "value": 0.10,
  "sourceType": "Class",
  "durationType": "Permanent",
  "stackingRule": "Additive",
  "condition": null,
  "tags": ["Warrior", "Melee", "Class"]
}
```

### 44.2 Backstory Bonus Example

```json
{
  "bonusId": "backstory_peasant_sell_price",
  "displayName": "Practical Barter",
  "affectedStat": "SellPriceMultiplier",
  "operationType": "FlatAdd",
  "value": 0.05,
  "sourceType": "Backstory",
  "durationType": "Permanent",
  "stackingRule": "Capped",
  "condition": null,
  "tags": ["Peasant", "Economy", "Backstory"]
}
```

### 44.3 Equipment Bonus Example

```json
{
  "bonusId": "ring_minor_mana_regen",
  "displayName": "Minor Mana Flow",
  "affectedStat": "ManaRegeneration",
  "operationType": "PercentAdd",
  "value": 0.10,
  "sourceType": "Equipment",
  "durationType": "WhileEquipped",
  "stackingRule": "Additive",
  "condition": null,
  "tags": ["Mage", "Mana", "Ring"]
}
```

### 44.4 Conditional Bonus Example

```json
{
  "bonusId": "skill_last_breath_stamina",
  "displayName": "Last Breath",
  "affectedStat": "StaminaRegeneration",
  "operationType": "PercentAdd",
  "value": 0.20,
  "sourceType": "Skill",
  "durationType": "Conditional",
  "stackingRule": "Additive",
  "condition": "HealthPercent <= 0.25",
  "tags": ["Survival", "Stamina", "LowHealth"]
}
```

---

## 45. Implementation Notes

### 45.1 Use Stable IDs

Every bonus should have a stable ID. This is essential for save/load, debugging, balancing, and content editing.

### 45.2 Separate Definitions from Runtime State

Static bonus definitions should be separate from active runtime bonus instances.

Example:

```text
Bonus Definition:
What the bonus is.

Active Bonus Instance:
Who has the bonus, where it came from, when it expires, and whether its condition is currently true.
```

### 45.3 Recalculate Derived Stats When Needed

The stat system should recalculate derived stats when:

- Equipment changes
- Skill is unlocked
- Buff is applied or removed
- Class/backstory is selected
- Level increases
- Temporary effects expire
- Region-specific effects activate/deactivate

### 45.4 Display Bonuses Clearly

The UI should show:

- Source of bonus
- Amount
- Duration
- Whether it is active
- Whether it is conditional
- Whether it is capped

Example tooltip:

```text
Melee Damage: +15%
Sources:
+10% Warrior Class
+5% Soldier Backstory
```

---

## 46. Conclusion

This bonus system should be treated as a modular RPG modifier framework. The same underlying system should support class bonuses, backstory bonuses, gear effects, skill upgrades, temporary buffs, faction rewards, region effects, and quest rewards.

For the first playable version, the game should use a small and readable MVP bonus set focused on:

- Core stats
- Combat damage
- Defense
- Stamina/mana management
- Healing
- Inventory/carry weight
- Durability/repair
- XP/progression
- Economy
- Gathering

Once Greyreach Valley is playable and balanced, additional bonus categories can be introduced region by region.

The guiding rule should be:

> **Every bonus should either strengthen a clear playstyle, support a meaningful RPG choice, or make the world feel more reactive.**
