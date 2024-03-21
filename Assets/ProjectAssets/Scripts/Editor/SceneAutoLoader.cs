using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
	[InitializeOnLoad]
	static class SceneAutoLoader
	{
		// Static constructor binds a playmode-changed callback.
		// [InitializeOnLoad] above makes sure this gets executed.
		static SceneAutoLoader()
		{
			EditorApplication.playmodeStateChanged += OnPlayModeChanged;
			cEditorPrefLoadFirstOnPlay += Application.productName;
			cEditorPrefPreviousScene += Application.productName;
		}

		[MenuItem("Galcon Tools/Scene AutoSwitcher", true)]
		private static bool SwitchSceneOnPlayValidator()
		{
			Menu.SetChecked("Galcon Tools/Scene AutoSwitcher", LoadFirstSceneOnPlay);
			return true;
		}

		[MenuItem("Galcon Tools/Scene AutoSwitcher")]
		private static void SwitchSceneOnPlay()
		{
			LoadFirstSceneOnPlay = !LoadFirstSceneOnPlay;
		}

		// Play mode change callback handles the scene load/reload.
		private static void OnPlayModeChanged()
		{
			if (!LoadFirstSceneOnPlay)
			{
				return;
			}


			//EditorBuildSettings.scenes[0].path
			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// User pressed play -- autoload master scene.
				PreviousSceneIndex = SceneManager.GetActiveScene().buildIndex;
				EditorToolsMenu.Play();
			}

			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// User pressed stop -- reload previous scene.
				EditorToolsMenu.LoadScene(EditorBuildSettings.scenes[PreviousSceneIndex].path);
			}
		}

		// Properties are remembered as editor preferences.
		private static string cEditorPrefLoadFirstOnPlay = "SceneSwitcher.LoadMasterOnPlay.";
		private static string cEditorPrefPreviousScene = "SceneSwitcher.PreviousScene.";

		private static bool LoadFirstSceneOnPlay
		{
			get { return EditorPrefs.GetBool(cEditorPrefLoadFirstOnPlay, false); }
			set { EditorPrefs.SetBool(cEditorPrefLoadFirstOnPlay, value); }
		}

		private static int PreviousSceneIndex
		{
			get { return EditorPrefs.GetInt(cEditorPrefPreviousScene, SceneManager.GetActiveScene().buildIndex); }
			set { EditorPrefs.SetInt(cEditorPrefPreviousScene, value); }
		}
	}
}