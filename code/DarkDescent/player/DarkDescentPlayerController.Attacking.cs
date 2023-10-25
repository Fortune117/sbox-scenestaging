using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController
{
	[Property]
	private CarriedItemComponent CarriedItemComponent { get; set; }
	
	private static class PlayerEvents
	{
		public const string AttackStartEvent = "AttackStartEvent";
		public const string AttackEndEvent = "AttackEndEvent";
	}
	
	private void OnGenericAnimEvent( SceneModel.GenericEvent genericEvent )
	{
		switch ( genericEvent.Type )
		{
			case PlayerEvents.AttackStartEvent:
				OnAttackStart();
				break;
			case PlayerEvents.AttackEndEvent:
				OnAttackEnd();
				break;
		}
	}

	private void OnAttackStart()
	{
		CarriedItemComponent.BeginAttack();
	}

	private void OnAttackEnd()
	{
		CarriedItemComponent.EndAttack();
	}
}
