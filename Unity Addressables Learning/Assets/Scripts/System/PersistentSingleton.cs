using UnityEngine;

/// <summary>
/// Makes a permanent singleton reference for the entire game session, which persists between different scenes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
	protected override void SetInstance()
	{
		base.SetInstance();
		DontDestroyOnLoad(gameObject);
	}
}
