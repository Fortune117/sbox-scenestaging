using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	public float Stamina { get; private set; }
	
	private TimeSince TimeSinceLastStaminaCost { get; set; }

	public float StaminaFraction => (Stamina / Stats.MaxStamina).Clamp( 0, 1f );

	public bool CanStartRun => StaminaFraction > 0.1f;
	public bool CanRun => StaminaFraction > 0f;
	


	public void PayStamina( float amount )
	{
		Stamina -= amount * Stats.StaminaCostMultiplier;
		Stamina = Stamina.Clamp( 0, Stats.MaxStamina );
		TimeSinceLastStaminaCost = 0;
	}

	public void AddStamina( float amount )
	{
		Stamina += amount;
		Stamina = Stamina.Clamp( 0, Stats.MaxStamina );
	}

	public bool CanAffordStaminaCost( float amount )
	{
		return GetStaminaCost( amount ) < Stamina;
	}

	public float GetStaminaCost( float amount )
	{
		return amount * Stats.StaminaCostMultiplier;
	}
	
	private void OnMaxStaminaChanged( float oldMaxStaminaBonus, float newMaxStaminaBonus )
	{
		var oldMaxStamina = Stats.GetMaxStaminaForMaxStaminaBonus( oldMaxStaminaBonus );
		var frac = Stamina / oldMaxStamina;

		frac = frac.Clamp( 0, 1 );

		Stamina = Stats.MaxStamina * frac;
	}
}
