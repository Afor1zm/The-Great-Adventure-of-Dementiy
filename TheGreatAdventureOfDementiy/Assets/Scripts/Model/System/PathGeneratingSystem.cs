using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

public class PathGeneratingSystem : ComponentSystem
{
    int startTileIndex = 0;
    protected override void OnCreate()
    {        
        Debug.Log("Generating Path2");
        startTileIndex = Random.Range(0, 100);
        Debug.Log($"{startTileIndex}");
    }
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
                    Debug.Log($"WORKS GOOD");
                    EntityManager.AddComponent<StopCreatingTag>(entity);
                }

            });

        Entities
            .WithNone<LevelDoneTag>()
            .WithAll<CreatorComponent, StopCreatingTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles) =>
            {
                foreach (var tile in tiles)
                {
                    var tileAdressX = EntityManager.GetComponentData<TileComponent>(tile._value)._xAdress;
                    var tileAdressY = EntityManager.GetComponentData<TileComponent>(tile._value)._yAdress;

                    for (int i = 0; i < tiles.Length; i++)
                    {
                        var checkingTile = tiles[i]._value;
                        var checkingTileAdressX = EntityManager.GetComponentData<TileComponent>(checkingTile)._xAdress;
                        var checkingTileAdressY = EntityManager.GetComponentData<TileComponent>(checkingTile)._yAdress;

                        if ((checkingTileAdressX -1 == tileAdressX) && (checkingTileAdressY == tileAdressY))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile});
                        }

                        if ((checkingTileAdressX + 1 == tileAdressX) && (checkingTileAdressY == tileAdressY))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }

                        if ((checkingTileAdressX  == tileAdressX) && (checkingTileAdressY == tileAdressY - 1))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }

                        if ((checkingTileAdressX  == tileAdressX) && (checkingTileAdressY == tileAdressY + 1))
                        {
                            EntityManager.GetBuffer<NeighborsElement>(tile._value).Add(new NeighborsElement { _value = checkingTile });
                        }
                        EntityManager.AddComponent<LevelDoneTag>(checkingTile);
                    }
                    
                }
            });

        Entities
            .WithNone<LevelDoneTag>()
            .WithAll<CreatorComponent, StopCreatingTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles, DynamicBuffer<PathTileElement> pathTileElements, DynamicBuffer<OpenTileElement> openTiles) =>
            {
                var weights = new List<int>();
                //Find all usefull links from startTile
                var startTileEntity = tiles[startTileIndex]._value;
                var startTileObject = EntityManager.GetComponentObject<TileView>(startTileEntity)._tileObjectView;
                //var startNeighborsComponent = EntityManager.GetComponentData<NeighborsComponent>(startTileEntity);
                var startTileAdress = EntityManager.GetComponentData<TileComponent>(startTileEntity);
                var testo = new OpenTileWeightElement { _value = startTileAdress._weight };
                Debug.Log($"Deleted start point with weight {EntityManager.GetComponentData<TileComponent>(startTileEntity)._weight}");
                startTileObject.SetActive(false);
                EntityManager.AddBuffer<OpenTileWeightElement>(entity).Add(testo);

                //Add StartTile to PathTileElement buffer
                var newPathTileElement = new PathTileElement() { _value = startTileEntity };
                pathTileElements.Add(newPathTileElement);

                
                var checkEntity = startTileEntity;
                
                for (int i = 0; i < 60; i++)
                {
                    //Visiting Neighbors for get weight from them 
                    var Neighbors = EntityManager.GetBuffer<NeighborsElement>(checkEntity).AsNativeArray();

                    //Getting weight form every neighboors
                    foreach (var neighbour in Neighbors)
                    {
                        if (EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._visitedNeighborsCount == 0)
                        {
                            var NeighbourWeight = EntityManager.GetComponentData<TileComponent>(neighbour._value)._weight;
                            var NewOpenTileWeightElement = new OpenTileWeightElement { _value = NeighbourWeight };
                            var NewOpenTileElementEntity = new OpenTileElement { _value = neighbour._value };                    
                            weights.Add(NewOpenTileWeightElement._value);
                            EntityManager.AddBuffer<OpenTileElement>(entity).Add(NewOpenTileElementEntity);
                            EntityManager.AddBuffer<OpenTileWeightElement>(entity).Add(NewOpenTileWeightElement);                            
                            //EntityManager.AddComponent<VisitedTileTag>(neighbour._value);                            
                        }
                        EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._pathNeighborsCount++;
                        EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._visitedNeighborsCount++;
                    }
                    //Get lowest Weight form OpenTileWeightElement and his index
                    if (weights.Count > 0)
                    {
                        var lowesWeight = weights.Min(p => p);
                        var lowestWeightIndex = weights.FindIndex(p => p == lowesWeight);
                        Debug.Log($"Linq works {lowesWeight} and index is {lowestWeightIndex}");

                        //Checked neighbour with lowest weight
                        var lowestWieghtNeighborEntity = openTiles[lowestWeightIndex]._value;
                        var count = EntityManager.GetComponentData<NeighborsComponent>(lowestWieghtNeighborEntity)._pathNeighborsCount;
                        if (count < 2)
                        {
                            weights.Remove(lowesWeight);
                            openTiles.RemoveAt(lowestWeightIndex);

                            EntityManager.GetComponentObject<TileView>(lowestWieghtNeighborEntity)._tileObjectView.SetActive(false);
                            var newOpenPathTile = new PathTileElement { _value = lowestWieghtNeighborEntity };
                            EntityManager.AddBuffer<PathTileElement>(entity).Add(newOpenPathTile);
                            checkEntity = lowestWieghtNeighborEntity;

                        }
                        else
                        {
                            weights.Remove(lowesWeight);
                            openTiles.RemoveAt(lowestWeightIndex);
                            //EntityManager.AddComponent<VisitedTileTag>(lowestWieghtNeighborEntity);
                        }
                    }
                }
                EntityManager.AddComponent<LevelDoneTag>(entity);
                //Debug.Log($"Generating Path {tiles.Length}");

            });
    }
}
