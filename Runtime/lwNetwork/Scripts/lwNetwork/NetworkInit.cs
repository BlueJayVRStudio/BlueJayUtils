using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluejayvrstudio;

public class NetworkInit : TempSingleton<NetworkInit>
{
    public bool? IsServer = null;
    public int tickrate;
    void Awake()
    {
        Application.targetFrameRate = 120;
        OVRManager.display.displayFrequency = 120.0f;
    }

    public void StartServer() {
        GetComponent<NetworkDiscoveryServer>().enabled = true;
        GetComponent<NetworkServer>().enabled = true;
        IsServer = true;
    }

    public void StartClient() {
        GetComponent<NetworkDiscoveryClient>().enabled = true;
        GetComponent<NetworkClient>().enabled = true;
        IsServer = false;
    }
}
