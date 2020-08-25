using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollapsableTimelineContainer : MonoBehaviour
{
	private CollapsableValueHud[] _collapsables;
	private List<float> _collapsedAnchorY = new List<float>();
	private float _openHeight;
	private RectTransform _contentWindow;

	private void Start()
	{
		_collapsables = GetComponentsInChildren<CollapsableValueHud>();
		_contentWindow = GetComponent<ScrollRect>().content;
		foreach (var collapsable in _collapsables)
		{
			collapsable.OnToggle += Redraw;
			var rect = collapsable.GetComponent<RectTransform>();
			_collapsedAnchorY.Add(rect.anchoredPosition.y);
			_openHeight = rect.sizeDelta.y;
		}
	}

	public void Redraw()
	{
		var offset = 0f;
		for (int i = 0; i < _collapsables.Length; i++)
		{
			CollapsableValueHud collapsable = _collapsables[i];
			var rect = collapsable.GetComponent<RectTransform>();
			var newpos = rect.anchoredPosition;
			newpos.y = _collapsedAnchorY[i] - offset;
			rect.anchoredPosition = newpos;
			if (!collapsable.IsOpen)
			{
				offset += _openHeight;
			}
		}
		_contentWindow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, offset + 250f);
	}
}