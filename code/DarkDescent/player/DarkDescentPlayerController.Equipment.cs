using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	[Property]
	private CarriedItemComponent CarriedItemComponent { get; set; }
	
	public void EquipItem( CarriedItemComponent carriedItemComponent )
	{
		CarriedItemComponent = carriedItemComponent;

		carriedItemComponent.PlayerController = this;
		carriedItemComponent.GameObject.SetParent( GameObject );
		carriedItemComponent.AnimatedModelComponent.BoneMergeTarget = Body;
		carriedItemComponent.OnEquipped();
	}
}
