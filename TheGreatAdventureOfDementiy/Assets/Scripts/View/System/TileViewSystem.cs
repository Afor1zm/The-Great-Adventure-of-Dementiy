using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TileViewSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<AdditionalTag>()
            .WithAll<LevelDoneTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles, DynamicBuffer<PathTileElement> pathTiles, DynamicBuffer<WallTileElements> wallTiles) =>
            {
                foreach (var tile in tiles)
                {
                    if (EntityManager.HasComponent<TilePathTag>(tile._value))
                    {
                        var newPathElement = new PathTileElement { _value = tile._value };
                        pathTiles.Add(newPathElement);

                        var neighbourTileComponent = EntityManager.GetComponentData<NeighborsComponent>(tile._value);
                        if (neighbourTileComponent._pathNeighborsCount == 1)
                        {
                            EntityManager.GetComponentObject<TileView>(tile._value)._deadEndTile.SetActive(true);
                        }
                        if (neighbourTileComponent._pathNeighborsCount == 2)
                        {
                            EntityManager.GetComponentObject<TileView>(tile._value)._angleTile.SetActive(true);
                        }
                        if (neighbourTileComponent._pathNeighborsCount == 3)
                        {
                            EntityManager.GetComponentObject<TileView>(tile._value)._aisleTile.SetActive(true);
                        }
                        if (neighbourTileComponent._pathNeighborsCount == 4)
                        {
                            EntityManager.GetComponentObject<TileView>(tile._value)._pathTile.SetActive(true);
                        }
                    }
                    else
                    {
                        var newWallElement = new WallTileElements { _value = tile._value };
                        wallTiles.Add(newWallElement);
                        var wallView = EntityManager.GetComponentObject<TileView>(tile._value);
                        wallView._tileObjectView.SetActive(true);
                    }
                }
                EntityManager.AddComponent<AdditionalTag>(entity);
            });
    }
}
