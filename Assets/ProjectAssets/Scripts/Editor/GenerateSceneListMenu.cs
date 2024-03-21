// This class is Auto-Generated
using UnityEditor;
using UnityEditor.SceneManagement;
namespace Editor
{
	public static class GenerateSceneListMenu {

		[MenuItem("Galcon Tools/Scenes/Load Game", priority=1)]
		private static void LoadSceneGame() {
			EditorToolsMenu.LoadScene("Assets/ProjectAssets/Scenes/Game.unity");
		}

		[MenuItem("Galcon Tools/Scenes/Load StartScene", priority=2)]
		private static void LoadSceneStartScene() {
			EditorToolsMenu.LoadScene("Assets/ProjectAssets/Scenes/StartScene.unity");
		}

	}
}
