using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class StopPlacingTileSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<StopCreatingTag>()
            .WithAll<CreatorComponent>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles) =>
            {
                var tileWithStopTag = 0;
                foreach (var tile in tiles)
                {
                    if (EntityManager.HasComponent<StopCreatingTag>(tile._value))
                    {
                        tileWithStopTag++;
                    }
                }
                if (tileWithStopTag == tiles.Length)
                {
                    EntityManager.AddComponent<StopCreatingTag>(entity);
                }
            });
    }
}
