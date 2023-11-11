using DarkDescent.Actor.Damage;
using DarkDescent.Actor.UI;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	public int Level { get; set; } = 1;
	
	public float Experience { get; set; } = 0;
	
	public float ExperienceRequirement { get; set; } = 100;
	
	/// <summary>
	/// If this is true, the actor will automatically spend stat points gained from its level ups.
	/// Experimental
	/// TODO: implement this i guess
	/// </summary>
	private bool AutoApplyLevelUps { get; set; }
	
	public void AddExperience( float exp )
	{
		Experience += exp;

		if ( Experience > ExperienceRequirement )
		{
			Level += (Experience / ExperienceRequirement).FloorToInt();
		}
		
		Experience %= ExperienceRequirement;
	}
}
