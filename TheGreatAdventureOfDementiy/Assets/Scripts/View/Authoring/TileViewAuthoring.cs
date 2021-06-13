using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TileViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject _startTilePrefab;
    public GameObject _aisleTilePrefab;
    public GameObject _angleTilePrefab;
    public GameObject _deadEndTilePrefab;
    public GameObject _fieldTilePrefab;

    void Start()
    {
        //_fieldTilePrefab = GetComponent<TileView>()._pathTile;
    }

    void Update()
    {
        
    }

    public void Convert(Entity creatorEntity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

    }
}
