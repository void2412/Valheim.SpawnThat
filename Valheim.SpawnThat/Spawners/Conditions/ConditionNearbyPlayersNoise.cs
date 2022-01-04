﻿using System.Collections.Generic;
using Valheim.SpawnThat.Spawners.Contexts;
using Valheim.SpawnThat.Utilities;
using Valheim.SpawnThat.Utilities.Extensions;

namespace Valheim.SpawnThat.Spawners.Conditions;

public class ConditionNearyPlayersNoise : ISpawnCondition
{
    private int SearchDistance { get; }
    private int NoiseThreshold { get; }

    public bool CanRunClientSide => true;
    public bool CanRunServerSide => true;

    public ConditionNearyPlayersNoise(int distanceToSearch, int noiseRequired)
    {
        SearchDistance = distanceToSearch;
        NoiseThreshold = noiseRequired;
    }

    public bool IsValid(SpawnSessionContext context)
    {
        if (SearchDistance <= 0)
        {
            return false;
        }

        if (NoiseThreshold <= 0)
        {
            return false;
        }

        List<ZDO> players = PlayerUtils.GetPlayerZdosInRadius(context.SpawnerZdo.GetPosition(), SearchDistance);

        foreach (var player in players)
        {
            if (player is null)
            {
                continue;
            }

            if (player.GetNoise() >= NoiseThreshold)
            {
                return true;
            }
        }

        return false;
    }
}