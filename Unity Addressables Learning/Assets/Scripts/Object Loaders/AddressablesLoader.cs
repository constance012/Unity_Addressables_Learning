using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesLoader : MonoBehaviour
{
	[SerializeField, AssetReferenceUILabelRestriction("environment")]
	private AssetReferenceGameObject environmentReference;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			environmentReference.LoadAssetAsync().Completed += Addressables_OnLoadAssetCompleted;
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log($"Is instantiated object null: {environmentReference.IsValid()}");
			environmentReference.ReleaseAsset();
			Debug.Log($"Is instantiated object null: {environmentReference.IsValid()}");
		}
	}

	private void Addressables_OnLoadAssetCompleted(AsyncOperationHandle<GameObject> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			Instantiate(handle.Result);
		}
		else
		{
			Debug.LogError($"Failed to load addressable {handle.OperationException}");
		}
	}
}