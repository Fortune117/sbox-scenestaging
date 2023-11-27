using DarkDescent.Components;
using Sandbox;

namespace DarkDescent.Items;

public partial class ItemComponent : IInteractable
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
