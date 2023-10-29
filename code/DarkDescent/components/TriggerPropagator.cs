namespace DarkDescent.Components;

public class TriggerPropagator : BaseComponent, BaseComponent.ITriggerListener
{
	public void OnTriggerEnter( Collider other )
	{
		if ( GameObject.Parent == Scene )
			return;
		
		foreach ( var trigger in GameObject.Parent.GetComponents<ITriggerListener>() )
		{
			trigger.OnTriggerEnter( other );
		}
	}

	public void OnTriggerExit( Collider other )
	{
		if ( GameObject.Parent == Scene )
			return;
		
		foreach ( var trigger in GameObject.Parent.GetComponents<ITriggerListener>() )
		{
			trigger.OnTriggerExit( other );
		}
	}
}
