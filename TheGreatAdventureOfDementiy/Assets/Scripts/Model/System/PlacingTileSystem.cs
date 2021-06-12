using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public sealed class PlacingTileSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<StopCreatingTag>()
            .WithAll<TileView, LevelCreatingTag>()
            .ForEach((Entity entity, Transform transform, ref TileComponent address) =>
            {
                transform.position = new Vector3(address._xAdress, transform.position.y, address._yAdress);
                PostUpdateCommands.AddComponent<StopCreatingTag>(entity);
            });
    }
}