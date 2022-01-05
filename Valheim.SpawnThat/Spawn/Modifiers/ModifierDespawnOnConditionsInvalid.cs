﻿using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Valheim.SpawnThat.Spawn.Modifiers;

public class ModifierDespawnOnConditionsInvalid : ISpawnModifier
{
    public const string ZdoConditionDay = "spawnthat_condition_daytime_day";
    public const string ZdoConditionNight = "spawnthat_condition_daytime_night";
    public const string ZdoConditionEnvironment = "spawnthat_condition_environments";
    public const string ZdoFeature = "spawnthat_despawn_on_invalid";

    public static int ZdoConditionDayHash { get; } = ZdoConditionDay.GetStableHashCode();
    public static int ZdoConditionNightHash { get; } = ZdoConditionNight.GetStableHashCode();
    public static int ZdoConditionEnvironmentHash { get; } = ZdoConditionEnvironment.GetStableHashCode();
    public static int ZdoFeatureHash { get; } = ZdoFeature.GetStableHashCode();

    private bool? ConditionAllowDuringDay { get; }
    private bool? ConditionAllowDuringNight { get; }
    private string ConditionAllowDuringEnvironments { get; }

    public ModifierDespawnOnConditionsInvalid(
        bool? conditionAllowDuringDay = null, 
        bool? conditionAllowDuringNight = null, 
        List<string> conditionAllowDuringEnvironments = null)
    {
        ConditionAllowDuringDay = conditionAllowDuringDay;
        ConditionAllowDuringNight = conditionAllowDuringNight;
        ConditionAllowDuringEnvironments = conditionAllowDuringEnvironments?.Join();
    }

    public void Modify(GameObject entity, ZDO entityZdo)
    {
        if (entityZdo is null)
        {
            return;
        }
        
        entityZdo.Set(ZdoFeature, true);

        if (ConditionAllowDuringDay is not null)
        {
            entityZdo.Set(ZdoConditionDayHash, ConditionAllowDuringDay.Value);
        }
        if (ConditionAllowDuringNight is not null)
        {
            entityZdo.Set(ZdoConditionNightHash, ConditionAllowDuringNight.Value);
        }
        if (ConditionAllowDuringEnvironments is not null)
        {
            entityZdo.Set(ZdoConditionEnvironmentHash, ConditionAllowDuringEnvironments);
        }
    }
}
