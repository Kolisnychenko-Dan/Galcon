using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Codice.CM.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
	public static class EditorToolsMenu
	{

		#region SceneManagment

		/// <summary>
		/// Function loads selected scene
		/// </summary>
		/// <param name="name">Scene name</param>
		internal static void LoadScene(string name)
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(name, OpenSceneMode.Single);
		}

		/// <summary>
		/// Function changes scene to root and enters playmode
		/// </summary>
		[MenuItem("Galcon Tools/Play", priority = 20)]
		public static void Play()
		{
			LoadScene(EditorBuildSettings.scenes[0].path);
			EditorApplication.EnterPlaymode();
		}

		#endregion //SceneManagment


		#region PlayerPrefs

		/// <summary>
		/// Regenerate scenes list
		/// </summary>
		[MenuItem("Galcon Tools/Generate scenes list", priority = 51)]
		private static void GenerateSceneList()
		{
			// the generated filepath
			string assetsFolderName = "Assets";
			string paScenesFolder = "ProjectAssets/Scenes";
			string paEditorSources = "ProjectAssets/Scripts/Editor";
			string unitySceneExtention = ".unity";
			string unitySceneExtentionPattern = "*" + unitySceneExtention;
			string fullPathToScenes = Path.Combine(Application.dataPath, paScenesFolder);
			string generatedClassName = "GenerateSceneListMenu";
			string generatedFileName = generatedClassName + ".cs";
			string scriptFile = Path.Combine(Application.dataPath, paEditorSources, generatedFileName);

			// an example string array used to generate the items

			var sceneFiles = Directory.GetFiles(fullPathToScenes, unitySceneExtentionPattern);

			List<string> sceneNames = new List<string>(sceneFiles.Length);

			foreach (var i in sceneFiles)
			{
				sceneNames.Add(Path.GetFileNameWithoutExtension(i));
				Debug.Log(Path.GetFileNameWithoutExtension(i));
			}

			// The class string
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("// This class is Auto-Generated");
			sb.AppendLine("using UnityEditor;");
			sb.AppendLine("using UnityEditor.SceneManagement;");
			sb.AppendLine("namespace Editor");
			sb.AppendLine("{");
			sb.AppendLine("	public static class " + generatedClassName + " {");
			sb.AppendLine("");
			// loops though the array and generates the menu items
			int priority = 1;
			foreach (var scene in sceneNames)
			{
				sb.AppendLine("		[MenuItem(\"Galcon Tools/Scenes/Load " + scene.Replace('_', '-') +
					"\", priority=" + priority + ")]");
				sb.AppendLine("		private static void LoadScene" + scene + "() {");
				sb.AppendLine("			EditorToolsMenu.LoadScene(\"" +
					Path.Combine(assetsFolderName, paScenesFolder, scene) + unitySceneExtention + "\");");
				sb.AppendLine("		}");
				sb.AppendLine("");
				++priority;
			}

			sb.AppendLine("	}");
			sb.AppendLine("}");

			// writes the class and imports it so it is visible in the Project window
			System.IO.File.Delete(scriptFile);
			System.IO.File.WriteAllText(scriptFile, sb.ToString(), System.Text.Encoding.UTF8);

			AssetDatabase.ImportAsset(Path.Combine(assetsFolderName, paEditorSources, generatedFileName));
		}

		/// <summary>
		/// Function clears all PlayerPrefs data
		/// </summary>
		[MenuItem("Galcon Tools/Player Prefs Delete All", priority = 32)]
		private static void DeleteAllPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

		#endregion

	}
}