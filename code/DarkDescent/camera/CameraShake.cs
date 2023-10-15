using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Cameras;

[Title( "Camera Shake" )]
[Category( "Camera" )]
[Icon( "earthquake" )]
public class CameraShake : BaseComponent, CameraComponent.ISceneCameraSetup
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
}
