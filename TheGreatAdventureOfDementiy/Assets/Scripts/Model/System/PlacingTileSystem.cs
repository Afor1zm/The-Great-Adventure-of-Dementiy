using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public sealed class PlacingTileSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<StopCreatingTag>()
            .WithAll<TileView, LevelCreatingTag>()
            .ForEach((Entity entity, Transform transform, ref TileComponent address) =>
            {
                transform.position = new Vector3((address._xAdress * 5f), transform.position.y, (address._yAdress * 5f));
                PostUpdateCommands.AddComponent<StopCreatingTag>(entity);
            });

    }
}