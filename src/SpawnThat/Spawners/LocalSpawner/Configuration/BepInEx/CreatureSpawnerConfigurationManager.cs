﻿using BepInEx;
using BepInEx.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using SpawnThat.Configuration;
using SpawnThat.Core;
using SpawnThat.Core.Configuration;
using System.Linq;
using SpawnThat.Core.Toml;

namespace SpawnThat.Spawners.LocalSpawner.Configuration.BepInEx;

internal static class CreatureSpawnerConfigurationManager
{
    public static CreatureSpawnerConfigurationFile CreatureSpawnerConfig;

    internal const string CreatureSpawnerConfigFile = "spawn_that.local_spawners_advanced.cfg";

    internal const string CreatureSpawnerSupplemental = "spawn_that.local_spawners.*";

    public static void LoadAllConfigurations()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        CreatureSpawnerConfig = LoadCreatureSpawnerConfiguration();

        stopwatch.Stop();

        Log.LogDebug("Config loading took: " + stopwatch.Elapsed);
    }

    public static CreatureSpawnerConfigurationFile LoadCreatureSpawnerConfiguration()
    {
        Log.LogInfo($"Loading local spawner configurations.");

        string configPath = Path.Combine(Paths.ConfigPath, CreatureSpawnerConfigFile);

        if (!File.Exists(configPath))
        {
            File.Create(configPath).Close();
        }

        var configs = LoadCreatureSpawnConfig(configPath);

        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, CreatureSpawnerSupplemental, SearchOption.AllDirectories);
        Log.LogDebug($"Found {supplementalFiles.Length} supplemental local spawner config files");

        foreach (var file in supplementalFiles)
        {
            try
            {
                var supplementalConfig = LoadCreatureSpawnConfig(file);

                supplementalConfig.MergeInto(configs);
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to load supplemental config '{file}'.", e);
            }
        }

        Log.LogDebug("Finished loading local spawner configurations");

        return configs;
    }

    private static CreatureSpawnerConfigurationFile LoadCreatureSpawnConfig(string configPath)
    {
        Log.LogDebug($"Loading local spawner configurations from {configPath}.");

        return TomlLoader.LoadFile<CreatureSpawnerConfigurationFile>(configPath);
    }
}
