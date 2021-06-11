﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

public class LevelGenerationSystem : ComponentSystem
{
    int startTileIndex = 0;
    protected override void OnCreate()
    {        
        Debug.Log("Generating Path2");
        startTileIndex = Random.Range(0, 100);
        Debug.Log($"Start index is {startTileIndex}");
    }
    protected override void OnUpdate()
    {
        Entities
            .WithNone<LevelDoneTag>()
            .WithAll<CreatorComponent, StopCreatingTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles, DynamicBuffer<PathTileElement> pathTileElements, DynamicBuffer<OpenTileElement> openTiles) =>
            {
                var weights = new List<int>();
                var startTileEntity = tiles[startTileIndex]._value;
                var startTileObject = EntityManager.GetComponentObject<TileView>(startTileEntity)._tileObjectView;
                var startTileAdress = EntityManager.GetComponentData<TileComponent>(startTileEntity);
                var testo = new OpenTileWeightElement { _value = startTileAdress._weight };
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
                            EntityManager.AddBuffer<OpenTileWeightElement>(entity).Add(NewOpenTileWeightElement);
                            EntityManager.AddBuffer<OpenTileElement>(entity).Add(NewOpenTileElementEntity);                                                                                 
                        }
                        EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._pathNeighborsCount++;
                        EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._visitedNeighborsCount++;
                    }
                    //Get lowest Weight from OpenTileWeightElement (from every opened tiles) and his index
                    if (weights.Count > 0)
                    {
                        var lowesWeight = weights.Min(p => p);
                        var lowestWeightIndex = weights.FindIndex(p => p == lowesWeight);
                        //Debug.Log($"Linq works {lowesWeight} and index is {lowestWeightIndex}");

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
                            Debug.Log($"WORKS REMOVING TILE {i}");
                        }
                        else
                        {
                            weights.Remove(lowesWeight);
                            openTiles.RemoveAt(lowestWeightIndex);
                            Debug.Log($"WORKS NOT REMOVING TILE {i}");
                        }
                    }
                }
                EntityManager.AddComponent<LevelDoneTag>(entity);
            });
    }
}