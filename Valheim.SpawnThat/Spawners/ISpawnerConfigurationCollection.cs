﻿using System.Collections.Generic;

namespace Valheim.SpawnThat.Spawners;

public interface ISpawnerConfigurationCollection
{
    List<ISpawnerConfiguration> SpawnerConfigurations { get; }

    TSpawnerConfig GetOrAddSpawnerConfiguration<TSpawnerConfig>(TSpawnerConfig spawnerConfig)
        where TSpawnerConfig : class, ISpawnerConfiguration;
}
