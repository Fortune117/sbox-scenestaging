using System;
using Sandbox;

namespace DarkDescent;

/// <summary>
/// This is the component in charge of the items the player is carrying.
/// Can have an item in your left hand, right hand, both or neither.
/// </summary>
public partial class ActiveItemsComponent : BaseComponent
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
	[Property]
	public GameObject RightHandPrefab { get; set; }
	
	/// <summary>
	/// Prefab for what we start with in our right hand.
	/// Mostly just here as a placeholder.
	/// </summary>
	[Property]
	public GameObject LeftHandPrefab { get; set; }
	
	public GameObject RightHandItem { get; set; }
	
	public GameObject LeftHandItem { get; set; }
	
	private GameObject LastRightHand { get; set; }
	private GameObject LastLeftHand { get; set; }
	
	/// <summary>
	/// True if the item in our right hand is two handed.
	/// Two handed items always go in the right hand slot.
	/// </summary>
	public bool IsTwoHanding { get; set; }

	public override void OnEnabled()
	{
		if (RightHandPrefab is not null)
			CreateActiveItem( Handedness.Right, RightHandPrefab );
			
		if (LeftHandPrefab is not null)
			CreateActiveItem( Handedness.Left, LeftHandPrefab );
	}

	private void CreateActiveItem( Handedness handedness, GameObject gameObject )
	{
		/*switch ( handedness )
		{
			case Handedness.Both:
			{
				IsTwoHanding = true;
				LeftHandItem?.Delete();
				RightHandItem?.Delete();
				RightHandItem = PrefabLibrary.Spawn<CarriableItem>( prefab );
				RightHandItem.SetParent( GameObject );
				RightHandItem.Carrier = GameObject;
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
		}*/
	}

	public void Simulate(IClient client)
	{
		SimulateRightHand( client, RightHandItem );
		SimulateLeftHand( client, LeftHandItem );
	}

	private void SimulateRightHand( IClient client, GameObject child )
	{
		if ( Prediction.FirstTime && LastRightHand != child )
		{
			OnHeldItemChanged( LastRightHand, child );
			LastRightHand = child;
		}
		
		if ( LastRightHand is null )
			return;

		/*if ( LastRightHand.IsAuthority )
		{
			LastRightHand.Simulate( client );
		}*/
	}

	private void SimulateLeftHand( IClient client, GameObject child )
	{
		if ( Prediction.FirstTime && LastLeftHand != child )
		{
			OnHeldItemChanged( LastLeftHand, child );
			LastLeftHand = child;
		}
		
		if ( LastLeftHand is null )
			return;

		/*if ( LastLeftHand.IsAuthority )
		{
			LastLeftHand.Simulate( client );
		}*/
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	private void OnHeldItemChanged( GameObject previous, GameObject next )
	{
		//previous?.CarriableItemComponent.ActiveEnd( Entity, previous.Owner != Entity );
		//next?.CarriableItemComponent.ActiveStart( Entity );
	}
}
