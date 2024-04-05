using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine.SceneManagement;

namespace Networking.Abstractions
{
	public interface INetworkRunnerService
	{
		NetworkRunner GetCurrentNetworkRunner();
		UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
	}
}