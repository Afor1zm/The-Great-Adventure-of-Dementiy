using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AddingNeighborSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<LevelDoneTag>()
            .WithAll<CreatorComponent, StopCreatingTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles) =>
            {
                foreach (var tile in tiles)
                {
                    //Set neighbors
                    var tileAdressX = EntityManager.GetComponentData<TileComponent>(tile._value)._xAdress;
                    var tileAdressY = EntityManager.GetComponentData<TileComponent>(tile._value)._yAdress;

                    for (int i = 0; i < tiles.Length; i++)
                    {
                        var checkingTile = tiles[i]._value;
                        var checkingTileAdressX = EntityManager.GetComponentData<TileComponent>(checkingTile)._xAdress;
                        var checkingTileAdressY = EntityManager.GetComponentData<TileComponent>(checkingTile)._yAdress;

                        if ((checkingTileAdressX - 1 == tileAdressX) && (checkingTileAdressY == tileAdressY))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }

                        if ((checkingTileAdressX + 1 == tileAdressX) && (checkingTileAdressY == tileAdressY))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }

                        if ((checkingTileAdressX == tileAdressX) && (checkingTileAdressY == tileAdressY - 1))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }

                        if ((checkingTileAdressX == tileAdressX) && (checkingTileAdressY == tileAdressY + 1))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }
                    }
                }
            });
    }
}
