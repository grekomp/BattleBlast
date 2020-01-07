using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class ClientDataCollection : IEnumerable<ClientData> {
	public List<ClientData> clients = new List<ClientData>();

	public ClientDataCollection() { }
	public ClientDataCollection(List<ClientData> clients) => this.clients = clients;

	public IEnumerator<ClientData> GetEnumerator() {
		return clients.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator() {
		return clients.GetEnumerator();
	}
}
