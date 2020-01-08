using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class ClientManager {

    public Action<ClientData> onAddClientData;

    public List<ClientData> clients = new List<ClientData>();
    List<ClientData> awaitingClients = new List<ClientData>();

    public void Init() {
        IntroduceResponseHandler.onIntroduceResponse += AddClient;
    }

    public void OnClientConnected(int id, string ip) {
        ClientData client = new ClientData(id,ip);
        if (!awaitingClients.Exists((c) => c.connectionId == id)) {
            awaitingClients.Add(client);
        }
    }

    public ClientData OnClientDisconnected(int connectionID) {
        ClientData data = clients.Find(x => x.connectionId == connectionID);       
        if(data == null) {
            awaitingClients.Remove(
                awaitingClients.Find(x => x.connectionId == connectionID
            ));
        }
        else {
            clients.Remove(data);
        }
        return data;
    }

    void AddClient(IntroduceResponseData data) {
        ClientData client = clients.Find(x => x.connectionId == data.clientId);
        if (client != null) {
            DebugNet.Log(string.Format("Reintroduced {0}.", client.ToString()));
            client.IntroduceMe(data);
        }
        else {
            client = awaitingClients.Find(x => x.connectionId == data.clientId);
            if (client != null) {
                client.IntroduceMe(data);
                awaitingClients.Remove(client);
                clients.Add(client);
                DebugNet.Log(string.Format("Introduced {0}.", client.ToString()));
                onAddClientData?.Invoke(client);
            }
        }
    }

}
