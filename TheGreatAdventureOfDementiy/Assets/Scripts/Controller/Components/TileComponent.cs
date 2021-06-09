using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
[GenerateAuthoringComponent]
public struct TileComponent :  IComponentData
{
    public int _xAdress;
    public int _yAdress;
    public int _weight;
}
