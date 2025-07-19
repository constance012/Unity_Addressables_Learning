using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class AddressablesChangesDetector
{
	private static readonly Dictionary<string, string> _currentHashes = new Dictionary<string, string>();
	private static readonly Dictionary<string, string> _previousHashes = new Dictionary<string, string>();

	public static void CompareAndLogChanges()
	{
		LoadHashes(AddressablesUtils.RemoteBuildPath, _currentHashes);

		if (!Directory.Exists(AddressablesUtils.SnapshotPath))
		{
			Debug.LogWarning("No previous snapshot found. Save a snapshot first.");
			return;
		}

		LoadHashes(AddressablesUtils.SnapshotPath, _previousHashes);

		List<string> changedFiles = new List<string>();

		foreach (var kvp in _currentHashes)
		{
			if (!_previousHashes.TryGetValue(kvp.Key, out var oldHash) || oldHash != kvp.Value)
			{
				changedFiles.Add(kvp.Key);
			}
		}

		Debug.Log($"<b>REMOTE ADDRESSABLES BUILD REPORT:</b> {changedFiles.Count} file(s) changed:");
		foreach (var file in changedFiles)
		{
			Debug.Log($"Changed: {file}");
		}
	}

	public static void SaveCurrentSnapshot()
	{
		if (Directory.Exists(AddressablesUtils.RemoteBuildPath))
		{
			AddressablesUtils.TryDeleteDirectory(AddressablesUtils.SnapshotPath);
			Directory.CreateDirectory(AddressablesUtils.SnapshotPath);

			foreach (var file in Directory.GetFiles(AddressablesUtils.RemoteBuildPath, "*", SearchOption.AllDirectories))
			{
				string relativePath = file.Substring(AddressablesUtils.RemoteBuildPath.Length + 1);
				string destPath = Path.Combine(AddressablesUtils.SnapshotPath, relativePath);
				string destDir = Path.GetDirectoryName(destPath);

				if (!Directory.Exists(destDir))
					Directory.CreateDirectory(destDir);

				File.Copy(file, destPath, true);
			}

			Debug.Log($"Snapshot saved to: {AddressablesUtils.SnapshotPath}");
		}
	}

	private static void LoadHashes(string path, Dictionary<string, string> hashDict)
	{
		if (Directory.Exists(path))
		{
			hashDict.Clear();
			var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var hash = ComputeHash(file);
				var relativePath = file.Substring(path.Length + 1).Replace("\\", "/");
				hashDict[relativePath] = hash;
			}
		}
	}

	private static string ComputeHash(string filePath)
	{
		using (var md5 = MD5.Create())
		using (var stream = File.OpenRead(filePath))
		{
			byte[] hashBytes = md5.ComputeHash(stream);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
		}
	}
}
