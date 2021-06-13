using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;


public class TileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{   
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int count = 0;
    public GameObject[] _tiles;
    private void Awake()
    {
        _tiles = new GameObject[width*height];
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {                
                _tiles[count] = Instantiate(tilePrefab, tilePrefab.transform.position, Quaternion.identity);
                count++;
            }
        }        
    }
    public void Convert(Entity creatorEntity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {        
        var tileElements = new NativeArray<TileElement>(_tiles.Length, Allocator.Temp);
        var countEntity = 0;
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                //Get conversed Entity from GameObjects
                var tileElementEntity = conversionSystem.GetPrimaryEntity(_tiles[countEntity]);
                
                //Added entity for TileElementBuffer
                tileElements[countEntity] = new TileElement() { _value = tileElementEntity }; 
                
                dstManager.AddBuffer<NeighborsElement>(tileElementEntity);

                //Added buffer Path and Weight for Creator Entity
                dstManager.AddBuffer<PathTileElement>(creatorEntity);
                dstManager.AddBuffer<OpenTileElement>(creatorEntity);
                dstManager.AddBuffer<WallTileElements>(creatorEntity);
                dstManager.AddBuffer<TileElement>(creatorEntity).Add(tileElements[countEntity]);

                // Generate Random weight for all tiles
                var weight = Random.Range(0, 100);
                dstManager.AddComponentData(tileElementEntity, new TileComponent() { _xAdress = i, _yAdress = y, _weight = weight });
                countEntity++;
            }
        }
    }
}
