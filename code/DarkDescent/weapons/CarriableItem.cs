using Sandbox;

namespace DarkDescent;

[Prefab]
public partial class CarriableItem : AnimatedEntity
{
	[BindComponent]
	public CarriableItemComponent CarriableItemComponent { get; }
	
	/// <summary>
	/// The player carrying this item.
	/// </summary>
	public Player Carrier { get; set; }

	public override void Simulate( IClient client )
	{
		CarriableItemComponent.Simulate( client );
	}
}
