using Game.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Views
{
	public class PlanetSwipeReactorView : MonoBehaviour
	{
		[SerializeField] private Camera _mainCamera;
		
		[Inject] private IPlanetSwipeDetector _planetSwipeDetector;

		private CompositeDisposable _planetDrawLineDisposable;

		private void Start()
		{
			_planetSwipeDetector.OnPlanetStartSelect.Subscribe(planet =>
			{
				var pv = planet.GetComponent<PlanetView>();
				OnPlanetSelected(pv);
			}).AddTo(gameObject);
			
			_planetSwipeDetector.OnPlanetHover.Subscribe(planets =>
			{
				var (startPv, hoverPv) = (planets.startPlanet.GetComponent<PlanetView>(), planets.hoverPlanet.GetComponent<PlanetView>());
				_planetDrawLineDisposable?.Dispose();
				
				hoverPv.SelectPlanet(true);
				startPv.SelectPlanet(true);
				startPv.DrawLine(hoverPv.transform.position);
			}).AddTo(gameObject);
			
			_planetSwipeDetector.OnPlanetStopHover.Subscribe(planets =>
			{
				var (startPv, hoverPv) = (planets.startPlanet.GetComponent<PlanetView>(), planets.hoverPlanet.GetComponent<PlanetView>());
				
				hoverPv.SelectPlanet(false);
				OnPlanetSelected(startPv);
			}).AddTo(gameObject);
			
			_planetSwipeDetector.OnPlanetSwipe.Subscribe(planets =>
			{
				var (startPv, endPv) = (planets.startPlanet.GetComponent<PlanetView>(), planets.endPlanet.GetComponent<PlanetView>());
				
				_planetDrawLineDisposable?.Dispose();
				startPv.SelectPlanet(false);
				endPv.SelectPlanet(false);
			}).AddTo(gameObject);
		}

		private void OnPlanetSelected(PlanetView pv)
		{
			pv.SelectPlanet(true);

			_planetDrawLineDisposable?.Dispose();
			_planetDrawLineDisposable = new CompositeDisposable();
			
			Observable.EveryUpdate().Subscribe(_ =>
				{
					if (Input.touchCount > 0)
					{
						var touchPosition = Input.GetTouch(0).position;
							
						float distanceToPlane = Mathf.Abs(_mainCamera.transform.position.z);
						var screenPosition = new Vector3(touchPosition.x, touchPosition.y, distanceToPlane);
							
						var mouseWorldPos = _mainCamera.ScreenToWorldPoint(screenPosition);
							
						pv.DrawLine(mouseWorldPos);
					}
					else
					{
						pv.SelectPlanet(false);
						_planetDrawLineDisposable.Dispose();
					}
				})
				.AddTo(_planetDrawLineDisposable);
		}
	}
}