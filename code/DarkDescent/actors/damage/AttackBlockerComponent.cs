using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent.Actor.Damage;

public class AttackBlockerComponent : BaseComponent
{
	[Property]
	private ColliderBoxComponent Collider { get; set; }
	
	[Property]
	private ParticleSystem ParticleSystem { get; set; }
	
	[Property]
	private CarriedWeaponComponent CarriedItemComponent { get; set; }

	private bool isActive;

	public Action OnBlock;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		private set
		{
			if ( isActive == value )
				return;

			isActive = value;
			Collider.Enabled = value;
		}
	}

	public void SetActive( bool status )
	{
		IsActive = status;
	}

	public void BlockedHit(AttackHitEvent hitEvent)
	{
		OnBlock?.Invoke();

		ParticleSystem.Transform.Position = hitEvent.TraceResult.HitPosition;

		var capsule = CarriedItemComponent.GetHurtBoxCapsule();
		
		var line = new Line(capsule.CenterA, capsule.CenterB);

		ParticleSystem.Transform.Position = line.ClosestPoint( hitEvent.TraceResult.HitPosition );

		if (!ParticleSystem.Enabled)
			ParticleSystem.Enabled = true;
		else 
			ParticleSystem.PlayEffect();
	}
	
	//code for normal and other stuff
	/*
	 	var dir = -damageEventData.Direction.Normal;
		var angles = (Rotation.LookAt( dir ) * Rotation.FromPitch( 90 )).Angles();

		foreach ( var damageEffectInfo in DamageEffectInfos )
		{
			if ( damageEventData.Tags.Has( damageEffectInfo.HitboxTag  ) )
			{
				var particle = Particles.Create( damageEffectInfo.ParticleEffect, damageEventData.Position + dir * 5f );
				particle.Set( "Normal", dir );
				particle.Set("RingPitch", angles.pitch  );
				particle.Set("RingYaw", angles.yaw  );
				particle.Set("RingRoll", angles.roll  );

				Sound.FromWorld( damageEffectInfo.ImpactSound, damageEventData.Position );
				return;
			}
		}
	 */
}
