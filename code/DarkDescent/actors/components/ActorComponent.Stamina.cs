using Sandbox;

namespace DarkDescent.Actor;

public partial class ActorComponent
{
	/// <summary>
	/// Cost of sprinting in stamina per second.
	/// </summary>
	[Property, Range(0, 20), Category("Stamina")]
	public float StaminaSprintCost { get; set; }
	
	/// <summary>
	/// Stamina cost to jump.
	/// </summary>
	[Property, Range(0, 30), Category("Stamina")]
	public float StaminaJumpCost { get; set; }
	
	public float Stamina { get; private set; }
	
	public bool IsSprinting { get; set; }
	
	private TimeSince TimeSinceLastStaminaCost { get; set; }

	public float StaminaFraction => (Stamina / Stats.MaxStamina).Clamp( 0, 1f );

	public bool CanStartRun => StaminaFraction > 0.1f;
	public bool CanRun => StaminaFraction > 0f;
	
	private void UpdateStamina()
	{
		if ( IsSprinting )
		{
			PayStamina(StaminaSprintCost * Time.Delta );
		}
	}

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

	private float GetStaminaCost( float amount )
	{
		return amount * Stats.StaminaCostMultiplier;
	}

	public void OnJump()
	{
		PayStamina( StaminaJumpCost );
	}

	public bool CanAffordJump()
	{
		return Stamina >= GetStaminaCost( StaminaJumpCost );
	}
	
	public void OnJumpQueued()
	{
		
	}
	
	private void OnMaxStaminaChanged( float oldMaxStaminaBonus, float newMaxStaminaBonus )
	{
		var oldMaxStamina = Stats.GetMaxStaminaForMaxStaminaBonus( oldMaxStaminaBonus );
		var frac = Stamina / oldMaxStamina;

		frac = frac.Clamp( 0, 1 );

		Stamina = Stats.MaxStamina * frac;
	}
}
