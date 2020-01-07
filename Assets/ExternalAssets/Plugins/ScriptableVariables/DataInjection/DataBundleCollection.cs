using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class DataBundleCollection : SerializableWideClass {
	public List<DataBundle> dataBundles = new List<DataBundle>();

	public void Add(DataBundle dataBundle) {
		dataBundles.Add(dataBundle);
	}
	public void AddOrUpdate(DataBundle dataBundle) {
		DataBundle existingDataBundle = GetDataBundle(dataBundle.id);

		if (existingDataBundle) {
			existingDataBundle.CopyValuesFrom(dataBundle);
		}
		else {
			Add(dataBundle);
		}
	}

    public void Remove(string id) {
        DataBundle dataBundle = GetDataBundle(id);
        
        if (dataBundle != null) {
            dataBundles.Remove(dataBundle);
        }
    }

	public DataBundle GetDataBundle(string id) {
		return dataBundles.Find(db => db.id == id);
	}

	/// <summary>
	/// Updates the DataBundleCollection, wherever possible updating only values and not changing the bound variables, to avoid reinjecting the collection.
	/// </summary>
	public void CopyValuesFrom(DataBundleCollection dataBundleCollection) {
		if (dataBundleCollection == this) return;

		List<DataBundle> remainingDataBundles = dataBundles.Where(db => dataBundleCollection.dataBundles.Contains(db)).ToList();
		dataBundles.Clear();

		foreach (DataBundle dataBundle in dataBundleCollection.dataBundles) {
			DataBundle existingDataBundle = remainingDataBundles.Find(db => db.id == dataBundle.id);

			if (existingDataBundle) {
				existingDataBundle.CopyValuesFrom(dataBundle);
				Add(existingDataBundle);
			}
			else {
				Add(dataBundle);
			}
		}
	}
}
