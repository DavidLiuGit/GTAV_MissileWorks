using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;



namespace GFPS
{
	public class HellstormMissile : Missile
	{
		#region properties
		protected const float initialHeight = 400.0f;
		protected const float initialRadius = 100.0f;

		// instance references & pointers
		protected Model clusterMissileModel;

		// lifecycle
		private MissileLifecycle lifecycleStage;
		protected int launchStageTime = 750;
		protected int launchStageTransitionTime = 200;

		// control
		protected float maxBoostSpeed = 150.0f;
		protected Vector3 xAxisControlVector = new Vector3(1f, 0f, 0f) * 10f;
		protected Vector3 yAxisControlVector = new Vector3(0f, 0f, 1f) * 10f;

		// cluster bombs
		protected bool clusterBombsReady;
		protected ExplosionType clusterBombExplosionType = ExplosionType.Grenade;
		protected int maxClusterBombs = 5;

		// camera
		protected Vector3 launchCameraOffset = new Vector3 (10f, 0f, -40f);
		protected Vector3 missileCamOffset = new Vector3(0f, 0f, 0f);
		protected float cameraFov = 75f;

		// targeting
		protected TargetingSystem targetingSys;
		protected string targetingTextureDict = "hud_reticle";
		protected string targetingAssetname = "reticle_smg";
		protected Sprite pedMarkerSprite;
		#endregion




		#region constructorDestructor
		public HellstormMissile()
			: base() 
		{}
		#endregion




		#region overridenMethods
		protected override void configure()
		{
			// call parent configure() method
			base.configure();

			// overwrite any configs as necessary
			missileModel = (Model)(-1479625776);			// SAM missile
			//clusterMissileModel = (Model)737852268;		// green missile
			clusterMissileModel = (Model)(-1146260322);		// homing missile
			attachCamera = true;
			explosionType = ExplosionType.Plane;
			timeout = 15000;
			explosionDamageScale = 2.0f;
			clusterBombsReady = false;						// initially, do not allow use of cluster bombs

			// load particle FX
			//particleFxAsset = new ParticleEffectAsset("scr_agencyheistb");
			//particleFxAsset.Request();
			particleFxOffset = new Vector3(0f, 2.925f, 0f);
			//particleFxName = "scr_agency3b_proj_rpg_trail";

			// control
			forwardVector = new Vector3(0f, -1f, 0f);
			forwardAngle = new Vector3(180f, 0f, 0f);

			// load targeting resources
			targetingSys = new TargetingSystem(Game.Player.Character);
			pedMarkerSprite = new Sprite(targetingTextureDict, targetingAssetname, 
				DrawingHelper.defaultSizeF, new PointF(0f, 0f), DrawingHelper.defaultColor, 0f, true);
		}



		/// <summary>
		/// Spawn a missile as a prop
		/// </summary>
		protected override void spawnMissile()
		{
			// get an offset Vector3 to spawn the missile at
			Vector3 playerPos = Game.Player.Character.Position;
			Vector3 randomAround = playerPos.Around(initialRadius);
			randomAround.Z += initialHeight;

			// spawn the prop
			missile = World.CreateProp(missileModel, randomAround, true, false);
		}



		/// <summary>
		/// Apply appropriate settings to the missile prop instance
		/// </summary>
		protected override void configureMissileProp()
		{
			// set lifecycle stage
			lifecycleStage = MissileLifecycle.Launch;

			// apply settings to the missile
			missile.HasGravity = false;
			missile.MaxSpeed = maxCruiseSpeed;

			// orient the missile towards the player. get normalized delta vector that points towards the player
			Vector3 directionVector = (Game.Player.Character.Position - missile.Position).Normalized;
			missile.Rotation = Helper.getEulerAngles(directionVector, forwardAngle);
		}



		/// <summary>
		/// Apply missile flight control logic. Apply user input if enabled.
		/// </summary>
		/// <returns>Whether the control flow can proceed</returns>
		public override bool control()
		{
			// invoke base control method; if it returns false, stop execution
			if (!base.control())
				return false;

			// calculate how long the missile has been created
			int age = Game.GameTime - creationTime;

			// check if it is necessary to transition to the next lifecycle stage
			switch (lifecycleStage)
			{
				case MissileLifecycle.Launch:
					if (age >= launchStageTime)
					{
						transitionToMissileCam(missileCamera);
						lifecycleStage = MissileLifecycle.Cruise;
						canControl = true;
						clusterBombsReady = true;
					}
					break;
			}

			// apply "thrust" to the missile; recall that the missile has a set maximum speed
			missile.ApplyForceRelative(thrustMagnitude * forwardVector);
			
			// if user control is enabled, detect relevant user input
			if (canControl)
				applyUserInput();

			// mark Peds according to their relationship with the player
			targetObservablePeds();

			return true;
		}


		
		/// <summary>
		/// Create the missile cam. Initially, the camera shows the "launch"
		/// </summary>
		protected override void createCamera()
		{
			// create new camera at the offset and point it towards to missile; render from this new camera
			missileCamera = World.CreateCamera(missile.Position + launchCameraOffset, Vector3.Zero, cameraFov);
			missileCamera.PointAt(missile);
			
			// activate camera for rendering
			World.RenderingCamera = missileCamera;
		}
		#endregion




		#region helperMethods
		/// <summary>
		/// Transition the specified camera and attach it to the missile, as if it were mounted
		/// to the front of the missile
		/// </summary>
		protected void transitionToMissileCam(Camera cam)
		{
			// fade the screen to black & pause the script
			GTA.UI.Screen.FadeOut(launchStageTransitionTime);
			Script.Wait(launchStageTransitionTime);

			// transition to missile cam
			cam.StopPointing();
			cam.AttachTo(missile, missileCamOffset);
			cam.Direction = forwardVector;
			cam.Rotation = missile.Rotation + forwardAngle;

			// fade the screen back in
			GTA.UI.Screen.FadeIn(launchStageTransitionTime);
		}



		/// <summary>
		/// When the user can control the missile, read & apply user input
		/// </summary>
		protected void applyUserInput()
		{
			// read fly up & down input; apply forces accordingly
			float upDownCtrl = Game.GetControlValueNormalized(Control.FlyUpDown);
			if (upDownCtrl != 0.0f)
				Helper.ApplyForceToCoG(missile, yAxisControlVector * upDownCtrl, true);

			// read fly left & right input; apply forces accordingly
			float leftRightCtrl = Game.GetControlValueNormalized(Control.FlyLeftRight);
			if (leftRightCtrl != 0.0f)
				Helper.ApplyForceToCoG(missile, xAxisControlVector * leftRightCtrl, true);
		}



		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool targetObservablePeds()
		{
			// find all observable peds from the missile's vantage point
			List<Ped> observablePeds = Helper.getPedsInRangeFromVantage(missile.Position).ToList();

			// iterate over the list of observable peds & mark peds as needed
			List<Entity> targets = new List<Entity>();
			foreach (Entity p in observablePeds)
			{
				TargetType tt = targetingSys.getPedTargetType((Ped)p);
				if (tt == TargetType.Hostile) targets.Add(p);		//if ped is hostile, add to target list
				DrawingHelper.markEntityOnScreen(p, pedMarkerSprite, DrawingHelper.getColorFromTargetType(tt));
			}

			// if cluster bombs are ready, and user is pressing 
			if (clusterBombsReady && Game.IsControlPressed(Control.VehicleFlyAttack))
				fireClusterBombs(targets);
			
			return true;
		}



		/// <summary>
		/// Fire cluster bombs from main Hellstorm missile.
		/// </summary>
		/// <param name="targets"></param>
		private void fireClusterBombs(List<Entity> targets)
		{
			// if no targets in list, do nothing and return
			if (targets.Count == 0)
				return;

			// mark cluster bombs as used; create explosions (up to the max limit) on targets
			clusterBombsReady = false;
			for (int i = 0; i < maxClusterBombs && i < targets.Count; i++)
				World.AddExplosion(targets[i].Position, clusterBombExplosionType, 1f, 1f, Game.Player.Character);
		}
		#endregion




		#region lifecycle
		protected new enum MissileLifecycle
		{
			Launch,			// missile is created, tracked w/ cinematic camera, and boosted up to speed
			Cruise,			// missile cam used; player allowed to control; can launch cluster bombs
			Boost,			// missile increases its thrust and speed
		}
		#endregion
	}
}
