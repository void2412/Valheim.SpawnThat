﻿using System.Collections.Generic;
using System.Linq;
using Valheim.SpawnThat.Configuration.ConfigTypes;
using Valheim.SpawnThat.Reset;
using Valheim.SpawnThat.Spawners.SpawnerSpawnSystem.Conditions;

namespace Valheim.SpawnThat.Spawners.SpawnerSpawnSystem
{
    public class ConditionManager
    {
        private HashSet<IConditionOnAwake> OnAwakeConditions = new HashSet<IConditionOnAwake>();
        private HashSet<IConditionOnSpawn> OnSpawnConditions = new HashSet<IConditionOnSpawn>();

        private static ConditionManager _instance;

        public static ConditionManager Instance
        {
            get
            {
                return _instance ??= new ConditionManager();
            }
        }

        ConditionManager()
        {
            StateResetter.Subscribe(() =>
            {
                _instance = null;
            });

            #region Add OnAwake
            
            OnAwakeConditions.Add(ConditionDistanceToCenter.Instance);

            #endregion

            #region Add OnSpawn

            OnSpawnConditions.Add(ConditionWorldAge.Instance);
            OnSpawnConditions.Add(ConditionGlobalKeys.Instance);

            #endregion
        }

        public bool FilterOnAwake(SpawnSystem spawner, SpawnConfiguration config)
        {
            return OnAwakeConditions.Any(x => x.ShouldFilter(spawner, config));
        }

        public bool FilterOnSpawn(SpawnSystem.SpawnData spawner)
        {
            var cache = SpawnDataCache.Get(spawner);

            if (cache?.Config == null)
            {
                return false;
            }

            return OnSpawnConditions.Any(x => x.ShouldFilter(spawner, cache.Config));
        }
    }
}
