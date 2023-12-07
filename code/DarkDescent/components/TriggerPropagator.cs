namespace DarkDescent.Components;

public class TriggerPropagator : Component, Component.ITriggerListener
{
	public void OnTriggerEnter( Collider other )
	{
		if ( GameObject.Parent == Scene )
			return;
		
		foreach ( var trigger in Components.GetAll<ITriggerListener>( FindMode.InAncestors ) )
		{
			trigger.OnTriggerEnter( other );
		}
	}

	public void OnTriggerExit( Collider other )
	{
		if ( GameObject.Parent == Scene )
			return;
		
		foreach ( var trigger in Components.GetAll<ITriggerListener>( FindMode.InAncestors ) )
		{
			trigger.OnTriggerExit( other );
		}
	}
}
