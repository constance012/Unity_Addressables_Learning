using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEngine;


[CreateAssetMenu(fileName = "CustomBuildScript", menuName = "Addressables/Content Builders/Custom Build Script")]
public class CustomAddressablesBuilder : BuildScriptPackedMode
{
	public override string Name => "Custom Build Script";

	protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
	{
		AddressablesChangesDetector.SaveCurrentSnapshot();

		AddressablesUtils.DeleteLocalBuildFolder();
		AddressablesUtils.DeleteRemoteBuildFolder();

		var result = base.BuildDataImplementation<TResult>(builderInput);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		AddressablesChangesDetector.CompareAndLogChanges();

		AddressablesUtils.OpenLocalBuildFolder();
		AddressablesUtils.OpenRemoteBuildFolder();

		return result;
	}
}
