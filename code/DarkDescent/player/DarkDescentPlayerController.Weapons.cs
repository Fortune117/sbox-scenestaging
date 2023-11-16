using DarkDescent.Weapons;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	public void EquipItem( CarriedWeaponComponent carriedWeaponComponent )
	{
		CarriedItemComponent = carriedWeaponComponent;

		carriedWeaponComponent.PlayerController = this;
		carriedWeaponComponent.GameObject.SetParent( GameObject );
		carriedWeaponComponent.AnimatedModelComponent.BoneMergeTarget = Body;
		carriedWeaponComponent.OnEquipped();
	}
}
