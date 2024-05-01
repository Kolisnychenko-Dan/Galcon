﻿using System;
using Stateless;
using UniRx;

namespace Abstractions
{
	public interface IAppStateService
	{
		public void ChangeGameState(Trigger trigger);

		public IObservable<StateMachine<State, Trigger>.Transition> StateChangedObservable { get; }
	}
	
	public enum State
	{
		Start,
		Login,
		WaitingPlayers,
		GameRunning,
		Ending
	}

	public enum Trigger
	{
		OpenLogin,
		ConnectToRoom,
		StartGame,
		EndGame,
		GoToStartScreen,
		Restart,
		GoToLobby,
		GoBack,
	}
}