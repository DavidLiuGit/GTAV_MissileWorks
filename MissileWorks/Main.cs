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
			Interval = 1;
		}


		private void mainOnTick(object sender, EventArgs e)
		{
			
		}



		private void onKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.J && e.Modifiers == Keys.Control)
			{
				activeMissile = new HellstormMissile();
			}
		}



		#region properties
		Missile activeMissile;

		#endregion
	}
}

// 
