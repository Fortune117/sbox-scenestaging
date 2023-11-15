using DarkDescent.Components;
using Sandbox;

namespace DarkDescent.Weapons;

public partial class CarriedItemComponent : IInteractable
{
	public bool Interactable { get; set; }
	
	public IInteractable.InteractionType InteractType { get; set; } = IInteractable.InteractionType.Equip;
	
	public void InteractionUpdate() { }

	public bool CanInteract( DarkDescentPlayerController playerController )
	{
		return Interactable;
	}

	public virtual void Interact( DarkDescentPlayerController playerController, PhysicsTraceResult tr )
	{
		
	}
}
