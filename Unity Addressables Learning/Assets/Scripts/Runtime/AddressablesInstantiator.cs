using UnityEngine;

public class AddressablesInstantiator : MonoBehaviour
{
	private bool _instantiated;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T) && !_instantiated)
		{
			AddressablesLoader.Instance.LoadAssetAsync<GameObject>("environment_prefab",
				(prefab) => Instantiate(prefab));

			_instantiated = true;
		}
	}
}