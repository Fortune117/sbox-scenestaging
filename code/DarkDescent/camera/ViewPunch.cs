﻿using System;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;
using Sandbox.Utility;

namespace DarkDescent.Cameras;

[Title( "View Punch" )]
[Category( "Camera" )]
[Icon( "cameraswitch" )]
public class ViewPunch : Component, CameraComponent.ISceneCameraSetup, IDamageTakenListener, IBlockListener
{
    private Rotation targetRotation = Rotation.Identity;
    private Rotation currentRotation = Rotation.Identity;

    public void Add(float vertMagnitude, float horizMagnitude, bool randomHorizontalDirection)
    {        
        var noise = Noise.Perlin(Time.Now * 60f);
        var noise2 = Noise.Perlin(Time.Now * 60f);

        if (randomHorizontalDirection)
        {
            noise2 -= 0.5f;
            noise2 *= 2f;
        }

        var rise = vertMagnitude * noise;
        var sway = horizMagnitude * noise2;
        
        targetRotation *= new Angles(rise, sway, 0).ToRotation();
    }
    
    public void SetupCamera( CameraComponent camera, SceneCamera sceneCamera )
    {
	    
	    //we are always spherically interpolated our target rotation back to zero, and then also interpolated our 
	    //current rotation back to zero.
	    //seems a bit wanky but it feels good in game.
	    targetRotation = Rotation.Slerp(targetRotation, Rotation.Identity, Time.Delta * 10f);
	    currentRotation = Rotation.Slerp(currentRotation, targetRotation, Time.Delta * 10f);

	    sceneCamera.Rotation *= currentRotation;
    }


    private float verticalImpact => 90f;
    private float horizontalImpact => 120f;
    
    public void OnDamageTaken( DamageEventData damageEvent, bool isLethal )
    {
	    if ( isLethal || damageEvent.WasBlocked || damageEvent.HasFlag( DamageFlags.NoFlinch ))
		    return;

	    var vert = -damageEvent.Direction.Dot( Transform.Rotation.Up );
		var horizontal = damageEvent.Direction.Dot( Transform.Rotation.Left );
	    
	    Add( vert * verticalImpact, horizontal * horizontalImpact, false );
    }

    public void OnBlock( DamageEventData damageEvent, bool isParry )
    {
	    var vert = -damageEvent.Direction.Dot( Transform.Rotation.Up );
	    var horizontal = damageEvent.Direction.Dot( Transform.Rotation.Left );
	    
	    Add( vert * verticalImpact/2f, horizontal * horizontalImpact/2f, false );
    }
}
