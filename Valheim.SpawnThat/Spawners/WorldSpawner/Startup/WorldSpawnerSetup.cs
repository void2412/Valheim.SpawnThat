﻿using System.Collections;
using UnityEngine;
using Valheim.SpawnThat.Lifecycle;
using Valheim.SpawnThat.Spawners.WorldSpawner.Configurations.BepInEx;
using Valheim.SpawnThat.Spawners.WorldSpawner.Sync;

namespace Valheim.SpawnThat.Spawners.WorldSpawner.Startup;

internal static class WorldSpawnerSetup
{
    public static void SetupWorldSpawners()
    {
        LifecycleManager.OnSinglePlayerInit += LoadBepInExConfigs;
        LifecycleManager.OnDedicatedServerInit += LoadBepInExConfigs;
        LifecycleManager.OnFindSpawnPointFirstTime += DelayedConfigRelease;

        // TODO: This SHOULD be on late configure, but configs doesn't have a concept of null yet, so to avoid overriding everything always, they will be applied first.
        SpawnerConfigurationManager.OnEarlyConfigure += ApplyBepInExConfigs;
        //SpawnerConfigurationManager.SubscribeConfiguration(ApplyBepInExConfigs);

        WorldSpawnerSyncSetup.Configure();
    }

    internal static void LoadBepInExConfigs()
    {
        SpawnSystemConfigurationManager.LoadAllConfigurations();
        WorldSpawnerManager.WaitingForConfigs = false;
    }

    internal static void ApplyBepInExConfigs(ISpawnerConfigurationCollection spawnerConfigs)
    {
        SpawnSystemConfigApplier.ApplyBepInExConfigs(spawnerConfigs);
    }

    internal static void DelayedConfigRelease()
    {
        _ = Game.instance.StartCoroutine(StopWaiting());
    }

    public static IEnumerator StopWaiting()
    {
        yield return new WaitForSeconds(8);

        WorldSpawnerManager.WaitingForConfigs = false;
    }
}
