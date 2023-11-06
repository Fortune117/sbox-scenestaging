using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Core
{
	public Core() { }

	[Range( 1, 100, 1f )] 
	public float Strength { get; set; } = 10;
	
	[Range( 1, 100, 1f )] 
	public float Dexterity { get; set; } = 10;
	
	[Range( 1, 100, 1f )] 
	public float Constitution { get; set; } = 10;
	
	[Range( 1, 100, 1f )] 
	public float Wisdom { get; set; } = 10;
	
	[Range( 1, 100, 1f )] 
	public float Intelligence { get; set; } = 10;
	
	[Range( 1, 100, 1f )] 
	public float Charisma { get; set; } = 10;
}
