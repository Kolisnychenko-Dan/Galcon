using DG.Tweening;
using Networking.Abstractions;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Views
{
	[RequireComponent(typeof(Planet))]
	public class PlanetView : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _tintSpriteRenderer;
		[SerializeField] private TextMeshProUGUI _valueText;
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private SpriteRenderer _outline;
		[SerializeField] private GameObject _startPlanetHighlighter;

		private Planet _planet;

		public void SelectPlanet(bool select)
		{
			_outline.gameObject.SetActive(select);
			if (!select)
			{
				_lineRenderer.positionCount = 0;
			}
		}
		
		public void DrawLine(Vector3 lineEndPoint)
		{
			UpdateView();
			_lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, transform.position);
			_lineRenderer.SetPosition(1, lineEndPoint);
		}
		
		private void Awake()
		{
			_planet = GetComponent<Planet>();
		}

		private void Start()
		{
			UpdateView();

			Observable.IntervalFrame(5, FrameCountType.EndOfFrame).Subscribe(_ =>
				{
					UpdateView();
				})
				.AddTo(gameObject);
			
			SelectPlanet(false);

			_startPlanetHighlighter.gameObject.SetActive(false);

			if (_planet.PlayerRef == _planet.Runner.LocalPlayer)
			{
				ShowStartPlanetHighlighter();
			}
		}
		
		private void UpdateView()
		{
			var color = Constants.PlayerIdToColorMap[_planet.OwnerId];
			var alphaStrippedColor = new Color32(color.r,color.g,color.b,255);
			
			_tintSpriteRenderer.color = color;
			_lineRenderer.startColor = alphaStrippedColor;
			_lineRenderer.endColor = alphaStrippedColor;
			_outline.color = alphaStrippedColor;

			_valueText.text = _planet.Value.ToString("0");
		}
		
		private void ShowStartPlanetHighlighter()
		{
			_startPlanetHighlighter.gameObject.SetActive(true);
			
			_startPlanetHighlighter.transform.DORotate(new Vector3(0f, 0f, -180f), 3, RotateMode.FastBeyond360)
				.SetEase(Ease.Linear)
				.SetLoops(1, LoopType.Restart)
				.OnComplete(() => _startPlanetHighlighter.gameObject.SetActive(false));
		}
	}
}