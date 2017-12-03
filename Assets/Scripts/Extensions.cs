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

		public static Vector3 TransformPointUnscaled ( this Transform transform, Vector3 position )
		{
			var localToWorldMatrix = Matrix4x4.TRS( transform.position, transform.rotation, Vector3.one );
			return localToWorldMatrix.MultiplyPoint3x4( position );
		}

		public static Vector3 InverseTransformPointUnscaled ( this Transform transform, Vector3 position )
		{
			var worldToLocalMatrix = Matrix4x4.TRS( transform.position, transform.rotation, Vector3.one ).inverse;
			return worldToLocalMatrix.MultiplyPoint3x4( position );
		}


	}
}
