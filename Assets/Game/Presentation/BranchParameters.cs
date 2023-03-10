using System;

namespace Assets.Game.Presentation
{
	[Serializable]
	public record BranchParameters
	{
		public float TorqueCoefficient;
		public float IntegralCoefficient;
		public float DifferentialCoefficient;
		public float Mass;
		public float AngularDrag;
		public float MassPropagation;
		public float AngularDragPropagation;

		public float ForwardForceMultiplier = 2.0f;
		public float ForwardForceFadeTime = 1.0f;
		public float CounterForceReturnTime = 1.0f;

		public BranchParameters Propagate()
			=> this with
			{
				Mass = Mass * MassPropagation,
				AngularDrag = AngularDrag * AngularDragPropagation
			};

		public BranchParameters ScaleMass(float massScale)
			=> this with { Mass = Mass * massScale };
	}
}