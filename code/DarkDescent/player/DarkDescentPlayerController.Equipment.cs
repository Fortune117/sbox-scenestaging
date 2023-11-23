using DarkDescent.Items;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	[Property]
	private ItemComponent RightHandItem { get; set; }
	
	[Property]
	private ItemComponent LeftHandItem { get; set; }
	
	[Property]
	private GameObject HoldRight { get; set; }
	
	[Property]
	private GameObject HoldLeft { get; set; }
	
	public void EquipItem( ItemComponent itemComponent )
	{
		if ( itemComponent.Handedness is not Handedness.Left )
		{
			RightHandItem = itemComponent;
			itemComponent.GameObject.SetParent( HoldRight, false );
		}
		else
		{
			LeftHandItem = itemComponent;
			itemComponent.GameObject.SetParent( HoldLeft, false );
		}

		itemComponent.PlayerController = this;
		//carriedItemComponent.AnimatedModelComponent.BoneMergeTarget = Body;
		itemComponent.OnEquipped();
	}
}
