using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Components;
using Sandbox;

namespace DarkDescent.Weapons;

public class CarriedWeaponComponent : CarriedItemComponent
{
	[Property, Group("Combat")]
	public DamageResource DamageResource { get; set; }
	
	[Property, Group("Combat")]
	public BlockingResource BlockResource { get; set; }
	
	[Property, Range( 0, 5 ), Group("Combat")] 
	public float KnockBackMultiplier { get; set; } = 1f;
	
	/// <summary>
	/// Wind up time of this weapon in seconds.
	/// </summary>
	[Property, Range(0, 3), Group("Swing Control")]
	public float WindupTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon stays active.
	/// </summary>
	[Property, Range(0, 3), Group("Swing Control")]
	public float ReleaseTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to combo after the release window is finished.
	/// </summary>
	[Property, Range(0, 3), Group("Swing Control")]
	public float ComboTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to recover after a swing.
	/// </summary>
	[Property, Range(0, 3), Group("Swing Control")]
	public float RecoveryTime { get; set; }

	[Property, Range( 0, 500 ), Group("Swing Control")] 
	public float TurnCapX { get; set; } = 2f;
	
	[Property, Range(0, 500), Group("Swing Control")]
	public float TurnCapY { get; set; } = 2f;

	/// <summary>
	/// How far the trace has to be before the attack no longer bounces upon hitting a solid surface.
	/// </summary>
	[Property, Range( 0, 1 ), Group("Swing Control")]
	public float BounceFraction { get; set; } = 1f;
	
	[Property, Group("Effects")]
	public SoundEvent ImpactSound { get; set; }
	
	[Property, Group("Effects")]
	public SoundEvent SwingSound { get; set; }
	
	[Property, Group("Effects")]
	public SwordTrail SwordTrail { get; set; }
	
	
	[Property, Group("Effects")]
	private ParticleSystem ScrapeParticles { get; set; }
	
	[Property] 
	private HurtBoxComponent HurtBox { get; set; }
	
	public AnimatedModelComponent WeaponModel { get; set; }

	public override void OnStart()
	{
		base.OnStart();

		WeaponModel = GetComponent<AnimatedModelComponent>();
	}

	public float GetDamage( ActorComponent actorComponent )
	{
		return DamageResource.Calculate( actorComponent.Stats );
	}

	public DamageType GetDamageType()
	{
		return DamageResource.DamageTypes;
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

	public void PlayScrapeEffect(PhysicsTraceResult traceResult)
	{
		ScrapeParticles.Transform.Position = traceResult.EndPosition;
		ScrapeParticles.Set( "Normal", traceResult.Normal );
		ScrapeParticles.EmissionStopped = false;
		
		if (ScrapeParticles.SceneObject is null)
			ScrapeParticles.PlayEffect();
	}

	public void StopScrapeEffect()
	{
		ScrapeParticles.EmissionStopped = true; 
	}
}

public enum HoldType
{
	None,
	TwoHandedSword,
}
