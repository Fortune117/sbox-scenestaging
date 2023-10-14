using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	public float Health { get; private set; }
	
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
}
