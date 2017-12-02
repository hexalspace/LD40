using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExtensionMethods
{
	public static class Extensions
	{
		public static Vector3 ZeroFill ( this Vector2 v )
		{
			return new Vector3( v.x, v.y, 0 );
		}

		public static Vector3 Clone ( this Vector3 v )
		{
			return new Vector3( v.x, v.y, v.z );
		}
	}
}
