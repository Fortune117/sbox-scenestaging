using System.Collections.Generic;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Components;
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

	[Property, Range( 0, 500 )] 
	public float TurnCapX { get; set; } = 2f;
	
	[Property, Range(0, 500)]
	public float TurnCapY { get; set; } = 2f;

	/// <summary>
	/// How far the trace has to be before the attack no longer bounces upon hitting a solid surface.
	/// </summary>
	[Property, Range( 0, 1 )]
	public float BounceFraction { get; set; } = 1f;
	
	[Property]
	public SoundEvent ImpactSound { get; set; }
	
	[Property]
	public SoundEvent SwingSound { get; set; }
	
	[Property]
	public SwordTrail SwordTrail { get; set; }
	
	[Property]
	public BlockingResource BlockResource { get; set; }
	
	[Property] 
	private HurtBoxComponent HurtBox { get; set; }

	public float GetDamage( ActorComponent actorComponent )
	{
		return actorComponent.Stats.PhysicalPower;
	}

	public DamageType GetDamageType()
	{
		return DamageType.Physical;
	}
	
	public Capsule GetHurtBoxCapsule()
	{
		return HurtBox.Capsule;
	}
	
	public virtual PhysicsTraceResult GetWeaponTrace()
	{
		return HurtBox.PerformTrace();
	}

	public virtual Vector3 GetImpactDirection()
	{
		return HurtBox.DirectionMoment;
	}
}
