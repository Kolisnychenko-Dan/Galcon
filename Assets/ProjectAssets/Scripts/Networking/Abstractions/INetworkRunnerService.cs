using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine.SceneManagement;

namespace Networking.Abstractions
{
	public interface INetworkRunnerService
	{
		NetworkRunner GetCurrentNetworkRunner();
		NetworkLobbyService LobbyService { get; }
		UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
		public Task<StartGameResult> StartGame(StartGameArgs gameArgs);
	}
}