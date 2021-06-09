using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public class NeighborsComponent : IComponentData
{
    public int _visitedNeighborsCount;
    public int _pathNeighborsCount;    
}
