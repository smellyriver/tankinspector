using System;
using System.Collections.Generic;
using SharpDX;
using System.Windows.Input;
using System.Windows;

// made after DXUTCamera.h & DXUTCamera.cpp

namespace Smellyriver.TankInspector.Graphics.Frameworks
{


	/// <summary>
	/// Simple first person camera class that moves and rotates.
	/// It allows yaw and pitch but not roll.  It uses WM_KEYDOWN and 
	/// GetCursorPos() to respond to keyboard and mouse input and updates the 
	/// view matrix based on input.  
	/// </summary>
	public partial class FirstPersonCamera : BaseCamera
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eye"></param>
		/// <param name="lookAt"></param>
		/// <param name="vUp"></param>
		public override void SetViewParams(Vector3 eye, Vector3 lookAt, Vector3 vUp)
		{
			base.SetViewParams(eye, lookAt, vUp);
			MViewRotQuat = Quaternion.Identity;
		}
	}

	/// <summary>
	/// Simple model viewing camera class that rotates around the object.
	/// </summary>
	public class ModelViewerCamera : BaseCamera
	{
		private float _mRadius;
		private Quaternion _mModelRotQuat;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eye"></param>
		/// <param name="lookAt"></param>
		/// <param name="vUp"></param>
		public override void SetViewParams(Vector3 eye, Vector3 lookAt, Vector3 vUp)
		{
			base.SetViewParams(eye, lookAt, vUp);

			_mModelRotQuat = Quaternion.Identity;
			_mRadius = (eye - lookAt).Length();
			UpdateWorld();
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateWorld()
		{
			this.World = Matrix.RotationQuaternion(_mModelRotQuat);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mMouseQuat"></param>
		protected override void MouseRotation(Quaternion mMouseQuat)
		{
			_mModelRotQuat = _mModelRotQuat * mMouseQuat;
			UpdateWorld();
		}

		/// <summary>
		/// 
		/// </summary>
		public Matrix World { get; private set; }
	}


	/// <summary>
	/// Simple base camera class that moves and rotates.  The base class
	/// records mouse and keyboard input for use by a derived class, and
	/// keeps common state.
	/// </summary>
	public abstract class BaseCamera
	{
		#region Init

		/// <summary>
		/// 
		/// </summary>
		public BaseCamera()
		{
			SetViewParams(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
			SetProjParams((float)Math.PI / 3, 1, 0.05f, 100.0f);
			OnInitInteractive();
		}
		#endregion

		#region View

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eye"></param>
		/// <param name="lookAt"></param>
		public void SetViewParams(Vector3 eye, Vector3 lookAt)
		{
			SetViewParams(eye, lookAt, _mUp);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eye"></param>
		/// <param name="lookAt"></param>
		/// <param name="vUp"></param>
		public virtual void SetViewParams(Vector3 eye, Vector3 lookAt, Vector3 vUp)
		{
			_mDefaultPosition = _mPosition = eye;
			_mDefaultLookAt = _mLookAt = lookAt;
			_mDefaultUp = _mUp = vUp;
			MViewRotQuat = Quaternion.Identity;
			UpdateView();
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void UpdateView()
		{
			this.View = Matrix.LookAtLH(_mPosition, _mLookAt, _mUp);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			SetViewParams(_mDefaultPosition, _mDefaultLookAt, _mDefaultUp);
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 Position
		{
			get => _mPosition;
			set
			{
				_mPosition = value;
				UpdateView();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 LookAt
		{
			get => _mLookAt;
			set
			{
				_mLookAt = value;
				UpdateView();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 Up
		{
			get => _mUp;
			set
			{
				_mUp = value;
				UpdateView();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Matrix View { get; private set; }

		#endregion

		#region Projection

        /// <summary>
        /// 
        /// </summary>
        private void UpdateProj()
		{
			this.Projection = Matrix.PerspectiveFovLH(_mFov, _mAspect, _mNearPlane, _mFarPlane);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fFov"></param>
        /// <param name="fAspect"></param>
        /// <param name="fNearPlane"></param>
        /// <param name="fFarPlane"></param>
		public void SetProjParams(float fFov, float fAspect, float fNearPlane, float fFarPlane)
		{
			_mFov = fFov;
			_mAspect = fAspect;
			_mNearPlane = fNearPlane;
			_mFarPlane = fFarPlane;
			UpdateProj();
		}

        /// <summary>
        /// 
        /// </summary>
		public float NearPlane
		{
			get => _mNearPlane;
	        set
			{
				_mNearPlane = value;
				UpdateProj();
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public float FarPlane
		{
			get => _mFarPlane;
	        set
			{
				_mFarPlane = value;
				UpdateProj();
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public float AspectRatio
		{
			get => _mAspect;
	        set
			{
				_mAspect = value;
				UpdateProj();
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public float FieldOfView
		{
			get => _mAspect;
	        set
			{
				_mFov = value;
				UpdateProj();
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public Matrix Projection { get; private set; }

		#endregion

		#region Interaction

		/// <summary>
		/// 
		/// </summary>
		private void OnInitInteractive()
		{
			SetScalers((float)Math.PI / 5, 3);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		protected static Vector2 GetVector(UIElement ui, MouseEventArgs e)
		{
			var p = e.GetPosition(ui);
			return new Vector2((float)p.X, (float)(ui.RenderSize.Height - p.Y));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleMouseDown(UIElement ui, MouseButtonEventArgs e)
		{
			MMouseDownPos = BaseCamera.GetVector(ui, e);
			MMouseLastPos = MMouseDownPos;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dp"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected float GetMouseAngle(Vector2 dp, UIElement ui)
		{
			float div = (float)Math.Max(ui.RenderSize.Width, ui.RenderSize.Height) / 2;
			if (div < 1)
				div = 1;

			float angle = dp.Length() / div;
			return angle;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleMouseMove(UIElement ui, MouseEventArgs e)
		{
			var pMouse = BaseCamera.GetVector(ui, e);
			var dp = pMouse - MMouseLastPos;

			{
				var rAxis = Vector3.Cross(new Vector3(dp.X, dp.Y, 0), new Vector3(0, 0, -1));
				if (rAxis.LengthSquared() >= 0.00001)
				{
					float angle = GetMouseAngle(dp, ui);
					var tmpQuat = Quaternion.RotationAxis(rAxis, angle);
					MouseRotation(tmpQuat);
				}
			}

			MMouseLastPos = pMouse;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleMouseUp(UIElement ui, MouseButtonEventArgs e)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleMouseWheel(UIElement ui, MouseWheelEventArgs e)
		{
			var dp = e.Delta > 0 ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
			KeyMove(dp);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleKeyDown(UIElement ui, KeyEventArgs e)
		{
			MDownKeys[e.Key] = true;

			switch (e.Key)
			{
				case Key.W:
				case Key.Up:
				case Key.S:
				case Key.Down:
				case Key.D:
				case Key.Right:
				case Key.A:
				case Key.Left:
				case Key.PageUp:
				case Key.PageDown:
					// speed
					break;
				case Key.E:
				case Key.Q:
					// roll speed
					break;
				case Key.Home:
					Reset();
					break;
				default:
					return;
			}
			e.Handled = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		private static Vector3 GetSpeed(Key k)
		{
			switch (k)
			{
				case Key.W:
				case Key.Up:
					return new Vector3(0, 0, 1);
				case Key.S:
				case Key.Down:
					return new Vector3(0, 0, -1);
				case Key.D:
				case Key.Right:
					return new Vector3(1, 0, 0);
				case Key.A:
				case Key.Left:
					return new Vector3(-1, 0, 0);
				case Key.PageUp:
					return new Vector3(0, 1, 0);
				case Key.PageDown:
					return new Vector3(0, -1, 0);
			}
			return SZero3;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		private static float GetRollSpeed(Key k)
		{
			switch (k)
			{
				case Key.E:
					return 1;
				case Key.Q:
					return -1;
			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="e"></param>
		public void HandleKeyUp(UIElement ui, KeyEventArgs e)
		{
			MDownKeys.Remove(e.Key);
		}

		/// <summary>
		/// 
		/// </summary>
		public bool EnableYAxisMovement
		{
			get => _mEnableYAxisMovement;
			set
			{
				if (value == _mEnableYAxisMovement)
					return;
				_mEnableYAxisMovement = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elapsed"></param>
		public void FrameMove(TimeSpan elapsed)
		{
			float rSpeed = 0;
			Vector3 speed = new Vector3();
			foreach (var item in MDownKeys.Keys)
			{
				speed += BaseCamera.GetSpeed(item);
				rSpeed += BaseCamera.GetRollSpeed(item);
			}

			KeyMove(speed * (float)elapsed.TotalSeconds);
			KeyRoll(rSpeed * (float)elapsed.TotalSeconds);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sRotation"></param>
		/// <param name="sMove"></param>
		public void SetScalers(float sRotation, float sMove)
		{
			RotationScaler = sRotation;
			MoveScaler = sMove;
		}

		/// <summary>
		/// 
		/// </summary>
		public float RotationScaler { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public float MoveScaler { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dp"></param>
		protected virtual void KeyMove(Vector3 dp)
		{
			if (!EnableYAxisMovement)
				dp.Y = 0;
			dp *= MoveScaler;
			dp = Matrix.RotationQuaternion(MViewRotQuat).TransformNormal(dp);
			Position += dp;
			LookAt += dp;
			UpdateView();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="angle"></param>
		protected virtual void KeyRoll(float angle)
		{
			angle *= RotationScaler;
			var m = Matrix.RotationZ(angle);
			Up = m.TransformNormal(Up);
			UpdateView();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dMouse"></param>
		protected virtual void MouseRotation(Quaternion dMouse)
		{
			var mRot = Matrix.RotationQuaternion(dMouse);

			LookAt = Position + mRot.TransformNormal(LookAt - Position);
			Up = mRot.TransformNormal(Up);

			MViewRotQuat *= dMouse;
			MViewRotQuat.Normalize();
		}

		#endregion

		#region Fields

		private float _mFov;                 // Field of view
        private float _mAspect;              // Aspect ratio
        private float _mNearPlane;           // Near plane
        private float _mFarPlane;            // Far plane

		private Vector3 _mPosition, _mDefaultPosition;
		private Vector3 _mLookAt, _mDefaultLookAt;
		private Vector3 _mUp, _mDefaultUp;

		protected Vector2 MMouseDownPos, MMouseLastPos;
		protected Dictionary<Key, bool> MDownKeys = new Dictionary<Key, bool>();
		protected Quaternion MViewRotQuat;
		private bool _mEnableYAxisMovement = true;
		private static readonly Vector3 SZero3 = new Vector3();
		#endregion
	}
}
