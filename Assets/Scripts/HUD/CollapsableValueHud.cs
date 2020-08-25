using UnityEngine;

public class CollapsableValueHud : MonoBehaviour
{
	[SerializeField]
	private GameObject _imageCollapseUp;

	[SerializeField]
	private GameObject _imageCollapseDown;

	private bool _isOpen = true;

	public bool IsOpen { get => _isOpen; }

	public delegate void OnToggleDelegate();

	public event OnToggleDelegate OnToggle;

	public void ToggleState()
	{
		_isOpen = !IsOpen;
		_imageCollapseUp.SetActive(IsOpen);
		_imageCollapseDown.SetActive(!IsOpen);
		OnToggle?.Invoke();
	}
}