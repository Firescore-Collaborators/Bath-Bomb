using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component places the specified <b>Transform</b> at the current hit point. A hit point can be found using a companion component like: <b>P3dHitScreen</b>, <b>P3dHitBetween</b>, etc.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dMover")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Mover")]
	public class P3dMover : MonoBehaviour, IHit, IHitPoint, IHitLine
	{
		/// <summary>The <b>Transform</b> that will be moved.</summary>
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		/// <summary>The offset from the hit point based on the normal in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The speed at which this Transform moves toward the hit point.
		/// -1 = Instant.</summary>
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = 5.0f;

		/// <summary>If no hit point has been set, use a default one?</summary>
		public Transform DefaultTransform { set { defaultTransform = value; } get { return defaultTransform; } } [SerializeField] private Transform defaultTransform;

		/// <summary>If no hit point has been set for this many seconds, then the target transform will be reset and move to the default transform.</summary>
		public float DefaultTime { set { defaultTime = value; } get { return defaultTime; } } [SerializeField] private float defaultTime;

		[SerializeField]
		private bool targetSet;

		[SerializeField]
		private Vector3 targetPosition;

		[SerializeField]
		private Quaternion targetRotation;

		[SerializeField]
		private float inactiveTime;

		public void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
		{
			inactiveTime   = 0.0f;
			targetSet      = true;
			targetPosition = position;
			targetRotation = rotation;
		}

		public void HandleHitLine(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 position2, Quaternion rotation)
		{
			HandleHitPoint(preview, priority, pressure, seed, position2, rotation);
		}

		protected virtual void Update()
		{
			if (Target != null)
			{
				if (inactiveTime > defaultTime && defaultTransform != null)
				{
					targetSet = false;
				}

				inactiveTime += Time.deltaTime;

				if (targetSet == true || defaultTransform != null)
				{
					var finalPosition = default(Vector3);
					var finalRotation = default(Quaternion);

					if (targetSet == true)
					{
						finalPosition = targetPosition + targetRotation * Vector3.back * offset;
						finalRotation = targetRotation;
					}
					else
					{
						finalPosition = defaultTransform.position + defaultTransform.rotation * Vector3.back * offset;
						finalRotation = defaultTransform.rotation;
					}

					var factor = P3dHelper.DampenFactor(dampening, Time.deltaTime);

					Target.position = Vector3.Lerp(Target.position, finalPosition, factor);
					Target.rotation = Quaternion.Slerp(Target.rotation, finalRotation, factor);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dMover))]
	public class P3dMover_Editor : P3dEditor<P3dMover>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				Draw("target", "The Transform that will be moved.");
			EndError();
			Draw("offset", "The offset from the hit point based on the normal in world space.");
			Draw("dampening", "The speed at which this Transform moves toward the hit point.\n\n-1 = Instant.");

			Separator();

			Draw("defaultTransform", "If no hit point has been set, use a default one?");
			Draw("defaultTime", "If no hit point has been set for this many seconds, then the target transform will be reset and move to the default transform.");
		}
	}
}
#endif