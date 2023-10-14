namespace DarkDescent;

public class StatValue
{
	public StatValue(float baseValue)
	{
		Base = baseValue;
	}

	public float Base { get; set; }
	public float Modifier { get; set; }

	public float Effective => Base + Modifier;

	/// <summary>
	/// implicitly get the effect value is you just access the stat value
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static implicit operator float(StatValue value)
	{
		return value.Effective;
	}

	public void Reset()
	{
		Modifier = 0;
	}
}
