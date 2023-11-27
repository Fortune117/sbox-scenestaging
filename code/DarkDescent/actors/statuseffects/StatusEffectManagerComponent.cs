using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class StatusEffectManagerComponent : BaseComponent
{
	public ActorComponent ActorComponent { get; private set; }
	
	
	private readonly List<StatusEffectComponent> StatusEffects = new();

	public override void OnAwake()
	{
		base.OnAwake();

		ActorComponent = GetComponentInParent<ActorComponent>( false, true );
	}

	public void AddStatusEffect(GameObject statusEffectPrefab, ActorComponent originator)
	{
		var obj = SceneUtility.Instantiate( statusEffectPrefab, Transform.Position, Transform.Rotation );
		
		var statusEffect = obj.GetComponent<StatusEffectComponent>();

		if ( statusEffect is null )
		{
			Log.Error( $"Tried to add status effect with no status effect component: {obj.PrefabInstanceSource}" );
			obj.DestroyImmediate();
			return;
		}

		if ( TryApplyingStatus( statusEffect, originator ) )
		{
			obj.DestroyImmediate();
			return;
		}

		obj.SetParent( GameObject );
		statusEffect.Originator = originator;
		statusEffect.Apply( this );
		
		StatusEffects.Add( statusEffect );
	}

	private bool TryApplyingStatus(StatusEffectComponent statusEffectComponent, ActorComponent originator)
	{
		var existingStatus = StatusEffects.Find( x => x.EffectID.Equals(statusEffectComponent.EffectID) );
		if ( existingStatus is not null )
		{
			if ( existingStatus.CanStack )
			{
				if (existingStatus.CanAddStack )
					existingStatus.AddStacks( 1 );

				if ( existingStatus.StacksResetDuration )
					existingStatus.ResetDuration();

				return true;
			}

			//we replace our status effect with the new one
			if ( existingStatus.IsUnique )
			{
				RemoveStatusEffect( existingStatus );
			}
		}

		return false;
	}

	public void RemoveStatusEffect( StatusEffectComponent statusEffectComponent )
	{
		StatusEffects.Remove( statusEffectComponent );
		
		statusEffectComponent.OnRemoved();
		statusEffectComponent.GameObject.Destroy();
	}
}
