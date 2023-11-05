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
	
	[Property]
	private SoundEvent BlockSound { get; set; }

	private bool isActive;

	public Action<DamageEventData> OnBlock;

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

	public void BlockedHit(DamageEventData damageEvent)
	{
		OnBlock?.Invoke(damageEvent);
		
		var capsule = CarriedItemComponent.GetHurtBoxCapsule();
		
		var line = new Line(capsule.CenterA, capsule.CenterB);

		ParticleSystem.Transform.Position = line.ClosestPoint( damageEvent.Position );

		var dir = damageEvent.Direction;
		var angles = (Rotation.LookAt( dir ) * Rotation.FromPitch( 90 )).Angles();
		
		if (!ParticleSystem.Enabled)
			ParticleSystem.Enabled = true;
		else 
			ParticleSystem.PlayEffect();
		
		ParticleSystem.Set( "Normal", dir );
		ParticleSystem.Set("RingPitch", angles.pitch  );
		ParticleSystem.Set("RingYaw", angles.yaw  );
		ParticleSystem.Set("RingRoll", angles.roll  );

		Sound.FromWorld( BlockSound.ResourceName, ParticleSystem.Transform.Position );
	}
}
