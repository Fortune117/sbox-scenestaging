using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// Controls how our lil actor should think, act and feel about life in general.
/// </summary>
public class BehaviourComponent : BaseComponent
{
	protected static class BehaviourAnimationEvents
	{
		public const string LockRotation = "LockRotation";
		public const string UnlockRotation = "UnlockRotation";
		public const string ActivateHitBoxes = "ActivateHitBoxes";
		public const string DeactivateHitBoxes = "DeactivateHitBoxes";
		public const string FinishAttack = "FinishAttack";
	}
	
	[Property]
	protected AnimatedModelComponent Body { get; set; }
	
	protected TargetingComponent TargetingComponent { get; set; }
	protected ActorComponent ActorComponent { get; set; }

	protected TargetComponent Target => TargetingComponent.Target;
	
	protected bool RotationLocked { get; set; }

	public override void OnStart()
	{
		TargetingComponent = GetComponent<TargetingComponent>();
		ActorComponent = GetComponent<ActorComponent>();

		if ( ActorComponent is null )
		{
			Log.Error( $"No Actor Component on {GameObject.Name} for Behaviour Component!" );
		}

		HookupAnimEvents();
	}
	
	protected virtual void HookupAnimEvents()
	{
		if ( Body is null )
			return;
		
		Body.SceneObject.OnGenericEvent += OnGenericAnimEvent;
	}

	protected virtual void OnGenericAnimEvent( SceneModel.GenericEvent genericEvent )
	{
		switch ( genericEvent.Type )
		{
			case BehaviourAnimationEvents.LockRotation:
			{
				RotationLocked = true;
				break;
			}
			case BehaviourAnimationEvents.UnlockRotation:
			{
				RotationLocked = false;
				break;
			}
		}
	}
}
