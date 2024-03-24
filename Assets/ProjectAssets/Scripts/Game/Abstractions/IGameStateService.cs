﻿using System;
using Stateless;
using UniRx;

namespace Game.Abstractions
{
	public interface IGameStateService
	{
		public void ChangeGameState(Trigger trigger);

		public IObservable<StateMachine<State, Trigger>.Transition> StateChangedObservable { get; }
	}
	
	public enum State
	{
		Start,
		Login,
		Lobby,
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
		Restart,
		GoToLobby
	}
}