using Sandbox;

namespace DarkDescent.Actor.Damage;

public class AttackBlockerComponent : BaseComponent
{
	[Property]
	private ColliderSphereComponent Collider { get; set; }

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

	public void BlockedHit()
	{
		OnBlock?.Invoke();
	}
}
