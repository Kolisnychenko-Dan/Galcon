using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
	public const string GameSceneName = "Game";
	public const string StartSceneName = "StartScene";
	public const string UIScene = "PersistantUI";

	public static IReadOnlyDictionary<int, Color32> PlayerIdToColorMap = new Dictionary<int, Color32>
	{
		{-1, new Color32(77,77,77, 128)},
		{0, new Color32(77,177,255, 128)},
		{1, new Color32(255,50,50, 128)},
		{2, new Color32(50,255,50, 128)},
		{3, new Color32(255,255,50, 128)},
	};
}
