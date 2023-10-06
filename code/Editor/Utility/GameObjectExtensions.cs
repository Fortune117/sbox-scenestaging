﻿namespace Sandbox;

public static class SceneExtensions
{
	public static bool IsDeletable( this GameObject target )
	{
		if ( target is null ) return false;
		if ( target is Scene ) return false;

		return true;
	}
}
