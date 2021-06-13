using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

public class LevelGenerationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<LevelDoneTag>()
            .WithAll<CreatorComponent, StopCreatingTag>()
            .ForEach((Entity entity, DynamicBuffer<TileElement> tiles, DynamicBuffer<OpenTileElement> openTiles) =>
            {
                var startTileIndex = Random.Range(0, 100);
                var weights = new List<int>();
                var startTileEntity = tiles[startTileIndex]._value;
                var startTileObject = EntityManager.GetComponentObject<TileView>(startTileEntity)._startTile;
                startTileObject.SetActive(true);
                EntityManager.GetComponentData<NeighborsComponent>(startTileEntity)._visitedNeighborsCount++;
                tiles.RemoveAt(startTileIndex);
                PostUpdateCommands.AddComponent<TilePathTag>(startTileEntity);
                var checkEntity = startTileEntity;

                var firsPath = EntityManager.GetBuffer<NeighborsElement>(checkEntity).AsNativeArray();
                foreach (var near in firsPath)
                {
                    EntityManager.GetComponentData<NeighborsComponent>(near._value)._pathNeighborsCount++;
                }

                do
                {
                    //Visiting Neighbors for get weight from them 
                    var Neighbors = EntityManager.GetBuffer<NeighborsElement>(checkEntity).AsNativeArray();

                    //Getting weight form every neighboors
                    foreach (var neighbour in Neighbors)
                    {
                        if (EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._visitedNeighborsCount == 0)
                        {
                            var NeighbourWeight = EntityManager.GetComponentData<TileComponent>(neighbour._value)._weight;                            
                            var NewOpenTileElementEntity = new OpenTileElement { _value = neighbour._value };
                            weights.Add(NeighbourWeight);
                            EntityManager.AddBuffer<OpenTileElement>(entity).Add(NewOpenTileElementEntity);
                        }                       
                        EntityManager.GetComponentData<NeighborsComponent>(neighbour._value)._visitedNeighborsCount++;
                    }

                    //Get lowest Weight from OpenTileWeightElement (from every opened tiles) and his index
                    var lowesWeight = weights.Min(p => p);
                    var lowestWeightIndex = weights.FindIndex(p => p == lowesWeight);

                    //Checked neighbour with lowest weight
                    var lowestWieghtNeighborEntity = openTiles[lowestWeightIndex]._value;
                    var count = EntityManager.GetComponentData<NeighborsComponent>(lowestWieghtNeighborEntity)._pathNeighborsCount;
                    if (count < 2)
                    {
                        weights.Remove(lowesWeight);
                        openTiles.RemoveAt(lowestWeightIndex);

                        EntityManager.GetComponentObject<TileView>(lowestWieghtNeighborEntity)._tileObjectView.SetActive(false);
                        checkEntity = lowestWieghtNeighborEntity;
                        PostUpdateCommands.AddComponent<TilePathTag>(checkEntity);
                        foreach (var near in Neighbors)
                        {
                            EntityManager.GetComponentData<NeighborsComponent>(near._value)._pathNeighborsCount++;
                        }
                    }
                    else
                    {
                        weights.Remove(lowesWeight);
                        openTiles.RemoveAt(lowestWeightIndex);
                    }
                } while (weights.Count > 0);
                EntityManager.AddComponent<LevelDoneTag>(entity);
            });
    }
}
