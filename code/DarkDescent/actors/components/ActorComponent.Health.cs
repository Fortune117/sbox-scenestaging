using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	public float Health { get; private set; }
	
	public bool Alive { get; private set; }
	
	public TimeSince TimeSinceTookDamage { get; set; }

	public void Heal(float amount)
	{
		Health += amount * Stats.HealMultiplier;
		Health = Health.Clamp( 0, Stats.MaxHealth );
	}

	public void DamageHealth( float amount )
	{
		Health -= amount;
		Health = Health.Clamp( 0, Stats.MaxHealth );

		TimeSinceTookDamage = 0;
	}

	public void FullHeal()
	{
		Health = Stats.MaxHealth;
		Stamina = Stats.MaxStamina;
	}

	public void Kill()
	{
		/*if ( Ga is Player player )
			player.Respawn();
		else
			Entity.Delete();*/
	}

	private void OnConstitutionChanged( float oldCon, float newCon )
	{
		var oldMaxHealth = Stats.GetMaxHealthForConScore( oldCon );
		var frac = Health / oldMaxHealth;

		frac = frac.Clamp( 0, 1 );

		Health = Stats.MaxHealth * frac;
		
		var oldMaxStamina = Stats.GetMaxStaminaForConScore( oldCon );
		frac = Stamina / oldMaxStamina;

		frac = frac.Clamp( 0, 1 );

		Stamina = Stats.MaxStamina * frac;
	}

	private void OnMaxHealthChanged( float oldMaxHealthBonus, float newMaxHealthBonus )
	{
		var oldMaxHealth = Stats.GetMaxHealthForMaxHealthBonus( oldMaxHealthBonus );
		var frac = Health / oldMaxHealth;

		frac = frac.Clamp( 0, 1 );

		Health = Stats.MaxHealth * frac;
	}

	private void OnDeath(DamageEventData damageEventData)
	{
		foreach ( var deathListener in Components.GetAll<IDeathListener>() )
		{
			deathListener.OnDeath( damageEventData );
		}

		foreach ( var behaviour in Components.GetAll<BehaviourComponent>() )
		{
			behaviour.Enabled = false;
		}
		
		if ( !Components.TryGet<ModelCollider>( out var modelCollider, FindMode.EnabledInSelfAndDescendants ) )
			return;

		modelCollider.Enabled = false;
		
		if ( !Components.TryGet<ModelPhysics>( out var modelPhysics, FindMode.EnabledInSelfAndDescendants ) )
			return;

		modelPhysics.GameObject.Tags.Add( "corpse" );
		modelPhysics.Enabled = true;
		//modelPhysics.SetBonesToRenderer();
	}

	public void Revive()
	{
		Health = Stats.MaxHealth;
		Alive = true;
		
		if ( Components.TryGet<ModelCollider>( out var modelCollider, FindMode.EverythingInSelfAndDescendants ) )	
			modelCollider.Enabled = true;

		if ( Components.TryGet<ModelPhysics>( out var modelPhysics, FindMode.EverythingInSelfAndDescendants ) )
		{
			modelPhysics.GameObject.Tags.Remove( "corpse" );
			modelPhysics.Enabled = false;
		}

		
		foreach ( var behaviour in Components.GetAll<BehaviourComponent>(FindMode.EverythingInSelfAndDescendants) )
		{
			behaviour.Enabled = true;
		}
	}
}
