using System.Collections.Generic;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public class CarriedWeaponComponent : CarriedItemComponent
{
	/// <summary>
	/// Wind up time of this weapon in seconds.
	/// </summary>
	[Property, Range(0, 3)]
	public float WindupTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon stays active.
	/// </summary>
	[Property, Range(0, 3)]
	public float ReleaseTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to combo after the release window is finished.
	/// </summary>
	[Property, Range(0, 3)]
	public float ComboTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to recover after a swing.
	/// </summary>
	[Property, Range(0, 3)]
	public float RecoveryTime { get; set; }
	
	[Property]
	private HurtBoxComponent HurtBox { get; set; }
	
	public virtual PhysicsTraceResult GetWeaponTrace()
	{
		return HurtBox.PerformTrace();
	}

	public virtual Vector3 GetImpactDirection()
	{
		return HurtBox.DirectionMoment;
	}
}
