using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;

public static class AddressablesUtils
{
	public static string CurrentPlatform => EditorUserBuildSettings.activeBuildTarget.ToString();
	public static string ServerDataPath => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "ServerData"));
	public static string RemoteBuildPath => Path.Combine(ServerDataPath, CurrentPlatform);
	public static string LocalBuildPath => Addressables.BuildPath;
	public static string SnapshotPath => Path.Combine(ServerDataPath, "AddressablesSnapshots");

	[MenuItem("Tools/Addressables/Open Local Build Folder")]
	public static void OpenLocalBuildFolder()
	{
		Debug.Log($"Local Path: {LocalBuildPath}");
		EditorUtility.RevealInFinder(LocalBuildPath);
	}

	[MenuItem("Tools/Addressables/Open Remote Build Folder")]
	public static void OpenRemoteBuildFolder()
	{
		Debug.Log($"Remote Path: {RemoteBuildPath}");
		EditorUtility.RevealInFinder(RemoteBuildPath);
	}

	[MenuItem("Tools/Addressables/Delete Local Build Folder")]
	public static void DeleteLocalBuildFolder()
	{
		Debug.Log($"Local Path: {LocalBuildPath}");
		TryDeleteDirectory(LocalBuildPath);
	}

	[MenuItem("Tools/Addressables/Delete Remote Build Folder")]
	public static void DeleteRemoteBuildFolder()
	{
		Debug.Log($"Remote Path: {RemoteBuildPath}");
		TryDeleteDirectory(RemoteBuildPath);
	}

	public static void TryDeleteDirectory(string path)
	{
		if (Directory.Exists(path))
		{
			try
			{
				Directory.Delete(path, true);
				Debug.Log($"Directory deleted: {path}");
			}
			catch (IOException ex)
			{
				Debug.LogError($"Failed to delete directory: {path}. Error: {ex.Message}");
			}
		}
		else
		{
			Debug.LogWarning($"Directory does not exist: {path}");
		}
	}
}
