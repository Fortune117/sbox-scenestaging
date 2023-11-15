using DarkDescent.Actor;
using Sandbox;

namespace DarkDescent.Components;

public interface IInteractable
{
	public enum InteractionType
	{
		Use,
		Pickup,
		Equip
	}
	
	public InteractionType InteractType { get; set; }
	
	public GameObject GameObject { get; }
	public GameTransform Transform { get; }

	public void InteractionUpdate();

	public bool CanInteract(DarkDescentPlayerController playerController);
	
	public void Interact(DarkDescentPlayerController playerController, PhysicsTraceResult tr );
}

