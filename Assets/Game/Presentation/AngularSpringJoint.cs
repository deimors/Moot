using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Game.Presentation
{
	public class AngularSpringJoint : MonoBehaviour
	{
		public float TorqueCoefficient = 1;

		public float DifferentialCoeff = 1f;
		public float IntegralCoeff = 0.1f;

		private AngularSpringJoint _parentJoint;
		private List<AngularSpringJoint> _childBranches;

		private float _forwardForceMultiplier = 1.0f;
		private float _counterForceMultiplier = 1.0f;

		private Rigidbody2D _rigidbody2d;
		private float _originalRotation;
		private Rigidbody2D _connectedRigidbody2d;
		private float _originalRotationDiff;

		private float _integral;

		void Awake()
		{
			_childBranches = new List<AngularSpringJoint>();
			_rigidbody2d = GetComponent<Rigidbody2D>();
			_connectedRigidbody2d = GetComponent<HingeJoint2D>().connectedBody;
			_originalRotation = _rigidbody2d.rotation;
			_originalRotationDiff = _originalRotation - _connectedRigidbody2d.rotation;
		}

		private void Start()
		{
			if (transform.parent != null)
			{
				if (transform.parent.TryGetComponent<AngularSpringJoint>(out var joint))
					_parentJoint = joint;
				else if (transform.parent.TryGetComponent<BranchPresenter>(out var presenter)
					&& presenter.transform.parent != null
					&& presenter.transform.parent.TryGetComponent<AngularSpringJoint>(out var presenterJoint))
					_parentJoint = presenterJoint;

				if (_parentJoint != null)
					_parentJoint._childBranches.Add(this);
			}

		}

		void FixedUpdate()
		{
			var targetRotation = _connectedRigidbody2d != null
				? _connectedRigidbody2d.rotation + _originalRotationDiff
				: _originalRotation;
			
			var rotationDiff = targetRotation - _rigidbody2d.rotation;

			rotationDiff %= 360;

			if (rotationDiff < -180)
				rotationDiff += 360;
			if (rotationDiff > 180)
				rotationDiff -= 360;

			_integral -= rotationDiff;

			var targetTorque = TorqueCoefficient * rotationDiff
				+ IntegralCoeff * _integral
				+ DifferentialCoeff * _rigidbody2d.angularVelocity;

			var multiplier = targetTorque < 0 ? _forwardForceMultiplier : _counterForceMultiplier;
					
			ApplyTorque(targetTorque * multiplier, transform.position);

			_forwardForceMultiplier -= Time.fixedDeltaTime * 1.0f;
			if (_forwardForceMultiplier < 1.0f)
				_forwardForceMultiplier = 1.0f;

			_counterForceMultiplier += Time.fixedDeltaTime * 1.0f;
			if (_counterForceMultiplier > 1.0f)
				_counterForceMultiplier = 1.0f;
		}

		private void ApplyTorque(float targetTorque, Vector2 origin)
		{
			var offset = (Vector2)transform.position - origin;
			var newOffset = RotateVector(offset, Mathf.Sign(targetTorque));
			var end = origin + newOffset;
			var forceDirection = end - (Vector2)transform.position;

			_rigidbody2d.AddTorque(targetTorque);
			_rigidbody2d.AddForce(forceDirection * Mathf.Abs(targetTorque) * 4);

			foreach (var child in _childBranches)
				child.ApplyTorque(targetTorque * 0.3f, origin);
		}

		public void SetOnlyAccelerate()
		{
			var root = this;

			while (root._parentJoint != null)
				root = root._parentJoint;

			root.SetChildrenToOnlyAccelerate();
		}

		private void SetChildrenToOnlyAccelerate()
		{
			_forwardForceMultiplier = 2.0f;
			_counterForceMultiplier = 0.0f;

			foreach (var child in _childBranches)
				child.SetChildrenToOnlyAccelerate();
		}

		public Vector2 RotateVector(Vector2 vector, float degrees) 
			=> Quaternion.Euler(0, 0, degrees) * vector;
	}
}