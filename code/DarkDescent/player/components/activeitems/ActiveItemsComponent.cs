using System;
using Sandbox;

namespace DarkDescent;

/// <summary>
/// This is the component in charge of the items the player is carrying.
/// Can have an item in your left hand, right hand, both or neither.
/// </summary>
[Prefab]
public partial class ActiveItemsComponent : EntityComponent<Player>, ISingletonComponent
{
	public enum Handedness
	{
		None,
		Left,
		Right,
		Both
	}
	
	/// <summary>
	/// Prefab for what we start with in our right hand.
	/// Mostly just here as a placeholder.
	/// </summary>
	[Net, Prefab]
	public Prefab RightHandPrefab { get; set; }
	
	/// <summary>
	/// Prefab for what we start with in our right hand.
	/// Mostly just here as a placeholder.
	/// </summary>
	[Net, Prefab]
	public Prefab LeftHandPrefab { get; set; }
	
	[Net]
	public CarriableItem RightHandItem { get; set; }
	
	[Net]
	public CarriableItem LeftHandItem { get; set; }
	
	private CarriableItem LastRightHand { get; set; }
	private CarriableItem LastLeftHand { get; set; }
	
	/// <summary>
	/// True if the item in our right hand is two handed.
	/// Two handed items always go in the right hand slot.
	/// </summary>
	public bool IsTwoHanding { get; set; }

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Game.IsServer )
		{
			if (RightHandPrefab is not null)
				CreateActiveItem( Handedness.Right, RightHandPrefab );
			
			if (LeftHandPrefab is not null)
				CreateActiveItem( Handedness.Left, LeftHandPrefab );
		}
	}

	private void CreateActiveItem( Handedness handedness, Prefab prefab )
	{
		switch ( handedness )
		{
			case Handedness.Both:
			{
				IsTwoHanding = true;
				LeftHandItem?.Delete();
				RightHandItem?.Delete();
				RightHandItem = PrefabLibrary.Spawn<CarriableItem>( prefab );
				RightHandItem.SetParent( Entity, true );
				RightHandItem.Carrier = Entity;
				break;
			}
			case Handedness.Right:
			{
				IsTwoHanding = false;
				RightHandItem?.Delete();
				RightHandItem = PrefabLibrary.Spawn<CarriableItem>( prefab );
				RightHandItem.SetParent( Entity, true );
				RightHandItem.Carrier = Entity;
				break;
			}
			case Handedness.Left:
			{
				IsTwoHanding = false;
				LeftHandItem?.Delete();
				LeftHandItem = PrefabLibrary.Spawn<CarriableItem>( prefab );
				LeftHandItem.SetParent( Entity, true );
				LeftHandItem.Carrier = Entity;
				break;
			}
			case Handedness.None:
				break;
		}
	}

	public void Simulate(IClient client)
	{
		SimulateRightHand( client, RightHandItem );
		SimulateLeftHand( client, LeftHandItem );
	}

	private void SimulateRightHand( IClient client, CarriableItem child )
	{
		if ( Prediction.FirstTime && LastRightHand != child )
		{
			OnHeldItemChanged( LastRightHand, child );
			LastRightHand = child;
		}
		
		if ( !LastRightHand.IsValid() )
			return;

		if ( LastRightHand.IsAuthority )
		{
			LastRightHand.Simulate( client );
		}
	}

	private void SimulateLeftHand( IClient client, CarriableItem child )
	{
		if ( Prediction.FirstTime && LastLeftHand != child )
		{
			OnHeldItemChanged( LastLeftHand, child );
			LastLeftHand = child;
		}
		
		if ( !LastLeftHand.IsValid() )
			return;

		if ( LastLeftHand.IsAuthority )
		{
			LastLeftHand.Simulate( client );
		}
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	private void OnHeldItemChanged( CarriableItem previous, CarriableItem next )
	{
		previous?.CarriableItemComponent.ActiveEnd( Entity, previous.Owner != Entity );
		next?.CarriableItemComponent.ActiveStart( Entity );
	}
}
