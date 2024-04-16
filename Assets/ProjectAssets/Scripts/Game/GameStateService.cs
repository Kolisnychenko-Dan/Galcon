﻿using System;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using Game.Abstractions;
using Networking.Abstractions;
using ProjectAssets.Scripts;
using Stateless;
using Tools;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
	public class GameStateService : AService, IGameStateService
	{
		private readonly Subject<StateMachine<State, Trigger>.Transition> _onStateChanged = new ();
		private StateMachine<State, Trigger> StateMachine { get; } = new (State.Start, FiringMode.Queued);

		[Inject] private INetworkRunnerService _networkRunnerService;
		
		public IObservable<StateMachine<State, Trigger>.Transition> StateChangedObservable => _onStateChanged;

		public void ChangeGameState(Trigger trigger)
		{
			StateMachine.FireAsync(trigger);
		}

		public override void Initialize()
		{
			base.Initialize();

			ConfigureStateMachine();

			StateMachine.OnTransitionCompleted(transition => _onStateChanged.OnNext(transition));
			
			LoadSceneAsync(Constants.UIScene, LoadSceneMode.Additive);
		}

		private void ConfigureStateMachine()
		{
			StateMachine.Configure(State.Start)
				.Permit(Trigger.OpenLogin, State.Login)
				.Permit(Trigger.GoToLobby, State.Lobby);
			
			StateMachine.Configure(State.Login)
				.Permit(Trigger.GoToLobby, State.Lobby)
				.Permit(Trigger.GoBack, State.Start);

			StateMachine.Configure(State.Lobby)
				.Permit(Trigger.ConnectToRoom, State.WaitingPlayers);

			StateMachine.Configure(State.WaitingPlayers)
				.Permit(Trigger.StartGame, State.GameRunning)
				.OnEntry(() =>
				{
					EventManager.Instance.EmitEvent(EventNames.ToggleLobbyWaitScreen, true);
				})
				.OnExit(() =>
				{
					EventManager.Instance.EmitEvent(EventNames.ToggleLobbyWaitScreen, false);
				});

			StateMachine.Configure(State.GameRunning)
				.OnEntryAsync(() => _networkRunnerService.LoadNetworkScene(Constants.GameSceneName));
		}

		private async UniTask LoadSceneAsync(Scene scene, LoadSceneMode loadSceneMode, bool makeActive = true)
		{
			var sceneName = scene.ToString();
			var asyncOp = SceneManager.LoadSceneAsync(sceneName, loadSceneMode)
				.ToUniTask(Progress.Create<float>(_ => {}));

			await asyncOp;

			if (makeActive)
			{
				var sceneInstance = SceneManager.GetSceneByName(sceneName);
				SceneManager.SetActiveScene(sceneInstance);
			}
		}

		private async UniTask UnloadSceneAsync(Scene scene)
		{
			var asyncOp = SceneManager.UnloadSceneAsync(scene.ToString());
			await asyncOp;
		}
		
		private async UniTask LoadSceneAsync(string scene, LoadSceneMode loadSceneMode, bool makeActive = true)
		{
			var asyncOp = SceneManager.LoadSceneAsync(scene, loadSceneMode)
				.ToUniTask(Progress.Create<float>(_ => {}));

			await asyncOp;

			if (makeActive)
			{
				var sceneInstance = SceneManager.GetSceneByName(scene);
				SceneManager.SetActiveScene(sceneInstance);
			}
		}

		private async UniTask UnloadSceneAsync(string scene)
		{
			var asyncOp = SceneManager.UnloadSceneAsync(scene);
			await asyncOp;
		}
	}
}