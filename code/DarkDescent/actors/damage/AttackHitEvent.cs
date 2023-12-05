using Sandbox;

namespace DarkDescent.Actor.Damage;

public struct AttackHitEvent
{
	public IDamageable Damageable { get; set; }
	public Vector3 HitDirection { get; set; }
	public SceneTraceResult TraceResult { get; set; }
	
	public AttackBlockerComponent Blocker { get; set; }
	
	public bool WasBlocked { get; set; }
	public bool HitWorld { get; set; }
}
