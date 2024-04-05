using UnityEngine;

namespace UI.Abstractions
{
	public interface IPopupService
	{
		T CreatePopup<T>(T prefab, Transform parent = null) where T : Object;
	}
}