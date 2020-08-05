using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsableValueHud : MonoBehaviour
{
	[SerializeField]
	private GameObject _imageCollapseUp;

	[SerializeField]
	private GameObject _imageCollapseDown;

	private bool _isOpen = true;

	public void ToggleState()
	{
		_isOpen = !_isOpen;
		_imageCollapseUp.SetActive(_isOpen);
		_imageCollapseDown.SetActive(!_isOpen);
	}
}