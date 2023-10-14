using System;
using Sandbox;
using Sandbox.Utility;

namespace DarkDescent.Cameras;

public class ViewPunch
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
        
        var rise = (-vertMagnitude * noise).Clamp(-vertMagnitude, -vertMagnitude / 2f);
        var sway = (horizMagnitude * noise2);

        if (sway > 0)
            sway = sway.Clamp(horizMagnitude / 2f, horizMagnitude);
        else
            sway = sway.Clamp(-horizMagnitude, -horizMagnitude / 2f);

        targetRotation *= new Angles(rise, sway, 0).ToRotation();
    }
    
    
    public Rotation UpdateRotation()
    {
	    //we are always spherically interpolated our target rotation back to zero, and then also interpolated our 
	    //current rotation back to zero.
	    //seems a bit wanky but it feels good in game.
	    
        targetRotation = Rotation.Slerp(targetRotation, Rotation.Identity, Time.Delta * 10f);
        currentRotation = Rotation.Slerp(currentRotation, targetRotation, Time.Delta * 10f);
        
        return currentRotation;
    }
}
