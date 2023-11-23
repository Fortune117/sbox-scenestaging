using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class StatusEffectManagerComponent : BaseComponent
{
	[Property]
	public ActorComponent ActorComponent { get; private set; }
	
	[Property]
	public GameObject StatusEffectPrefab { get; set; }
	
	private readonly List<StatusEffectComponent> StatusEffects = new();

	public override void OnAwake()
	{
		base.OnAwake();

		if ( StatusEffectPrefab is not null )
			AddStatusEffect( StatusEffectPrefab );
	}

	public void AddStatusEffect(GameObject statusEffectPrefab)
	{
		var obj = SceneUtility.Instantiate( statusEffectPrefab, Transform.Position, Transform.Rotation );
		obj.SetParent( GameObject );
		
		var statusEffect = obj.GetComponent<StatusEffectComponent>();
		
		statusEffect.Apply( this );
	}

	public void RemoveStatusEffect( StatusEffectComponent statusEffectComponent )
	{
		StatusEffects.Remove( statusEffectComponent );
		
		statusEffectComponent.OnRemoved();
		statusEffectComponent.GameObject.Destroy();
	}
}
