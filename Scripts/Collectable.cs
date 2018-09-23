using System;
using UnityEngine;

[Serializable]
public class Collectable
{

    // TODO: multiple collectables such as items, house wares, etc
    public string GUID;
    protected Collectable()
    {
        System.Guid _guid = System.Guid.NewGuid();
        GUID = _guid.ToString();
    }
}