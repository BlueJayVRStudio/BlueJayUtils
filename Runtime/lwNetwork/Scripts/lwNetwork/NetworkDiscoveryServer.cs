using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
// using UnityEditor.PackageManager;
// using Google.Protobuf;

[RequireComponent(typeof(NetworkServer))]
public class NetworkDiscoveryServer : MonoBehaviour
{
    private UdpClient udpClient;
    private IPAddress ipAddress = null;
    [SerializeField] private const int listenPort = 8100;
    float timeout_limit = 2.0f;

    NetworkServer Server;
    Dictionary<string, float> ClientLookUp = new();
    
    void Awake() 
    {
        Server = GetComponent<NetworkServer>();
    }
    void OnEnable() 
    {
        udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, listenPort));
        Socket udpSocket = udpClient.Client;
        // udpSocket.DontFragment = true;
        udpSocket.ReceiveBufferSize = 65507;

        StartListening();
        Debug.Log("Is Listening");
    }


    void Update() 
    {
        foreach (string address in ClientLookUp.Keys.ToList()) {
            ClientLookUp[address] += Time.deltaTime;
            if (ClientLookUp[address] >= timeout_limit) {
                Server.destroy_player(address);
                ClientLookUp.Remove(address);
            }
        }
    }

    void StartListening() => udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    
    void ReceiveCallback(IAsyncResult ar) {
        try {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            string message = Encoding.UTF8.GetString(bytes);
            string clientAddress = ip.Address.ToString();
            if (message.Length >= 9)
            {
                if (message.Substring(0, 9) == ClientConsts.CONNECTION_REQUEST) {
                    Debug.Log("Received response from: " + clientAddress + " Message: " + message);
                    var serverPort = 8100;
                    var sendBytes = Encoding.UTF8.GetBytes(ServerConsts.CONNECTION_REPLY);
                    IPEndPoint endPoint = new IPEndPoint(ip.Address, serverPort);
                    udpClient.Send(sendBytes, sendBytes.Length, endPoint);
                    ClientLookUp[clientAddress] = 0;
                    Server.InstantiateQueue.Enqueue(clientAddress);
                }
                if (message.Substring(0, 9) == ClientConsts.CONTROLLER_POSITIONS) {
                    if (ClientLookUp.ContainsKey(clientAddress) && ClientLookUp[clientAddress] < timeout_limit) {
                        ClientLookUp[clientAddress] = 0f;
                        Server.positions_queue.Enqueue((clientAddress, bytes.Skip(9).Take(bytes.Length - 9).ToArray()));
                    }
                }
                if (message.Substring(0, 9) == ClientConsts.AXES) {
                    if (ClientLookUp.ContainsKey(clientAddress) && ClientLookUp[clientAddress] < timeout_limit) {
                        ClientLookUp[clientAddress] = 0f;
                        Server.axes_queue.Enqueue((clientAddress, bytes.Skip(9).Take(bytes.Length - 9).ToArray()));
                    }
                }
                if (message.Substring(0, 9) == ClientConsts.INPUTS) {
                    if (ClientLookUp.ContainsKey(clientAddress) && ClientLookUp[clientAddress] < timeout_limit) {
                        ClientLookUp[clientAddress] = 0f;
                        Server.input_queue.Enqueue((clientAddress, bytes.Skip(9).Take(bytes.Length - 9).ToArray()));
                    }
                }
            }

            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }
        catch (Exception e) {
            Debug.Log(e);
            Debug.Log("UDP Client Closed");
        }
    }

    public void SendToAll(string header, byte[] message) {
        byte[] bytes = Encoding.UTF8.GetBytes(header).Concat(message).ToArray();

        foreach (string address in ClientLookUp.Keys.ToList()) {
            if (ClientLookUp[address] < timeout_limit) {
                IPAddress remoteIPAddress = IPAddress.Parse(address);
                IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, listenPort);
                udpClient.Send(bytes, bytes.Length, remoteEndPoint);}}}

    public void SendToSpecific(string address, string header, byte[] message) {
        if (!ClientLookUp.ContainsKey(address) || ClientLookUp[address] >= timeout_limit) return;
        IPAddress remoteIPAddress = IPAddress.Parse(address);
        IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, listenPort);
        byte[] bytes = Encoding.UTF8.GetBytes(header).Concat(message).ToArray();
        udpClient.Send(bytes, bytes.Length, remoteEndPoint);}

    void OnApplicationQuit() { udpClient?.Close(); }
    void OnDestroy() { udpClient?.Close(); }
    void OnDisable() { udpClient?.Close(); }
}
