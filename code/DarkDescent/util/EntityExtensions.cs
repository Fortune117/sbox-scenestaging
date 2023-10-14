using System;
using System.Collections.Generic;
using Sandbox;

namespace DarkDescent;

public static class EntityExtensions
{
    public static bool CanSee(this Entity self, Vector3 point)
    {
        var tr = Trace.Ray( self.AimRay.Position, point )
            .StaticOnly()
            .Radius( 1f )
            .Run();
        
        //DebugOverlay.TraceResult( tr, 0.5f );

        return tr.EndPosition.AlmostEqual(point, 5f);
    }
    
    public static bool CanSee(this Entity self, Entity target, bool thorough = false)
    {
	    return CanSee( self, target.WorldSpaceBounds.Center ) ||(!thorough || CanSee( self, target.AimRay.Position ));
    }

    public static List<Entity> GetEntitiesInCone(Vector3 origin, Vector3 direction, float length, float angle)
    {
        direction = direction.Normal;
        
        var list = new List<Entity>();

        var halfLen = length / 2f;
        var startPos = origin + direction * halfLen;

        var entsInSphere = Entity.FindInSphere(startPos, halfLen);

        foreach (var entity in entsInSphere)
        {
            var directionToEntity = (entity.Position - origin).Normal;
            var deltaAngle = MathF.Acos(directionToEntity.Dot(direction));
            
            if (deltaAngle <= angle)
                list.Add(entity);
        }
        
        return list;
    }

    public static float DistanceToSquared(this Entity self, Entity target)
    {
        return (target.Position - self.Position).LengthSquared;
    }

    public static TraceResult GetEyeTrace(this Entity entity)
    {
        var tr = Trace.Ray(entity.AimRay, 5000f)
            .Ignore(entity)
            .WithoutTags(GameTags.Physics.Trigger)
            .Run();

        return tr;
    }
}
