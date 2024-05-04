﻿using System;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using Game.Abstractions;
using Networking.Abstractions;
using Stateless;
using Tools;
using UniRx;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Game
{
	public class AppStateService : AService, IAppStateService
	{
		private readonly Subject<StateMachine<State, Trigger>.Transition> _onStateChanged = new ();
		private StateMachine<State, Trigger> StateMachine { get; } = new (State.Start, FiringMode.Queued);

		[Inject] private INetworkRunnerService _networkRunnerService;
		
		public IObservable<StateMachine<State, Trigger>.Transition> StateChangedObservable => _onStateChanged;

		public void ChangeGameState(Trigger trigger)
		{
			StateMachine.FireAsync(trigger);
		}

		public override async void Initialize()
		{
			base.Initialize();

			Application.targetFrameRate = 60;
			
			ConfigureStateMachine();

			StateMachine.OnTransitionCompleted(transition => _onStateChanged.OnNext(transition));
			
			await LoadSceneAsync(Constants.UIScene, LoadSceneMode.Additive);
		}

		private void ConfigureStateMachine()
		{
			StateMachine.Configure(State.Start)
				.Permit(Trigger.OpenLogin, State.Login)
				.Permit(Trigger.ConnectToRoom, State.WaitingPlayers);

			StateMachine.Configure(State.WaitingPlayers)
				.Permit(Trigger.StartGame, State.GameRunning)
				.OnEntryFromAsync(Trigger.ConnectToRoom, () => UnloadSceneAsync(Constants.StartSceneName))
				.OnEntry(() =>
				{
					EventManager.Instance.EmitEvent(EventNames.ToggleLobbyWaitScreen, true);
				})
				.OnExit(() =>
				{
					EventManager.Instance.EmitEvent(EventNames.ToggleLobbyWaitScreen, false);
				});

			StateMachine.Configure(State.GameRunning)
				.Permit(Trigger.EndGame, State.Ending)
				.OnEntryAsync(async() =>
				{
					await _networkRunnerService.LoadNetworkScene(Constants.GameSceneName, LoadSceneMode.Additive);
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(Constants.GameSceneName));
				});

			StateMachine.Configure(State.Ending)
				.Permit(Trigger.GoToStartScreen, State.Start)
				.OnExitAsync(async() =>
				{
					await UniTask.WhenAll(
						_networkRunnerService.GetCurrentNetworkRunner().Shutdown().AsUniTask(),
						UnloadSceneAsync(Constants.GameSceneName),
						LoadSceneAsync(Constants.StartSceneName, LoadSceneMode.Additive)
					);
				});
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