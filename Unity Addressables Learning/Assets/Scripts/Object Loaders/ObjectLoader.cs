using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
	[SerializeField] private GameObject _environmentPrefab;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Instantiate(_environmentPrefab);
		}
	}
}
