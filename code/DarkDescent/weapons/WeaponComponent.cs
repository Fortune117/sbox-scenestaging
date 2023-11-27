using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using DarkDescent.Components;
using Sandbox;

namespace DarkDescent.Items;

public partial class WeaponComponent : ItemComponent, IDamageInflictor
{
	[Property, ToggleGroup("Combat")]
	public DamageResource DamageResource { get; set; }
	
	[Property, ToggleGroup("Combat")]
	public BlockingResource BlockResource { get; set; }
	
	[Property, Range( 0, 5 ), ToggleGroup("Combat")] 
	public float KnockBackMultiplier { get; set; } = 1f;
	
	/// <summary>
	/// Wind up time of this weapon in seconds.
	/// </summary>
	[Property, Range(0, 3), ToggleGroup("Swing Control")]
	public float WindupTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon stays active.
	/// </summary>
	[Property, Range(0, 3), ToggleGroup("Swing Control")]
	public float ReleaseTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to combo after the release window is finished.
	/// </summary>
	[Property, Range(0, 3), ToggleGroup("Swing Control")]
	public float ComboTime { get; set; }
	
	/// <summary>
	/// How long in seconds this weapon takes to recover after a swing.
	/// </summary>
	[Property, Range(0, 3), ToggleGroup("Swing Control")]
	public float RecoveryTime { get; set; }

	[Property, Range( 0, 500 ), ToggleGroup("Swing Control")] 
	public float TurnCapX { get; set; } = 2f;
	
	[Property, Range(0, 500), ToggleGroup("Swing Control")]
	public float TurnCapY { get; set; } = 2f;

	/// <summary>
	/// How far the trace has to be before the attack no longer bounces upon hitting a solid surface.
	/// </summary>
	[Property, Range( 0, 1 ), ToggleGroup("Swing Control")]
	public float BounceFraction { get; set; } = 1f;
	
	[Property, ToggleGroup("Effects")]
	public SoundEvent ImpactSound { get; set; }
	
	[Property, ToggleGroup("Effects")]
	public SoundEvent SwingSound { get; set; }
	
	[Property, ToggleGroup("Effects")]
	public SwordTrail SwordTrail { get; set; }
	
	
	[Property, ToggleGroup("Effects")]
	private ParticleSystem ScrapeParticles { get; set; }
	
	[Property] 
	private HurtBoxComponent HurtBox { get; set; }

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

	public override void Interact( DarkDescentPlayerController playerController, PhysicsTraceResult tr )
	{
		playerController.EquipItem( this ); 
	}

	public void StartAttackTrail()
	{
		SwordTrail?.StartTrail();
	}

	public void StopAttackTrail()
	{
		SwordTrail?.StopTrail();
	}
}
