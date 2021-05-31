using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
[GenerateAuthoringComponent]
public struct AddressComponent :  IComponentData
{
    public int xAdress;
    public int yAdress;
}
