using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace DarkDescent;

public static class FortuneUtil
{
    public static bool IsMouseInside(this Panel panel)
    {
        var deltaPos = panel.ScreenPositionToPanelPosition(Mouse.Position);
        var width = panel.Box.Rect.Width;
        var height = panel.Box.Rect.Height;
            
        return deltaPos.x >= 0 && deltaPos.y >= 0 && deltaPos.x <= width && deltaPos.y <= height;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        return list[Game.Random.Int(list.Count - 1)];
    }
    
    public static T GetRandom<T>(this IList<T> list)
    {
        return list[Game.Random.Int(list.Count - 1)];
    }
    
    public static void SetPosition(this Panel panel, Vector2 position)
    {
        panel.Style.Left = position.x;
        panel.Style.Top = position.y;
    }
    
    public static float RangedFloat( this Random random, RangedFloat rangedFloat )
    {
	    return random.Float( rangedFloat.x, rangedFloat.y );
    }
}
