using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AYellowpaper.SerializedCollections;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AddressablesLoader : PersistentSingleton<AddressablesLoader>
{
	[Header("Labels Dictionary"), Space]
	[SerializeField] private SerializedDictionary<AddressablesLabel, AssetLabelReference> _labelsDictionary;

	public bool IsInitialized => _isInitialized;

	private HashSet<AsyncOperationHandle> _inUsedHandles = new HashSet<AsyncOperationHandle>();
	private bool _isInitialized = false;

	private void OnDestroy()
	{
		Debug.Log("Releasing all addressables handles...");
		ReleaseAllHandles();
	}

	#region Loader's Life Cycle Methods
	public void Initialize()
	{
		Debug.Log("<b>Initializing Addressables...</b>");

		Addressables.InitializeAsync().Completed += (op) =>
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				Debug.Log("Addressables initialization <b><color=#64AE20>SUCCESS!</color></b>");

				_isInitialized = true;
			}
			else
			{
				Debug.LogError($"Addressables initialization <b><color=#DB3333>FAILED!</color></b> Catalog requests most likely <b>timed out</b>: {op.DebugName}.\n" +
					$"Reason: {op.OperationException}");
			}
		};
	}

	public void ReleaseAllHandles()
	{
		foreach (var handle in _inUsedHandles)
		{
			if (handle.IsValid())
			{
				Addressables.Release(handle);
			}
		}

		_inUsedHandles.Clear();
	}
	#endregion

	#region Asset Loading Methods Using Labels
	public AsyncOperationHandle<T> LoadAssetAsync<T>
	(
		AddressablesLabel label,
		Action<T> onCompleteCallback,
		Action<AsyncOperationHandle<T>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<T> handle = default;

		if (_labelsDictionary.TryGetValue(label, out var assetLabelReference))
		{
			handle = Addressables.LoadAssetAsync<T>(assetLabelReference);
			handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);
		}

		return handle;
	}

	public AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>
	(
		AddressablesLabel label,
		Action<T> onEachAssetLoaded,
		Action<IList<T>> onCompleteCallback,
		Action<AsyncOperationHandle<IList<T>>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<IList<T>> handle = default;

		if (_labelsDictionary.TryGetValue(label, out var assetLabelReference))
		{
			handle = Addressables.LoadAssetsAsync(assetLabelReference, onEachAssetLoaded);
			handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);
		}

		return handle;
	}
	#endregion

	#region Asset Loading Methods Using Path
	public AsyncOperationHandle<T> LoadAssetAsync<T>
	(
		string path,
		Action<T> onCompleteCallback,
		Action<AsyncOperationHandle<T>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<T> handle = default;

		handle = Addressables.LoadAssetAsync<T>(path);
		handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);

		return handle;
	}

	public AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>
	(
		string path,
		Action<T> onEachAssetLoaded,
		Action<IList<T>> onCompleteCallback,
		Action<AsyncOperationHandle<IList<T>>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<IList<T>> handle = default;

		handle = Addressables.LoadAssetsAsync(path, onEachAssetLoaded);
		handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);

		return handle;
	}
	#endregion

	#region Scene Loading Methods
	public AsyncOperationHandle<SceneInstance> LoadSceneAsync
	(
		string path,
		LoadSceneMode mode,
		Action<SceneInstance> onCompleteCallback,
		Action<AsyncOperationHandle<SceneInstance>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(path, mode);
		handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);

		return handle;
	}

	public AsyncOperationHandle<SceneInstance> LoadSceneAsync
	(
		string path,
		LoadSceneMode mode,
		bool activateOnLoad,
		Action<SceneInstance> onCompleteCallback,
		Action<AsyncOperationHandle<SceneInstance>> onFailedCallback = null
	)
	{
		AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(path, mode, activateOnLoad);
		handle.Completed += (op) => OnAssetLoaded(op, onCompleteCallback, onFailedCallback);

		return handle;
	}
	#endregion

	#region Event Wrappers
	private void OnAssetLoaded<T>(AsyncOperationHandle<T> op, Action<T> onCompleteCallback, Action<AsyncOperationHandle<T>> onFailedCallback)
	{
		if (op.Status == AsyncOperationStatus.Succeeded)
		{
			_inUsedHandles.Add(op);
			onCompleteCallback?.Invoke(op.Result);
		}
		else
		{
			onFailedCallback?.Invoke(op);
			Debug.LogError($"<b><color=#DB3333>FAILED</color></b> to load asset: {op.DebugName}.\n" +
				$"Reason: {op.OperationException}");
		}
	}
	#endregion
}
