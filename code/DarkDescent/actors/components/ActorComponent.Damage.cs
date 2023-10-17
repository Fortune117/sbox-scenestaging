using System.Collections.Generic;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.UI;
using DarkDescent.GameLog;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	[Property, Category("Damage")]
	private List<DamageEffectInfo> DamageEffectInfos { get; set; }
	
	public void TakeDamage( DamageInfo damageInfo )
	{
		var damageEventData = DamageMaster.GenerateDamageEventData( this, damageInfo );

		var physics = GetComponent<PhysicsComponent>( true, true );
		if ( physics is not null && physics.GetBody() is not null )
		{
			ApplyKnockBack( physics.GetBody(), damageEventData );
		}
		
		/*PhysicsBody closestBody = null;
		var closestDistance = float.PositiveInfinity;
		foreach ( var body in Entity.PhysicsGroup.Bodies )
		{
			var distance = body.Position.Distance( damageInfo.Position );
			if (  distance < closestDistance )
			{
				closestBody = body;
				closestDistance = distance;
			}
		}
		
		if (closestBody is not null)
			ApplyKnockBack( closestBody, damageEventData );*/
		
		//CreateDamageEffects(damageEventData, damageInfo);
		//CreateDamageNumber(damageEventData);
		
		DamageHealth( damageEventData.DamageResult );

		GameLogSystem.DamageEventReceived( damageEventData );

		//i'm not dead yet!!
		if ( Health > 0 )
			return;

		if ( damageEventData.OriginatorActor is not null )
		{
			var exp = Game.Random.Float( 1, 100 );
			ExperienceNumberPanel.CreateExperienceNumber( exp, damageEventData );
			damageEventData.OriginatorActor.AddExperience( exp );
		}

		GameObject.Destroy();
	}

	private void CreateDamageEffects( DamageEventData damageEventData, DamageInfo damageInfo )
	{
		var dir = -damageEventData.Direction.Normal;
		var angles = (Rotation.LookAt( dir ) * Rotation.FromPitch( 90 )).Angles();

		foreach ( var damageEffectInfo in DamageEffectInfos )
		{
			if ( damageInfo.Hitbox.HasTag( damageEffectInfo.HitboxTag  ) )
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
	}

	private void ApplyKnockBack(PhysicsBody body, DamageEventData damageEventData)
	{
		/*if ( !body.IsValid() )
			return;
		
		body.ApplyForceAt( body.FindClosestPoint( damageEventData.Position ),
			damageEventData.Direction * damageEventData.KnockBack * Entity.PhysicsGroup.Mass * 100 );*/
	}
	
	public void CreateDamageNumber(DamageEventData damageEventData)
	{
		var damageNumber = DamageNumberPanel.Create( damageEventData );
	}
}

internal struct DamageEffectInfo
{
	public string HitboxTag { get; set; }
	
	[ResourceType("vpcf")]
	public string ParticleEffect { get; set; }
	
	[ResourceType("sound")]
	public string ImpactSound { get; set; }
}
