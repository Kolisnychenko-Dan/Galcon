using System;
using System.Linq;
using Fusion;
using Game.Abstractions;
using MonoInstallers;
using Networking.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
	public class PlanetSwipeDetector : MonoBehaviour, IPlanetSwipeDetector
	{
		[SerializeField] private Camera _mainCamera;
		
		private Vector2 _swipeStartPos;
		private Planet _startPlanet;
		private Planet _currentPlanet;
		private PlayerRef _localPlayer;
		private ILobbyService _lobbyService;

		private readonly Subject<Planet> _onPlanetStartSelect = new();
		private readonly Subject<(Planet startPlanet, Planet hoverPlanet)> _onPlanetHover = new();
		private readonly Subject<(Planet startPlanet, Planet hoverPlanet)> _onPlanetStopHover = new();
		private readonly Subject<(Planet startPlanet, Planet endPlanet)> _onPlanetSwipe = new();

		[Inject] private INetworkRunnerService _networkRunnerService;
			
		public IObservable<Planet> OnPlanetStartSelect => _onPlanetStartSelect;
		public IObservable<(Planet startPlanet, Planet hoverPlanet)> OnPlanetHover => _onPlanetHover;
		public IObservable<(Planet startPlanet, Planet hoverPlanet)> OnPlanetStopHover => _onPlanetStopHover;
		public IObservable<(Planet startPlanet, Planet endPlanet)> OnPlanetSwipe => _onPlanetSwipe;

		private void Start()
		{
			_localPlayer = _networkRunnerService.GetCurrentNetworkRunner().LocalPlayer;
			
			Observable.EveryUpdate()
				.Where(_ => Input.touchCount > 0)
				.Subscribe(_ => DetectSwipe())
				.AddTo(gameObject);
		}

		private void DetectSwipe()
		{
			_lobbyService ??= _networkRunnerService.LobbyService;

			var touch = Input.GetTouch(0);
			
			if (touch.phase == TouchPhase.Began /* && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)*/)
			{
				_swipeStartPos = touch.position;
				_startPlanet = DetectPlanetUnderPoint(_swipeStartPos);

				if (_startPlanet != null && _lobbyService.PlayerRefToIdMap[_localPlayer] != _startPlanet.OwnerId)
				{
					_startPlanet = null;
				}
				
				if (_startPlanet != null)
				{
					_onPlanetStartSelect.OnNext(_startPlanet);
				}
			}
			else if (touch.phase == TouchPhase.Moved && _startPlanet != null)
			{
				var currentPlanet = DetectPlanetUnderPoint(touch.position);
				if (currentPlanet == null && _currentPlanet != null)
				{
					_onPlanetStopHover.OnNext((_startPlanet, _currentPlanet));
				}

				if (currentPlanet != null && currentPlanet != _currentPlanet && _startPlanet != _currentPlanet)
				{
					_onPlanetHover.OnNext((_startPlanet, currentPlanet));
				}
				
				_currentPlanet = currentPlanet;
			}
			else if (touch.phase == TouchPhase.Ended && _startPlanet != null)
			{
				var endPlanet = DetectPlanetUnderPoint(touch.position);
				if (endPlanet != null && _startPlanet != endPlanet)
				{
					_onPlanetSwipe.OnNext((_startPlanet, endPlanet));
				}

				_startPlanet = null;
				_currentPlanet = null;
			}
		}

		private Planet DetectPlanetUnderPoint(Vector2 point)
		{
			var ray = _mainCamera.ScreenPointToRay(point);
			return Physics2D.RaycastAll(ray.origin, ray.direction)
				.Select(hit => hit.collider.gameObject.GetComponent<Planet>()).FirstOrDefault();
		}
	}
}