using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This interface allows you to make components that can be painted.</summary>
	public interface IModel
	{
		void GetPrepared(ref Mesh mesh, ref Matrix4x4 matrix);
	}
}