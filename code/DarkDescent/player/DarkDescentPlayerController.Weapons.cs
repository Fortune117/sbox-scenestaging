using DarkDescent.Weapons;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	public void EquipWeapon( CarriedWeaponComponent carriedWeaponComponent )
	{
		CarriedItemComponent = carriedWeaponComponent;
		
		carriedWeaponComponent.GameObject.SetParent( GameObject );
		carriedWeaponComponent.AnimatedModelComponent.BoneMergeTarget = Body;
		carriedWeaponComponent.OnEquipped();
	}
}
