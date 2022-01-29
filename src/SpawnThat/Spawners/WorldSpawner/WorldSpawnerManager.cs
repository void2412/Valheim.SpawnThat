﻿using System;
using System.Collections.Generic;
using UnityEngine;
using SpawnThat.Core;
using SpawnThat.Lifecycle;
using SpawnThat.Spawners.WorldSpawner.Services;

namespace SpawnThat.Spawners.WorldSpawner;

internal static class WorldSpawnerManager
{
    private static bool HasInstantiatedSpawnLists;
    private static List<GameObject> SpawnListsObjects { get; set; } = new();
    private static List<SpawnSystemList> SpawnLists { get; set; } = new();

    private static Dictionary<SpawnSystem.SpawnData, WorldSpawnTemplate> TemplatesBySpawnEntry { get; set; } = new();

    private static List<SpawnSystemList> PrefabSpawnSystemLists { get; set; } = new(0);

    /// <summary>
    /// Disables world spawner updates while true.
    /// Indended to delay updates until configs have been loaded.
    /// </summary>
    public static bool WaitingForConfigs { get; set; } = true;

    static WorldSpawnerManager()
    {
        LifecycleManager.SubscribeToWorldInit(() =>
        {
            WaitingForConfigs = true;

            HasInstantiatedSpawnLists = false;
            SpawnListsObjects.Clear();
            SpawnLists.Clear();

            TemplatesBySpawnEntry = new();
        });
    }

    public static void SetPrefabSpawnSystemLists()
    {
        if (PrefabSpawnSystemLists.Count == 0)
        {
            var prefabSpawnSystem = ZoneSystem.instance.m_zoneCtrlPrefab.GetComponent<SpawnSystem>();

            PrefabSpawnSystemLists = prefabSpawnSystem.m_spawnLists;
        }
    }

    public static void EnsureInstantiatedSpawnListAssigned(SpawnSystem spawner)
    {
        if (HasInstantiatedSpawnLists)
        {
            spawner.m_spawnLists = SpawnLists;
            return;
        }

        GameObject testForPrefabGo = new GameObject();

        try
        {
            foreach (var spawnList in PrefabSpawnSystemLists)
            {
                Log.LogTrace($"Instantiating spawn list: '{spawnList.name}'");
                var instantiatedSpawnList = GameObject.Instantiate(spawnList.gameObject);

                SpawnListsObjects.Add(instantiatedSpawnList);
                SpawnLists.Add(instantiatedSpawnList.GetComponent<SpawnSystemList>());
            }
        }
        catch (Exception e)
        {
            Log.LogWarning("Unable to instantiate SpawnSystemLists. Skipping step. Avoid entering multiple worlds without restarting, then everything will still be fine", e);
            HasInstantiatedSpawnLists = true;
            return;
        }

        spawner.m_spawnLists = SpawnLists;
        HasInstantiatedSpawnLists = true;
    }

    public static bool ShouldDelaySpawnerUpdate(SpawnSystem spawner)
    {
        if (WaitingForConfigs)
        {
            Log.LogTrace("SpawnSysten waiting for configs. Skipping update.");
            return true;
        }

        return false;
    }

    public static void ConfigureSpawnList(SpawnSystem spawner)
    {
        if (WaitingForConfigs)
        {
            return;
        }

        WorldSpawnerConfigurationService.ConfigureSpawnLists(spawner.m_spawnLists);
    }

    public static void SetTemplate(SpawnSystem.SpawnData spawnEntry, WorldSpawnTemplate template)
    {
        TemplatesBySpawnEntry[spawnEntry] = template;
    }

    public static WorldSpawnTemplate GetTemplate(SpawnSystem.SpawnData spawnEntry)
    {
        if (TemplatesBySpawnEntry.TryGetValue(spawnEntry, out WorldSpawnTemplate template))
        {
            return template;
        }

        return null;
    }
}
