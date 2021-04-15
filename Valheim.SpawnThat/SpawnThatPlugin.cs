﻿using BepInEx;
using HarmonyLib;
using Valheim.SpawnThat.Configuration;
using Valheim.SpawnThat.Core;

namespace Valheim.SpawnThat
{
    [BepInPlugin("asharppen.valheim.spawn_that", "Spawn That!", "0.5.1")]
    public class SpawnThatPlugin : BaseUnityPlugin
    {        
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Log.Logger = Logger;

            ConfigurationManager.GeneralConfig = ConfigurationManager.LoadGeneral();

            new Harmony("mod.spawn_that").PatchAll();
        }
    }
}
