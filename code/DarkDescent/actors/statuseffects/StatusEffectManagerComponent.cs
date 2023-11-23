using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class StatusEffectManagerComponent : BaseComponent
{
	public ActorComponent ActorComponent { get; private set; }
	
	[Property]
	public GameObject StatusEffectPrefab { get; set; }
	
	private readonly List<StatusEffectComponent> StatusEffects = new();

	public override void OnAwake()
	{
		base.OnAwake();

		ActorComponent = GetComponentInParent<ActorComponent>( false, true );
		
		if ( StatusEffectPrefab is not null )
			AddStatusEffect( StatusEffectPrefab, ActorComponent );
	}

	public void AddStatusEffect(GameObject statusEffectPrefab, ActorComponent originator)
	{
		var obj = SceneUtility.Instantiate( statusEffectPrefab, Transform.Position, Transform.Rotation );
		obj.SetParent( GameObject );
		
		var statusEffect = obj.GetComponent<StatusEffectComponent>();

		statusEffect.Originator = originator;
		statusEffect.Apply( this );
	}

	public void RemoveStatusEffect( StatusEffectComponent statusEffectComponent )
	{
		StatusEffects.Remove( statusEffectComponent );
		
		statusEffectComponent.OnRemoved();
		statusEffectComponent.GameObject.Destroy();
	}
}
