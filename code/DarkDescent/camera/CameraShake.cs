using System.Collections.Generic;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Cameras;

[Title( "Camera Shake" )]
[Category( "Camera" )]
[Icon( "earthquake" )]
public class CameraShake : BaseComponent, CameraComponent.ISceneCameraSetup, IDamageTakenListener, IBlockListener
{
	private readonly List<Shake> cameraShakes = new();
	
	private Vector3 CalculateShakeOffset()
	{
		var offset = Vector3.Zero;
		for (var i = 0; i < cameraShakes.Count; i++)
		{
			var shake = cameraShakes[i];
			if (shake.IsFinished)
			{
				cameraShakes.RemoveAt(i);
				i--;
				continue;
			}
            
			offset += shake.UpdateShake();
		}

		return offset;
	}
	
	public void AddShake(float magnitude, float roughness, float fadeInDuration = 0.1f, float fadeOutDuration = 0.5f)
	{
		cameraShakes.Add(new Shake(magnitude, roughness, fadeInDuration, fadeOutDuration));
	}
	
	public void SetupCamera( CameraComponent camera, SceneCamera sceneCamera )
	{
		sceneCamera.Position += CalculateShakeOffset();
	}
	
	private float verticalImpact => 90f;
	private float horizontalImpact => 120f;
    
	public void OnDamageTaken( DamageEventData damageEvent, bool isLethal )
	{
		if ( isLethal || damageEvent.WasBlocked )
			return;
	    
		AddShake( 4f, 2f, -1f, 0.5f );
	}

	public void OnBlock( DamageEventData damageEvent, bool isParry )
	{
		AddShake( 4f, 2f, -1f, 0.5f );
	}
}
