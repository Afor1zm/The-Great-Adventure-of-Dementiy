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
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {        
        var tileElements = new NativeArray<TileElement>(_tiles.Length, Allocator.Temp);
        var countEntity = 0;
        for (int i = 0; i < width; i++)
        {
            for (int y = 0; y < height; y++)
            {
                var tileElementEntity = conversionSystem.GetPrimaryEntity(_tiles[countEntity]);
                Debug.Log($"{tilePrefab.transform.position}");
                tileElements[countEntity] = new TileElement() { _value = tileElementEntity };
                dstManager.AddBuffer<TileElement>(entity).Add(tileElements[countEntity]);
                dstManager.AddComponentData(tileElementEntity, new AddressComponent() { xAdress = i, yAdress = y });
                countEntity++;
            }
        }
    }
}
