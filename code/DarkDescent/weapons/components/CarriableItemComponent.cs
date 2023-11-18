using Sandbox;

namespace DarkDescent;

[Prefab]
public partial class CarriableItemComponent : EntityComponent<CarriableItem>
{
	[Net, Prefab]
	protected float HolsterDelay { get; set; } = 1;
	
	[Net, Prefab] 
	protected float DeployDelay { get; set; } = 1;
	
	[Net, Predicted] 
	protected TimeSince TimeSinceDeployed { get; set; }
	
	[Net, Predicted]
	protected TimeSince TimeSincePrimaryAction { get; set; }

	[Net, Predicted]
	protected TimeSince TimeSinceSecondaryAction { get; set; }
	
	[Net, Predicted]
	protected bool IsHolstering { get; set; }
	
	[Net, Predicted]
	protected bool IsDeploying { get; set; }
	
	protected bool IsFullyDeployed => TimeSinceDeployed > DeployDelay;
	
	public void Simulate( IClient owner )
	{
		if ( CanPerformPrimaryAction() )
		{
			PerformPrimaryAction();
			return;
		}
		
		if ( CanPerformSecondaryAction() )
		{
			CanPerformSecondaryAction();
			return;
		}
	}

	public virtual bool CanPerformPrimaryAction()
	{
		if ( !Entity.Carrier.IsValid() || !Input.Down( GameTags.Input.AttackPrimary ) ) 
			return false;

		return true;
	}

	public virtual void PerformPrimaryAction()
	{
		if (Input.Down( GameTags.Input.AttackPrimary ))
			Log.Info( "Wee haa" );
	}

	public virtual bool CanPerformSecondaryAction()
	{
		if ( !Entity.Carrier.IsValid() || !Input.Down( GameTags.Input.AttackSecondary ) ) 
			return false;

		return true;
	}

	public virtual void PerformSecondaryAction()
	{

	}

	public virtual void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = CitizenAnimationHelper.HoldTypes.Pistol;
		anim.Handedness = CitizenAnimationHelper.Hand.Both;
		anim.AimBodyWeight = 1.0f;
	}

	/// <summary>
	/// This entity has become active, meaning the player is now holding in one of their hands.
	/// </summary>
	public virtual void ActiveStart( Entity newOwner )
	{
		TimeSinceDeployed = 0;
	}

	/// <summary>
	/// This entity has stopped being active.
	/// Likely means the player has swapped away from it. 
	/// </summary>
	public virtual void ActiveEnd( Entity oldOwner, bool dropped )
	{
	}
	
	public virtual void OnJump() { }

	public virtual void OnDeath() { }

	public virtual void OnFootStep() { }

	public virtual bool CanDrop()
	{
		return false;
	}

	public virtual bool CanHolster()
	{
		return true;
	}

	public virtual void OnDeployed() { }

	public virtual void OnHolstered() { }
}
