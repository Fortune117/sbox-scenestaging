using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	[Property]
	private CarriedItemComponent RightHandItem { get; set; }
	
	[Property]
	private CarriedItemComponent LeftHandItem { get; set; }
	
	[Property]
	private GameObject HoldRight { get; set; }
	
	[Property]
	private GameObject HoldLeft { get; set; }
	
	public void EquipItem( CarriedItemComponent carriedItemComponent )
	{
		if ( carriedItemComponent.Handedness is not Handedness.Left )
		{
			RightHandItem = carriedItemComponent;
			carriedItemComponent.GameObject.SetParent( HoldRight, false );
		}
		else
		{
			LeftHandItem = carriedItemComponent;
			carriedItemComponent.GameObject.SetParent( HoldLeft, false );
		}

		carriedItemComponent.PlayerController = this;
		//carriedItemComponent.AnimatedModelComponent.BoneMergeTarget = Body;
		carriedItemComponent.OnEquipped();
	}
}
