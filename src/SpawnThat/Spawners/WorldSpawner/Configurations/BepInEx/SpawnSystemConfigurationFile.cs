﻿using SpawnThat.Core.Configuration;

namespace SpawnThat.Spawners.WorldSpawner.Configurations.BepInEx;

internal class SpawnSystemConfigurationFile : ConfigWithSubsections<SpawnSystemConfiguration>, IConfigFile
{
    protected override SpawnSystemConfiguration InstantiateSubsection(string subsectionName)
    {
        return new SpawnSystemConfiguration();
    }
}

internal class SpawnSystemConfiguration : ConfigWithSubsections<SpawnConfiguration>
{
    protected override SpawnConfiguration InstantiateSubsection(string subsectionName)
    {
        return new SpawnConfiguration();
    }
}

internal class SpawnConfiguration : ConfigWithSubsections<Config>
{
    protected override Config InstantiateSubsection(string subsectionName)
    {
        Config newModConfig = null;

        if (subsectionName == SpawnSystemConfigCLLC.ModName.Trim().ToUpperInvariant())
        {
            newModConfig = new SpawnSystemConfigCLLC();
        }
        else if (subsectionName == SpawnSystemConfigMobAI.ModName.Trim().ToUpperInvariant())
        {
            newModConfig = new SpawnSystemConfigMobAI();
        }
        else if (subsectionName == SpawnSystemConfigEpicLoot.ModName.Trim().ToUpperInvariant())
        {
            newModConfig = new SpawnSystemConfigEpicLoot();
        }

        return newModConfig;
    }

    private int? index = null;

    public int Index
    {
        get
        {
            if (index.HasValue)
            {
                return index.Value;
            }

            if (int.TryParse(SectionName, out int sectionIndex) && sectionIndex >= 0)
            {
                index = sectionIndex;
            }
            else
            {
                index = int.MaxValue;
            }

            return index.Value;
        }
    }

    public ConfigurationEntry<string> TemplateId = new("", "Technical setting intended for cross-mod identification of mobs spawned by this template. Sets a custom identifier which will be assigned to the spawned mobs ZDO as 'ZDO.Set(\"spawn_template_id\", TemplateIdentifier)'.");

    public ConfigurationEntry<string> Name = new("My spawner", "Just a field for naming the configuration entry.");

    public ConfigurationEntry<bool> Enabled = new(true, "Enable/disable this spawner entry.");

    public ConfigurationEntry<bool> TemplateEnabled = new(true, "Enable/disable this configuration. This does not disable existing entries, just this configuration itself.");

    public ConfigurationEntry<string> Biomes = new("", "Biomes in which entity can spawn. Leave empty for all.");

    //public ConfigurationEntry<bool> DriveInward = new ConfigurationEntry<bool>(false, "Mobs always spawn towards the world edge from player.");

    //Bound to the spawner itself. Need to transpile in a change for this to work.
    //public ConfigurationEntry<float> LevelUpChance = new ConfigurationEntry<float>(10, "Chance to increase level above min. This is run multiple times. 100 is 100%.\nEg. if Chance is 10, LevelMin is 1 and LevelMax is 3, the game will have a 10% to become level 2. The game will then run an additional 10% check for increasing to level 3.");

    public ConfigurationEntry<float> ConditionDistanceToCenterMin = new(0, "Minimum distance to center for configuration to apply.");

    public ConfigurationEntry<float> ConditionDistanceToCenterMax = new(0, "Maximum distance to center for configuration to apply. 0 means limitless.");

    public ConfigurationEntry<float> ConditionWorldAgeDaysMin = new(0, "Minimum world age in in-game days for this configuration to apply.");

    public ConfigurationEntry<float> ConditionWorldAgeDaysMax = new(0, "Maximum world age in in-game days for this configuration to apply. 0 means no max.");

    public ConfigurationEntry<float> DistanceToTriggerPlayerConditions = new(100, "Distance of player to spawner, for player to be included in player based checks such as ConditionNearbyPlayersCarryValue.");

    public ConfigurationEntry<int> ConditionNearbyPlayersCarryValue = new(0, "Checks if nearby players have a combined value in inventory above this condition.\nEg. If set to 100, entry will only activate if nearby players have more than 100 worth of values combined.");

    public ConfigurationEntry<string> ConditionNearbyPlayerCarriesItem = new("", "Checks if nearby players have any of the listed item prefab names in inventory.\nEg. IronScrap, DragonEgg");

    public ConfigurationEntry<float> ConditionNearbyPlayersNoiseThreshold = new(0, "Checks if any nearby players have accumulated noise at or above the threshold.");

    public ConfigurationEntry<string> ConditionNearbyPlayersStatus = new("", "Checks if any nearbly players have any of the listed status effects.\nEg. Wet, Burning");

    public ConfigurationEntry<string> RequiredNotGlobalKey = new("", "Array of global keys which disable the spawning of this entity if any are detected.\nEg. defeated_bonemass,KilledTroll");

    public ConfigurationEntry<string> SetFaction = new("", "Assign a specific faction to spawn. If empty uses default.");

    public ConfigurationEntry<bool> SetRelentless = new(false, "When true, forces mob AI to always be alerted.");

    public ConfigurationEntry<bool> SetTryDespawnOnConditionsInvalid = new(false, "When true, mob will try to run away and despawn when spawn conditions become invalid.\nEg. if spawning only during night, it will run away and despawn at night. Currently this only take into account conditions for daytime and environment.");

    public ConfigurationEntry<bool> SetTryDespawnOnAlert = new(false, "When true, mob will try to run away and despawn when alerted.");

    public ConfigurationEntry<bool> SetTamed = new(false, "When true, mob will be set to tamed status on spawn.");

    public ConfigurationEntry<bool> SetTamedCommandable = new(false, "Experimental. When true, will set mob as commandable when tamed. When false, whatever was default for the creature is used. Does not always seem to work for creatures not tameable in vanilla.");

    public ConfigurationEntry<string> ConditionLocation = new("", "Array of locations in which this spawn is enabled. If empty, allows all.\nEg. Runestone_Boars, FireHole");

    public ConfigurationEntry<float> ConditionAreaSpawnChance = new(100, "Chance for spawn to spawn at all in the area. The chance will be rolled once for the area. Range is 0 to 100. Eg. if a whole area of BlackForest rolls higher than the indicated chance, this spawn template will never be active in that forest. Another BlackForest will have another roll however, that may activate this template there. Chance is rolled based on world seed, area id and template index.");

    public ConfigurationEntry<string> ConditionAreaIds = new("", "Advanced feature. List of area id's in which the template is valid. Note: If ConditionSpawnChanceInArea is not 100 or disabled, it will still roll area chance.\nEg. 1, 123, 543");

    #region Default Configuration Options

    public ConfigurationEntry<string> PrefabName = new("Deer", "Prefab name of the entity to spawn.");

    public ConfigurationEntry<bool> HuntPlayer = new(false, "Sets AI to hunt a player target.");

    public ConfigurationEntry<int> MaxSpawned = new(1, "Maximum entities of type spawned in area.");

    public ConfigurationEntry<float> SpawnInterval = new(90, "Seconds between spawn checks.");

    public ConfigurationEntry<float> SpawnChance = new(100, "Chance to spawn per check. Range 0 to 100.");

    public ConfigurationEntry<int> LevelMin = new(1, "Minimum level to spawn.");

    public ConfigurationEntry<int> LevelMax = new(1, "Maximum level to spawn.");

    public ConfigurationEntry<float> LevelUpMinCenterDistance = new(0, "Minimum distance from world center, to allow higher than min level.");

    public ConfigurationEntry<float> SpawnDistance = new(0, "Minimum distance to another entity.");

    public ConfigurationEntry<float> SpawnRadiusMin = new(0, "Minimum spawn radius.");

    public ConfigurationEntry<float> SpawnRadiusMax = new(0, "Maximum spawn radius.");

    public ConfigurationEntry<string> RequiredGlobalKey = new("", "Required global key to spawn.\nEg. defeated_bonemass");

    public ConfigurationEntry<string> RequiredEnvironments = new("", "Array (separate by comma) of environments required to spawn in.\tEg. Misty, Thunderstorm. Leave empty to allow all.");

    public ConfigurationEntry<int> GroupSizeMin = new(1, "Minimum count to spawn at per check.");

    public ConfigurationEntry<int> GroupSizeMax = new(1, "Maximum count to spawn at per check.");

    public ConfigurationEntry<float> GroupRadius = new(3, "Size of circle to spawn group inside.");

    public ConfigurationEntry<float> GroundOffset = new(0.5f, "Offset to ground to spawn at.");

    public ConfigurationEntry<bool> SpawnDuringDay = new(true, "Toggles spawning at day.");

    public ConfigurationEntry<bool> SpawnDuringNight = new(true, "Toggles spawning at night.");

    public ConfigurationEntry<float> ConditionAltitudeMin = new(-1000, "Minimum altitude (distance to water surface) to spawn in.");

    public ConfigurationEntry<float> ConditionAltitudeMax = new(1000, "Maximum altitude (distance to water surface) to spawn in.");

    public ConfigurationEntry<float> ConditionTiltMin = new(0, "Minium tilt of terrain to spawn in.");

    public ConfigurationEntry<float> ConditionTiltMax = new(35, "Maximum tilt of terrain to spawn in.");

    public ConfigurationEntry<bool> SpawnInForest = new(true, "Toggles spawning in forest.");

    public ConfigurationEntry<bool> SpawnOutsideForest = new(true, "Toggles spawning outside of forest.");

    public ConfigurationEntry<float> OceanDepthMin = new(0, "Minimum ocean depth to spawn in. Ignored if min == max.");

    public ConfigurationEntry<float> OceanDepthMax = new(0, "Maximum ocean depth to spawn in. Ignored if min == max.");

    #endregion
}

internal class SpawnSystemConfigCLLC : Config
{
    public const string ModName = "CreatureLevelAndLootControl";

    public ConfigurationEntry<int> ConditionWorldLevelMin = new(-1, "Minimum CLLC world level for spawn to activate. Negative value disables this condition.");

    public ConfigurationEntry<int> ConditionWorldLevelMax = new(-1, "Maximum CLLC world level for spawn to active. Negative value disables this condition.");

    public ConfigurationEntry<string> SetInfusion = new("", "Assigns the specified infusion to creature spawned. Ignored if empty.");

    public ConfigurationEntry<string> SetExtraEffect = new("", "Assigns the specified effect to creature spawned. Ignored if empty.");

    public ConfigurationEntry<string> SetBossAffix = new("", "Assigns the specified boss affix to creature spawned. Only works for the default 5 bosses. Ignored if empty.");

    public ConfigurationEntry<bool> UseDefaultLevels = new(false, "Use the default LevelMin and LevelMax for level assignment, ignoring the usual CLLC level control.");
}

internal class SpawnSystemConfigMobAI : Config
{
    public const string ModName = "MobAI";

    public ConfigurationEntry<string> SetAI = new("", "Name of MobAI to register for spawn. Eg. the defaults 'Fixer' and 'Worker'.");

    public ConfigurationFileEntry AIConfigFile = new("", "Configuration file to use for the SetAI. Eg. 'MyFixerConfig.json', can include path, but will always start searching from config folder. See MobAI documentation for file setup.");
}

internal class SpawnSystemConfigEpicLoot : Config
{
    public const string ModName = "EpicLoot";

    public ConfigurationEntry<string> ConditionNearbyPlayerCarryItemWithRarity = new("", "Checks if nearby players have any items of the listed rarities.\nEg. Magic, Legendary");

    public ConfigurationEntry<string> ConditionNearbyPlayerCarryLegendaryItem = new("", "Checks if nearby players have any of the listed epic loot legendary id's in inventory.\nEg. HeimdallLegs, RagnarLegs");
}
