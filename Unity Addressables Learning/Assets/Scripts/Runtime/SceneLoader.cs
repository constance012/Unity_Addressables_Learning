using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneLoader : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			AddressablesLoader.Instance.LoadSceneAsync("Game", LoadSceneMode.Single,
				(sceneInstance) => Debug.Log($"Scene {sceneInstance.Scene.name} loaded!"));
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			SceneManager.LoadSceneAsync("Scenes/Game");
		}
	}
}