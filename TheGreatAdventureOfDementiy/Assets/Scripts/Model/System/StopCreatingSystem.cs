using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class StopCreatingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<TileView>()
            .ForEach((Entity entity, Transform transform, ref TileComponent address) =>
            {                
                //EntityManager.RemoveComponent<WallComponent>(entity);
                //Debug.Log("wow");
            });
                
    }
}
