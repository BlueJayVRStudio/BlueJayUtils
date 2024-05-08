using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using bluejayvrstudio;

public class NetworkObjectsManager : TempSingleton<NetworkObjectsManager>
{
    public List<GameObject> toAdd;
    public List<GameObject> toRemove;
    Dictionary<GameObject, string> UUIDLookUp;

    void Awake()
    {
        toAdd = new();
        toRemove = new();
    }

    void Start() 
    {
        UUIDLookUp = new();
    }
    
    void Update()
    {
        if (NetworkInit.CurrInst.IsServer != true) return;
        while (toAdd.Count > 0) {
            var newGuid = Guid.NewGuid().ToString();
            UUIDLookUp[toAdd[toAdd.Count - 1]] = newGuid;
            NetworkServer.CurrInst.AddItem(newGuid, toAdd[toAdd.Count - 1]);
            toAdd.RemoveAt(toAdd.Count-1);
        }
        while (toRemove.Count > 0) {
            string uuid = UUIDLookUp[toRemove[toRemove.Count - 1]];
            NetworkServer.CurrInst.RemoveItem(uuid);
            UUIDLookUp.Remove(toRemove[toRemove.Count - 1]);
            Destroy(toRemove[toRemove.Count - 1]);
            toRemove.RemoveAt(toRemove.Count - 1);
        }
    }
}
