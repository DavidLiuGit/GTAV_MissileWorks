// MissileWorks main
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;



namespace GFPS // !!!! IMPORTANT REPLACE THIS WITH YOUR MODS NAME !!!!
{
	public class Main : Script
	{
		public Main()
		{
			Tick += mainOnTick;
			KeyDown += onKeyDown;
			Interval = 5;
		}


		private void mainOnTick(object sender, EventArgs e)
		{
			// filter list of activeMissiles to only those still active
			activeMissiles = activeMissiles.FindAll(missile => missile.active);

			// control all active missiles
			foreach (Missile missile in activeMissiles)
			{
				missile.control();
			}
		}



		private void onKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.J && e.Modifiers == Keys.Control)
			{
				activateMissile(typeof(HellstormMissile));
			}
		}



		/// <summary>
		/// Instantiate a Missile of a specified type. Hand over control by attaching an event handler to Script.Tick as necessary.
		/// </summary>
		/// <param name="missileType"></param>
		private void activateMissile(Type missileType)
		{
			// Instantiate missile
			Missile newMissile = (Missile)Activator.CreateInstance(missileType);
			activeMissiles.Add(newMissile);
		}


		#region properties
		List<Missile> activeMissiles = new List<Missile>();

		#endregion
	}
}

// 
