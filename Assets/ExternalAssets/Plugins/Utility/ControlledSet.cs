using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class ControlledSet : SerializableWideClass { }

/// <summary>
/// A collection with callbacks for added and removed elements. Internally it keeps the elements distinct.
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class ControlledSet<T> : ControlledSet, ISerializationCallbackReceiver, IEnumerable<T> {
	[NonSerialized]
	List<T> _elements = new List<T>();
	[SerializeField]
	List<T> elements = new List<T>();
	public List<T> Elements {
		get {
			return new List<T>(elements);
		}
		set {
			elements = value;
			OnValidate();
		}
	}

	#region Events
	public event Action OnCollectionUpdated;
	public event Action<T> OnElementAdded;
	public event Action<T> OnElementRemoved;

	protected virtual void HandleCollectionUpdated() {
		OnCollectionUpdated?.Invoke();
		DebugCollectionUpdated();
	}
	protected virtual void HandleElementAdded(T element) {
		OnElementAdded?.Invoke(element);
		DebugElementAdded(element);
	}
	protected virtual void HandleElementRemoved(T element) {
		OnElementRemoved?.Invoke(element);
		DebugElementRemoved(element);
	}
	#endregion

	#region Manipulating elements
	public void Add(T elementToAdd) {
		if (_elements.Contains(elementToAdd) == false) {
			_elements.Add(elementToAdd);
			elements.Add(elementToAdd);

			HandleElementAdded(elementToAdd);
			HandleCollectionUpdated();
		}
	}
	public void Remove(T elementToRemove) {
		if (_elements.Contains(elementToRemove) == true) {
			_elements.Remove(elementToRemove);
			elements.Remove(elementToRemove);

			HandleElementRemoved(elementToRemove);
			HandleCollectionUpdated();
		}
	}
	public void Replace(int index, T newElement) {
		if (_elements.Count > index) {
			HandleElementRemoved(_elements[index]);

			_elements[index] = newElement;
			elements[index] = newElement;

			HandleElementAdded(_elements[index]);
			HandleCollectionUpdated();
		}
	}
	public T ElementAt(int index) {
		if (_elements.Count > index) {
			return _elements[index];
		}

		return default;
	}
	#endregion

	#region Helpers
	public void OnValidate() {
		bool collectionUpdated = false;

		// Removed elements
		foreach (var item in _elements.Except(elements)) {
			HandleElementRemoved(item);
			collectionUpdated = true;
		}

		// Added elements
		foreach (var item in elements.Except(_elements)) {
			HandleElementAdded(item);
			collectionUpdated = true;
		}

		//// Changed elements
		//for (int i = 0; i < elements.Count(); i++) {
		//	if (elements[i].Equals(_elements[i]) == false) {
		//		collectionUpdated = true;
		//		break;
		//	}
		//}

		// If the type is cloneable, clone all elements, otherwise just create a copy of the list
		if (typeof(ICloneable).IsAssignableFrom(typeof(T))) {
			_elements = elements.Distinct().Select(e => (T)((ICloneable)e).Clone()).ToList();
		}
		else {
			_elements = elements.Distinct().ToList();
		}

		if (collectionUpdated)
			HandleCollectionUpdated();
	}
	public void OnAfterDeserialize() {
		OnValidate();
	}
	public void OnBeforeSerialize() {
		OnValidate();
	}

	public IEnumerator<T> GetEnumerator() {
		return ((IEnumerable<T>)Elements).GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator() {
		return ((IEnumerable<T>)Elements).GetEnumerator();
	}

	public static implicit operator ControlledSet<T>(List<T> list) {
		return new ControlledSet<T> {
			Elements = list
		};
	}
	public static implicit operator List<T>(ControlledSet<T> set) {
		return set.Elements;
	}
	#endregion

	#region Debugging 
	[SerializeField]
	public bool debugEnabled = false;
	public bool debugCollectionUpdated = true;
	public bool debugElementAdded = true;
	public bool debugElementRemoved = true;

	protected void DebugCollectionUpdated() {
		if (debugEnabled && debugCollectionUpdated) {
			Debug.Log(string.Format("{0} (ControlledSet): Collection Updated", this));
		}
	}
	protected void DebugElementAdded(T element) {
		if (debugEnabled && debugElementAdded) {
			Debug.Log(string.Format("{0} (ControlledSet): Element added: {1}", this, element));
		}
	}
	protected void DebugElementRemoved(T element) {
		if (debugEnabled && debugElementRemoved) {
			Debug.Log(string.Format("{0} (ControlledSet): Element removed: {1}", this, element));
		}
	}
	#endregion
}
