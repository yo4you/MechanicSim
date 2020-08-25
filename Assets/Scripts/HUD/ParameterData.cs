public class ParameterData
{
	/// <summary>
	/// stored value
	/// </summary>
	private object value = null;

	/// <summary>
	/// value read from value storage
	/// </summary>
	private ValueEntry refrenceValue = null;

	private bool isRefrenceValue = false;
	private bool _isRefrenceOnlyValue;

	public bool IsRefrenceOnlyValue { get => _isRefrenceOnlyValue; internal set => _isRefrenceOnlyValue = value; }
	public bool IsRefrenceValue { get => isRefrenceValue; set => isRefrenceValue = value; }
	public ValueEntry RefrenceValue { get => refrenceValue; set => refrenceValue = value; }
	public object Value { get => value; set => this.value = value; }
}