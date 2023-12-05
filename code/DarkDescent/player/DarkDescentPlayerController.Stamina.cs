using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	/// <summary>
	/// Cost of sprinting in stamina per second.
	/// </summary>
	[Property, Range(0, 20), ToggleGroup("Stamina")]
	public float StaminaSprintCost { get; set; }
	
	/// <summary>
	/// Stamina cost to jump.
	/// </summary>
	[Property, Range(0, 30), ToggleGroup("Stamina")]
	public float StaminaJumpCost { get; set; }
	
	public bool IsSprinting { get; set; }
	
	private void UpdateStamina()
	{
		if ( IsSprinting )
		{
			ActorComponent.PayStamina(StaminaSprintCost * Time.Delta );
		}
	}
	
	public void OnJump()
	{
		ActorComponent.PayStamina( StaminaJumpCost );
	}

	public bool CanAffordJump()
	{
		return ActorComponent.Stamina >= ActorComponent.GetStaminaCost( StaminaJumpCost );
	}
	
	public void OnJumpQueued()
	{
		
	}
}
