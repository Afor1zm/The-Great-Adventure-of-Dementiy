using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public sealed class LocationCreator : SystemBase
{
    protected override void OnUpdate()
    {
        var keyDown = Input.GetKeyDown(KeyCode.Space);
        Entities.ForEach((Entity entity) =>
        {
           
        })
            .Run();
    }
}
