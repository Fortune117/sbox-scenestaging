using DarkDescent.Items;
using Sandbox;

namespace DarkDescent.Actor.Damage;

public class AttackBlockerComponent : BaseComponent
{
	[Property]
	public ActorComponent BlockOwner { get; set; }
	
	[Property]
	private ColliderBoxComponent Collider { get; set; }
	
	[Property]
	private ParticleSystem ParticleSystem { get; set; }
	
	[Property]
	private WeaponComponent ItemComponent { get; set; }
	
	public Action<DamageEventData, bool> OnBlock;

	private TimeSince TimeSinceBlockStarted;
	private float baseParryWindow => 0.25f;
	private float parryWindowAddition => 0.25f;
	private float parryWindow;
	
	private bool isActive;

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
		var oldBlock = isActive;
		IsActive = status;

		if ( !oldBlock && status )
		{
			TimeSinceBlockStarted = 0;
			parryWindow = baseParryWindow;
		}

	}

	public DamageEventData BlockedHit(DamageEventData damageEvent)
	{
		var isParry = TimeSinceBlockStarted < parryWindow;

		if ( isParry )
		{
			parryWindow += parryWindowAddition;
		}
		
		var capsule = ItemComponent.GetHurtBoxCapsule();
		
		var line = new Line(capsule.CenterA, capsule.CenterB);

		ParticleSystem.Transform.Position = line.ClosestPoint( damageEvent.Position );

		ParticleSystem.Particles = isParry ? ItemComponent.BlockResource.ParryEffect : ItemComponent.BlockResource.BlockEffect;

		var dir = -damageEvent.Direction;
		var angles = (Rotation.LookAt( dir ) * Rotation.FromPitch( 90 )).Angles();
		
		if (!ParticleSystem.Enabled)
			ParticleSystem.Enabled = true;
		else 
			ParticleSystem.PlayEffect();
		
		ParticleSystem.Set( "Normal", dir );
		ParticleSystem.Set("RingPitch", angles.pitch  );
		ParticleSystem.Set("RingYaw", angles.yaw  );
		ParticleSystem.Set("RingRoll", angles.roll  );

		Sound.FromWorld( isParry ? ItemComponent.BlockResource.ParrySound.ResourceName : ItemComponent.BlockResource.BlockSound.ResourceName, ParticleSystem.Transform.Position );

		if ( isParry )
		{
			ApplyParry( ref damageEvent );
		}
		else
		{
			ApplyBlock( ref damageEvent );
		}
		
		OnBlock?.Invoke(damageEvent, isParry);

		return damageEvent;
	}

	private void ApplyParry( ref DamageEventData damageEventData )
	{
		damageEventData.DamageResult = -1f;
	}
	
	/// <summary>
	/// Apply damage negation to our damage.
	/// If the damage is multi-typed, we use our weakest block value.
	/// </summary>
	/// <param name="damageEventData"></param>
	private void ApplyBlock( ref DamageEventData damageEventData )
	{
		var mult = 1f;
		
        if (damageEventData.HasDamageType(DamageType.Physical))
        {
	        mult = 1 - ItemComponent.BlockResource.PhysicalNegation;
        }

        var wardingMult = 1 - ItemComponent.BlockResource.MagicalNegation;
        if (damageEventData.HasDamageType(DamageType.Magical) && wardingMult < mult)
        {
	        mult = wardingMult;
        }

        var fireMult = 1 - ItemComponent.BlockResource.FireNegation;
        if (damageEventData.HasDamageType(DamageType.Fire) && fireMult < mult)
        {
	        mult = fireMult;
        }

        var frostMult = 1 - ItemComponent.BlockResource.FrostNegation;
        if (damageEventData.HasDamageType(DamageType.Frost) && frostMult < mult)
        {
	        mult = frostMult;
        }

        var electricMult = 1 - ItemComponent.BlockResource.ElectricNegation;
        if (damageEventData.HasDamageType(DamageType.Electric) && electricMult < mult)
        {
	        mult = electricMult;
        }

        var poisonMult = 1 - ItemComponent.BlockResource.PoisonNegation;
        if (damageEventData.HasDamageType(DamageType.Poison) && poisonMult < mult)
        {
	        mult = poisonMult;
        }

        var necroticMult = 1 - ItemComponent.BlockResource.NecroticNegation;
        if (damageEventData.HasDamageType(DamageType.Necrotic) && necroticMult < mult)
        {
	        mult = necroticMult;
        }

        var arcaneMult = 1 - ItemComponent.BlockResource.ArcaneNegation;
        if (damageEventData.HasDamageType(DamageType.Arcane) && arcaneMult < mult)
        {
	        mult = arcaneMult;
        }

        var divineMult = 1 - ItemComponent.BlockResource.DivineNegation;
        if (damageEventData.HasDamageType(DamageType.Divine) && divineMult < mult)
        {
	        mult = divineMult;
        }

        var occultMult = 1 - ItemComponent.BlockResource.OccultNegation;
        if (damageEventData.HasDamageType(DamageType.Occult) && occultMult < mult)
        {
	        mult = occultMult;
        }

        var knockBackMult = 1 - ItemComponent.BlockResource.KnockBackNegation;

        damageEventData.DamageResult *= mult;
        damageEventData.KnockBackResult *= knockBackMult;
	}
}
