using UnityEngine;
using System.Collections;

public class OnLoad : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}
