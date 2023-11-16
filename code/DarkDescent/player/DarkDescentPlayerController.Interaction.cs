using DarkDescent.Components;
using DarkDescent.UI;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	public IInteractable ActiveInteractable { get; set; }
	
	[Property, Range(0, 100)]
	public float InteractRange { get; set; }
	
	[Property, Range(0, 100)] 
	public float HoldRange { get; set; }

	private TimeSince TimeSinceInteraction;
	
	public void InteractableUpdate()
	{
		if ( ActiveInteractable is not null )
		{
			ActiveInteractable.InteractionUpdate();

			TimeSinceInteraction = 0;
			return;
		}

		var tr = GetInteractionTrace();
		var interactable = FindInteractable(tr);

		if ( interactable is null )
		{
			Crosshair.Instance?.CrosshairInternal?.SetClass( "interact", false );
			return;
		}
		
		var canInteract = interactable.CanInteract( this );
		
		Crosshair.Instance?.CrosshairInternal?.SetClass( "interact", true );
		Crosshair.InteractPossible = canInteract;
		
		if ( !Input.Pressed( "use" ) )
			return;

		if ( canInteract )
		{
			interactable.Interact( this, tr );
			Crosshair.Instance?.CrosshairInternal?.SetClass( "interact", false );
		}
	}

	private PhysicsTraceResult GetInteractionTrace()
	{
		var tr = Physics.Trace.Ray( AimRay, InteractRange )
			.WithoutTags( "trigger", "player" )
			.Radius( 3 )
			.Run();

		return tr;
	}

	private IInteractable FindInteractable(PhysicsTraceResult tr)
	{
		if ( tr.Body?.GameObject is not GameObject gameObject )
			return null;
		
		if ( gameObject.TryGetComponent<IInteractable>(out var interactable ))
			return interactable;

		return null;
	}
}
