﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SpawnThat.Core;
using SpawnThat.Spawners.WorldSpawner.Configurations.BepInEx;
using SpawnThat.Utilities.Extensions;
using SpawnThat.Debugging;
using SpawnThat.Spawners.WorldSpawner.Managers;
using SpawnThat.Options.Conditions;
using SpawnThat.Options.Modifiers;
using SpawnThat.Integrations.CLLC.Conditions;
using SpawnThat.Integrations.CLLC.Modifiers;
using SpawnThat.Integrations.EpicLoot.Conditions;
using SpawnThat.Integrations.MobAi.Modifiers;
using SpawnThat.Options.PositionConditions;

namespace SpawnThat.Spawners.WorldSpawner.Debug;

internal static class SpawnDataFileGenerator
{
    public static void WriteToFile(List<SpawnSystem.SpawnData> spawners, string fileName, bool postChange = false)
    {
        try
        {
            if (spawners is null)
            {
                return;
            }

            List<string> lines = new List<string>(spawners.Count * 30);

            AddIntroToFile(lines);

            for (int i = 0; i < spawners.Count; ++i)
            {
                var spawner = spawners[i];

                if (spawner is not null)
                {
                    lines.AddRange(WriteSpawner(spawner, i, postChange));
                }
                else
                {
                    //Empty spawner. Just add the index and continue.
                    lines.Add($"[WorldSpawner.{i}]");
                    lines.Add($"## Spawner is empty for unknown reasons.");
                    lines.Add($"");
                }
            }

            DebugFileWriter.WriteFile(lines, fileName, "world spawner configurations");
        }
        catch (Exception e)
        {
            Log.LogWarning($"Error while trying to write world spawner debug file '{fileName}'.", e);
        }
    }

    internal static void AddIntroToFile(List<string> lines)
    {
        lines.Add($"# This file was auto-generated by Spawn That {SpawnThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.");
        lines.Add($"# The spawn entries listed here are extracted at runtime.");
        lines.Add($"# The file is intended for investigation, while showing entries in the default Spawn That config format.");
        lines.Add($"# Be aware, for entries added by mods using the Spawn That API, it might not be showing the full truth but an approximation, since the API can do a lot more than the config file allows.");
        lines.Add($"");
    }

    internal static List<string> WriteSpawner(SpawnSystem.SpawnData spawner, int index, bool postChange)
    {
        List<string> lines = new List<string>();

        // Try get template.
        var template = WorldSpawnerManager.GetTemplate(spawner);

        //Write header
        if (template is not null)
        {
            lines.Add($"[WorldSpawner.{template.Index}]");
        }
        else
        {
            lines.Add($"[WorldSpawner.{index}]");
        }

        string environmentArray = "";
        try
        {
            if ((spawner.m_requiredEnvironments?.Count ?? 0) > 0)
            {
                environmentArray = spawner.m_requiredEnvironments.Join();
            }
        }
        catch (Exception e)
        {
            Log.LogWarning($"Error while attempting to read required environments of spawner {spawner}", e);
        }

        //Write lines
        lines.Add($"{nameof(SpawnConfiguration.Name)}={spawner.m_name}");
        lines.Add($"{nameof(SpawnConfiguration.Enabled)}={spawner.m_enabled}");
        try
        {
            lines.Add($"{nameof(SpawnConfiguration.Biomes)}={BiomeArray(spawner.m_biome)}");
        }
        catch (Exception e)
        {
            Log.LogWarning($"Failed to read biome of {spawner}", e);
        }

        try
        {
            lines.Add($"{nameof(SpawnConfiguration.PrefabName)}={spawner.m_prefab.GetCleanedName()}");
        }
        catch (Exception e)
        {
            Log.LogWarning($"Error while attempting to read name of prefab for world spawn {spawner?.m_name}, to print to debug file.", e);
        }

        lines.Add($"{nameof(SpawnConfiguration.HuntPlayer)}={spawner.m_huntPlayer}");
        lines.Add($"{nameof(SpawnConfiguration.MaxSpawned)}={spawner.m_maxSpawned}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnInterval)}={spawner.m_spawnInterval.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnChance)}={spawner.m_spawnChance.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.LevelMin)}={spawner.m_minLevel}");
        lines.Add($"{nameof(SpawnConfiguration.LevelMax)}={spawner.m_maxLevel}");
        lines.Add($"{nameof(SpawnConfiguration.LevelUpChance)}={spawner.m_overrideLevelupChance.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.LevelUpMinCenterDistance)}={spawner.m_levelUpMinCenterDistance.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnDistance)}={spawner.m_spawnDistance.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnRadiusMin)}={spawner.m_spawnRadiusMin.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnRadiusMax)}={spawner.m_spawnRadiusMax.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.RequiredGlobalKey)}={spawner.m_requiredGlobalKey}");
        lines.Add($"{nameof(SpawnConfiguration.RequiredEnvironments)}={environmentArray}");
        lines.Add($"{nameof(SpawnConfiguration.GroupSizeMin)}={spawner.m_groupSizeMin}");
        lines.Add($"{nameof(SpawnConfiguration.GroupSizeMax)}={spawner.m_groupSizeMax}");
        lines.Add($"{nameof(SpawnConfiguration.GroupRadius)}={spawner.m_groupRadius.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.GroundOffset)}={spawner.m_groundOffset.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnDuringDay)}={spawner.m_spawnAtDay}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnDuringNight)}={spawner.m_spawnAtNight}");
        lines.Add($"{nameof(SpawnConfiguration.ConditionAltitudeMin)}={spawner.m_minAltitude.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.ConditionAltitudeMax)}={spawner.m_maxAltitude.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.ConditionTiltMin)}={spawner.m_minTilt.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.ConditionTiltMax)}={spawner.m_maxTilt.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnInForest)}={spawner.m_inForest}");
        lines.Add($"{nameof(SpawnConfiguration.SpawnOutsideForest)}={spawner.m_outsideForest}");
        lines.Add($"{nameof(SpawnConfiguration.OceanDepthMin)}={spawner.m_minOceanDepth.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.OceanDepthMax)}={spawner.m_maxOceanDepth.ToString(CultureInfo.InvariantCulture)}");
        lines.Add($"{nameof(SpawnConfiguration.BiomeArea)}={spawner.m_biomeArea}");

        try
        {
            if (template is null)
            {
                Character character = spawner.m_prefab.IsNull()
                    ? null
                    : spawner.m_prefab.GetComponent<Character>();

                string factionName = "";

                if (character.IsNotNull())
                {
                    factionName = character.m_faction.ToString();
                }

                lines.Add($"{nameof(SpawnConfiguration.SetFaction)}={factionName}");
            }
        }
        catch (Exception e)
        {
            Log.LogWarning($"Error while attempting to write faction of spawner {spawner}", e);
        }

        HashSet<Type> AddedOptions = new();

        if (template is not null)
        {
            // Add custom settings.
            foreach (var condition in template.SpawnConditions)
            {
                switch (condition)
                {
                    case ConditionDistanceToCenter con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            if ((con.MinDistance ?? 0) != 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionDistanceToCenterMin)}={(con.MinDistance ?? 0).ToString(CultureInfo.InvariantCulture)}");
                            }
                            if ((con.MaxDistance ?? 0) != 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionDistanceToCenterMax)}={(con.MaxDistance ?? 0).ToString(CultureInfo.InvariantCulture)}");
                            }
                        }
                        break;
                    case ConditionWorldAge con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            if ((con.MinDays ?? 0) != 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionWorldAgeDaysMin)}={con.MinDays ?? 0}");
                            }
                            if ((con.MaxDays ?? 0) != 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionWorldAgeDaysMax)}={con.MaxDays ?? 0}");
                            }
                        }
                        break;
                    case ConditionNearbyPlayersCarryValue con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionNearbyPlayersCarryValue)}={con.RequiredValue}");
                        }
                        break;
                    case ConditionNearbyPlayersCarryItem con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionNearbyPlayerCarriesItem)}={con.ItemsSearchedFor.Join()}");
                        }
                        break;
                    case ConditionNearbyPlayersNoise con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionNearbyPlayersNoiseThreshold)}={con.NoiseThreshold.ToString(CultureInfo.InvariantCulture)}");
                        }
                        break;
                    case ConditionNearbyPlayersStatus con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionNearbyPlayersStatus)}={con.RequiredStatusEffects.Join()}");
                        }
                        break;
                    case ConditionGlobalKeysRequiredMissing con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.RequiredNotGlobalKey)}={con.RequiredMissing.Join()}");
                        }
                        break;
                    case ConditionLocation con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionLocation)}={con.Locations.Join()}");
                        }
                        break;
                    case ConditionAreaSpawnChance con:
                        // Can't select entity ids in cfg files, so only output if it matches the template id to properly simulate what is possible.
                        if (con.EntityId == template.Index)
                        {
                            if (con.AreaChance != 100)
                            {
                                if (AddedOptions.Add(con.GetType()))
                                {
                                    lines.Add($"{nameof(SpawnConfiguration.ConditionAreaSpawnChance)}={con.AreaChance.ToString(CultureInfo.InvariantCulture)}");
                                }
                            }
                        }
                        break;
                    case ConditionAreaIds con:
                        if (AddedOptions.Add(con.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.ConditionAreaIds)}={con.RequiredAreaIds.Join()}");
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var modifier in template.SpawnModifiers)
            {
                switch (modifier)
                {
                    case ModifierSetFaction mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            lines.Add($"{nameof(SpawnConfiguration.SetFaction)}={mod.Faction?.ToString() ?? ""}");
                        }
                        break;
                    case ModifierSetRelentless mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            if (mod.Relentless)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.SetRelentless)}={mod.Relentless}");
                            }
                        }
                        break;
                    case ModifierDespawnOnConditionsInvalid mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            // Can't truly reflect the internal settings, so just treat it as set if the type is present.
                            lines.Add($"{nameof(SpawnConfiguration.SetTryDespawnOnConditionsInvalid)}=true");
                        }
                        break;
                    case ModifierDespawnOnAlert mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            if (mod.DespawnOnAlert)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.SetTryDespawnOnAlert)}={mod.DespawnOnAlert}");
                            }
                        }
                        break;
                    case ModifierSetTamed mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            if (mod.Tamed)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.SetTamed)}={mod.Tamed}");
                            }
                        }
                        break;
                    case ModifierSetTamedCommandable mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            if (mod.Commandable)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.SetTamedCommandable)}={mod.Commandable}");
                            }
                        }
                        break;
                    case ModifierSetTemplateId mod:
                        if (AddedOptions.Add(mod.GetType()))
                        {
                            if (!string.IsNullOrWhiteSpace(mod.TemplateId))
                            {
                                lines.Add($"{nameof(SpawnConfiguration.TemplateId)}={mod.TemplateId}");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var positionCondition in template.SpawnPositionConditions)
            {
                switch (positionCondition)
                {
                    case PositionConditionMustBeNearAllPrefabs cond:
                        if (AddedOptions.Add(cond.GetType()))
                        {
                            if (cond.Prefabs.Count > 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustBeNearAllPrefabs)}={cond.Prefabs.Join()}");
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustBeNearAllPrefabsDistance)}={cond.Distance}");
                            }
                        }
                        break;
                    case PositionConditionMustBeNearPrefabs cond:
                        if (AddedOptions.Add(cond.GetType()))
                        {
                            if (cond.Prefabs.Count > 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustBeNearPrefabs)}={cond.Prefabs.Join()}");
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustBeNearPrefabsDistance)}={cond.Distance}");
                            }
                        }
                        break;
                    case PositionConditionMustNotBeNearPrefabs cond:
                        if (AddedOptions.Add(cond.GetType()))
                        {
                            if (cond.Prefabs.Count > 0)
                            {
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustNotBeNearPrefabs)}={cond.Prefabs.Join()}");
                                lines.Add($"{nameof(SpawnConfiguration.ConditionPositionMustNotBeNearPrefabsDistance)}={cond.Distance}");
                            }
                        }
                        break;
                }
            }

            if (Integrations.IntegrationManager.InstalledCLLC)
            {
                AddCllcIntegrationLines(lines, template);
            }

            if (Integrations.IntegrationManager.InstalledEpicLoot)
            {
                AddEpicLootLines(lines, template);
            }

            if (Integrations.IntegrationManager.InstalledMobAI)
            {
                AddMobAiLines(lines, template);
            }
        }

        lines.Add("");

        return lines;
    }

    private static void AddCllcIntegrationLines(List<string> lines, WorldSpawnTemplate template)
    {
        bool addedHeader = false;

        HashSet<Type> AddedOptions = new();

        foreach (var condition in template.SpawnModifiers)
        {
            switch (condition)
            {
                case ConditionWorldLevel con:
                    if (AddedOptions.Add(con.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.ConditionWorldLevelMin)}={con.MinWorldLevel}");
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.ConditionWorldLevelMax)}={con.MaxWorldLevel}");
                    }
                    break;
                default:
                    break;
            }
        }

        foreach (var modifier in template.SpawnModifiers)
        {
            switch (modifier)
            {
                case ModifierCllcInfusion mod:
                    if (AddedOptions.Add(mod.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.SetInfusion)}={mod.Infusion}");
                    }
                    break;
                case ModifierCllcBossAffix mod:
                    if (AddedOptions.Add(mod.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.SetBossAffix)}={mod.Affix}");
                    }
                    break;
                case ModifierCllcExtraEffect mod:
                    if (AddedOptions.Add(mod.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.SetExtraEffect)}={mod.ExtraEffect}");
                    }
                    break;
                case ModifierDefaultRollLevel mod:
                    // Not entirely correct, since this is not necessarily bound to CLLC, but its the closest to old cfg.
                    if (AddedOptions.Add(mod.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigCLLC.UseDefaultLevels)}=true");
                    }
                    break;
                default:
                    break;
            }
        }

        if (addedHeader)
        {
            lines.Add("");
        }

        void InsertHeaderIfMissing()
        {
            if (!addedHeader)
            {
                addedHeader = true;
                lines.Add("");
                lines.Add($"[WorldSpawner.{template.Index}.{SpawnSystemConfigCLLC.ModName}]");
            }
        }
    }

    private static void AddEpicLootLines(List<string> lines, WorldSpawnTemplate template)
    {
        bool addedHeader = false;

        HashSet<Type> AddedOptions = new();

        foreach (var condition in template.SpawnConditions)
        {
            switch (condition)
            {
                case ConditionNearbyPlayerCarryItemWithRarity con:
                    if (AddedOptions.Add(con.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigEpicLoot.ConditionNearbyPlayerCarryItemWithRarity)}={con.RaritiesRequired.Join()}");
                    }
                    break;
                case ConditionNearbyPlayerCarryLegendaryItem con:
                    if (AddedOptions.Add(con.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigEpicLoot.ConditionNearbyPlayerCarryLegendaryItem)}={con.LegendaryIds.Join()}");
                    }
                    break;
            }
        }

        void InsertHeaderIfMissing()
        {
            if (!addedHeader)
            {
                addedHeader = true;
                lines.Add("");
                lines.Add($"[WorldSpawner.{template.Index}.{SpawnSystemConfigEpicLoot.ModName}]");
            }
        }
    }

    private static void AddMobAiLines(List<string> lines, WorldSpawnTemplate template)
    {
        bool addedHeader = false;

        HashSet<Type> AddedOptions = new();

        foreach (var modifier in template.SpawnModifiers)
        {
            switch (modifier)
            {
                case ModifierSetAI mod:
                    if (AddedOptions.Add(mod.GetType()))
                    {
                        InsertHeaderIfMissing();
                        lines.Add($"{nameof(SpawnSystemConfigMobAI.SetAI)}={mod.AiName}");
                    }
                    break;
                default:
                    break;
            }
        }

        if (addedHeader)
        {
            lines.Add("");
        }

        void InsertHeaderIfMissing()
        {
            if (!addedHeader)
            {
                addedHeader = true;
                lines.Add("");
                lines.Add($"[WorldSpawner.{template.Index}.{SpawnSystemConfigMobAI.ModName}]");
            }
        }
    }

    private static string BiomeArray(Heightmap.Biome spawnerBiome) =>
        spawnerBiome.Split().Join();
}
