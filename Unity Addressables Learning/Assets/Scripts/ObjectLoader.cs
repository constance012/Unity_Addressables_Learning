using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
	[SerializeField] private Transform _environmentPrefab;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Instantiate(_environmentPrefab);
		}
	}
}
