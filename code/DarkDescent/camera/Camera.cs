using System;
using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Cameras;

public abstract partial class Camera : EntityComponent<Player>
{
	[ConVar.Client]
	public bool viewbob_enabled { get; set; } = true;

	[Prefab, Net]
	public bool IsActiveCamera { get; set; }
	
    public Vector3 CurrentShake { get; set; }
    public Rotation CurrentFlinch { get; set; }
    
    public Vector3 Position { get; set; }
    public Rotation Rotation { get; set; }
    public float FieldOfView { get; set; }
    
    protected float TotalJumpTime { get; set; }
    protected float TimeUntilJump { get; set; }
    
    protected float JumpCooldown { get; set; }
    protected float TimeSinceLanded { get; set; }
    
    private readonly List<CameraShake> cameraShakes = new();
    private readonly ViewPunch viewPunch = new ViewPunch();
    
    public void FrameSimulate()
    {
        Update();
        SetupCamera();
        
        TimeUntilJump -= Time.Delta;
        TimeSinceLanded += Time.Delta;
    }

    protected virtual void Update()
    {
        var offset = CalculateShakeOffset();
        var rotation = CalculateViewPunchRotation();
        
        CurrentShake = offset;
        CurrentFlinch = rotation;
        
        Position += offset;
        Rotation *= rotation;
    }
    
    protected abstract void SetupCamera();
    
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

    private Rotation CalculateViewPunchRotation()
    {
        return Rotation.Identity * viewPunch.UpdateRotation();
    }

    public void AddShake(float magnitude, float roughness, float fadeInDuration = 0.1f, float fadeOutDuration = 0.5f)
    {
        cameraShakes.Add(new CameraShake(magnitude, roughness, fadeInDuration, fadeOutDuration));
    }

    public void AddViewPunch(float vertMagnitude, float horizontalMagnitude, bool randomHorizontalDirection = true)
    {
        viewPunch.Add(vertMagnitude, horizontalMagnitude, randomHorizontalDirection);
    }
    
    public void SnapToEyePosition()
    {
        var pawn = Game.LocalPawn;
        if ( pawn == null ) return;

        Position = pawn.AimRay.Position;
    }

    /// <summary>
    /// Called every camera build for base cameras.
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetViewBobOffset()
    {
	    return Vector3.Zero;
    }
    
    public void JumpQueued(float delay)
    {
        TotalJumpTime = delay;
        TimeUntilJump = delay;
    }
    
    public void JumpLanded(float cooldown)
    {
        if (TimeSinceLanded < JumpCooldown)
            return;
        
        JumpCooldown = cooldown;
        TimeSinceLanded = 0;
    }

}
