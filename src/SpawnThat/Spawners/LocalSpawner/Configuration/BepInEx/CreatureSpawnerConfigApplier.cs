﻿using System;
using SpawnThat.Core;
using SpawnThat.Core.Configuration;
using SpawnThat.Integrations;
using SpawnThat.Options.Modifiers;
using SpawnThat.Utilities;

namespace SpawnThat.Spawners.LocalSpawner.Configuration.BepInEx;

internal static class CreatureSpawnerConfigApplier
{
    internal static void ApplyBepInExConfigs(ISpawnerConfigurationCollection spawnerConfigs)
    {
        var configs = CreatureSpawnerConfigurationManager.CreatureSpawnerConfig?.Subsections;

        if ((configs?.Count ?? 0) == 0)
        {
            return;
        }

        foreach (var locationConfig in configs)
        {
            foreach (var creatureConfig in locationConfig.Value.Subsections)
            {
                if (!creatureConfig.Value.TemplateEnabled.Value)
                {
                    continue;
                }

                // BepInEx configs are not set up to distinguish between rooms and locations in config path.
                // Instead, the first matching name is used. Therefore, two builders are configured to leave
                // it up to the first and most specific identified to be used.
                var roomBuilder = spawnerConfigs.ConfigureLocalSpawnerByRoomAndCreature(locationConfig.Key, creatureConfig.Key);
                var locationBuilder = spawnerConfigs.ConfigureLocalSpawnerByLocationAndCreature(locationConfig.Key, creatureConfig.Key);

                ApplyConfigToBuilder(creatureConfig.Value, roomBuilder);
                ApplyConfigToBuilder(creatureConfig.Value, locationBuilder);
            }
        }
    }

    private static void ApplyConfigToBuilder(CreatureSpawnerConfig config, ILocalSpawnBuilder builder)
    {
        // Default
        if (config.PrefabName.Value.IsNotEmpty())
        {
            builder.SetPrefabName(config.PrefabName.Value);
        }

        builder.SetEnabled(config.Enabled.Value);
        builder.SetMaxLevel(config.LevelMax.Value);
        builder.SetMinLevel(config.LevelMin.Value);
        builder.SetLevelUpChance(config.LevelUpChance.Value);
        builder.SetSpawnInterval(TimeSpan.FromMinutes(config.RespawnTime.Value));
        builder.SetPatrolSpawn(config.SetPatrolPoint.Value);
        builder.SetSpawnDuringNight(config.SpawnAtDay.Value);
        builder.SetSpawnDuringDay(config.SpawnAtNight.Value);
        builder.SetConditionPlayerWithinDistance(config.TriggerDistance.Value);
        builder.SetConditionPlayerNoise(config.TriggerNoise.Value);
        builder.SetSpawnInPlayerBase(config.SpawnInPlayerBase.Value);

        // Modifiers
        if (config.SetFaction.Value.IsNotEmpty())
        {
            builder.SetModifier(new ModifierSetFaction(config.SetFaction.Value));
        }

        builder.SetModifierTamed(config.SetTamed.Value);
        builder.SetModifierTamedCommandable(config.SetTamedCommandable.Value);

        // Modifiers - Integrations
        Config cfg;

        {
            if (IntegrationManager.InstalledCLLC)
            {
                if (config.TryGet(CreatureSpawnerConfigCLLC.ModName, out cfg) &&
                    cfg is CreatureSpawnerConfigCLLC cllcConfig)
                {
                    if (cllcConfig.SetBossAffix.Value.IsNotEmpty())
                    {
                        builder.SetCllcModifierBossAffix(cllcConfig.SetBossAffix.Value);
                    }
                    if (cllcConfig.SetExtraEffect.Value.IsNotEmpty())
                    {
                        builder.SetCllcModifierExtraEffect(cllcConfig.SetExtraEffect.Value);
                    }
                    if (cllcConfig.SetInfusion.Value.IsNotEmpty())
                    {
                        builder.SetCllcModifierInfusion(cllcConfig.SetInfusion.Value);
                    }

                    if (cllcConfig.UseDefaultLevels.Value)
                    {
                        builder.SetModifier(new ModifierDefaultRollLevel(config.LevelMin.Value, config.LevelMax.Value, 0, config.LevelUpChance.Value));
                    }
                }
            }

            if (IntegrationManager.InstalledMobAI)
            {
                if (config.TryGet(CreatureSpawnerConfigMobAI.ModName, out cfg) &&
                    cfg is CreatureSpawnerConfigMobAI mobAIConfig)
                {
                    if (mobAIConfig.SetAI.Value.IsNotEmpty())
                    {
                        builder.SetMobAiModifier(mobAIConfig.SetAI.Value, mobAIConfig.AIConfigFile.Value);
                    }
                }
            }
        }
    }
}