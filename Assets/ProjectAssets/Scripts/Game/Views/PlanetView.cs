using TMPro;
using UniRx;
using UnityEngine;

namespace Game.Views
{
	[RequireComponent(typeof(Planet))]
	public class PlanetView : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _tintSpriteRenderer;
		[SerializeField] private TextMeshProUGUI _valueText;
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private SpriteRenderer _outline;

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

			Observable.IntervalFrame(20, FrameCountType.EndOfFrame).Subscribe(_ =>
				{
					UpdateView();
				})
				.AddTo(gameObject);
			
			SelectPlanet(false);
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
	}
}