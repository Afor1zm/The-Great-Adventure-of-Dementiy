using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public sealed class LocationCreatorSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<StopCreatingTag>()
            .WithAll<TileView, LevelCreatingTag>()
            .ForEach((Entity entity, Transform transform, ref AddressComponent address) =>
            {
                transform.position = new Vector3(address.xAdress, transform.position.y, address.yAdress);
                EntityManager.AddComponent<StopCreatingTag>(entity);
                Debug.Log("123123");
            });
    }
}
