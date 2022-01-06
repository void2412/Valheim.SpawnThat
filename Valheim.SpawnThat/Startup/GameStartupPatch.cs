﻿using HarmonyLib;
using System.Collections;
using UnityEngine;
using Valheim.SpawnThat.Core;
using Valheim.SpawnThat.Spawners.LocalSpawner;
using Valheim.SpawnThat.Spawners.WorldSpawner;

namespace Valheim.SpawnThat.Startup;

[HarmonyPatch(typeof(Game))]
public static class GameStartupPatch
{
    private static bool FirstTime = true;

    static GameStartupPatch()
    {
        StateResetter.Subscribe(() =>
        {
            FirstTime = true;
        });
    }

    [HarmonyPatch(nameof(Game.FindSpawnPoint))]
    [HarmonyPostfix]
    private static void LoadConfigs(Game __instance)
    {
        if (FirstTime)
        {
            FirstTime = false;
            _ = __instance.StartCoroutine(ReleaseConfigs());
        }
    }

    public static IEnumerator ReleaseConfigs()
    {
        Log.LogDebug("Starting early delay for config application.");

        yield return new WaitForSeconds(2);

        Log.LogDebug("Finished early delay for config application.");
        WorldSpawnerManager.WaitingForConfigs = false;
        LocalSpawnerManager.WaitingForConfigs = false;
    }
}
