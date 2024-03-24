using System;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using Game.Abstractions;
using Stateless;
using UniRx;
using UnityEngine.SceneManagement;

namespace Game
{
	public class GameStateService : AService, IGameStateService
	{
		private Subject<StateMachine<State, Trigger>.Transition> _onStateChanged;
		private StateMachine<State, Trigger> StateMachine { get; } = new (State.Start, FiringMode.Queued);
		
		public IObservable<StateMachine<State, Trigger>.Transition> StateChangedObservable => _onStateChanged;

		public void ChangeGameState(Trigger trigger)
		{
			StateMachine.FireAsync(trigger);
		}

		public override void Initialize()
		{
			base.Initialize();

			ConfigureStateMachine();
			Start();
			
			StateMachine.OnTransitionCompleted(transition => _onStateChanged.OnNext(transition));
		}
		
		private void Start() => StateMachine.FireAsync(Trigger.OpenLogin);

		private void ConfigureStateMachine()
		{
			StateMachine.Configure(State.Start)
				.Permit(Trigger.OpenLogin, State.Login);
			
			StateMachine.Configure(State.Login)
				.Permit(Trigger.GoToLobby, State.Lobby);
			
			StateMachine.Configure(State.Lobby)
				.Permit(Trigger.ConnectToRoom, State.WaitingPlayers);
			
			StateMachine.Configure(State.WaitingPlayers)
				.Permit(Trigger.StartGame, State.GameRunning);
		}

		private async Task LoadSceneAsync(Scene scene, LoadSceneMode loadSceneMode, bool makeActive = true)
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

		private async Task UnloadSceneAsync(Scene scene)
		{
			var asyncOp = SceneManager.UnloadSceneAsync(scene.ToString());
			await asyncOp;
		}
	}
}