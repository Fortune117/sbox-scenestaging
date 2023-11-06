using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Misc
{
	public Misc() { }

	/// <summary>
	/// How much weight we can carry.
	/// Increased by our Strength score.
	/// </summary>
	[Range( 0, 400 )]
	public float CarryWeight { get; set; } = 0f;

	[Range( 0, 100 )]
	public float Luck { get; set; } = 0;
}
