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
	public class DrawingHelper
	{
		#region drawing
		public static SizeF defaultSizeF = new SizeF(64f, 64f);
		public static Color defaultColor = Color.White;

		/// <summary>
		/// Draw a texture around specified peds
		/// </summary>
		/// <param name="peds">Instances of <c>Ped</c> to mark</param>
		/// <param name="textureDict">Name of the .ytd file containing the texture</param>
		/// <param name="assetName">Name of the texture asset</param>
		/// <param name="distinguishRelationship">Change color based on the <c>Ped</c>'s relationship with the player</param>
		public static void markPedsOnScreen(List<Ped> peds, string textureDict, string assetName, bool distinguishRelationship = true)
		{
			foreach (Ped p in peds)
			{
				markEntityOnScreen(p, textureDict, assetName);
			}
		}


		/// <summary>
		/// Draw a preloaded <c>Sprite</c> on the screen where a specified entity is
		/// </summary>
		/// <param name="entity">Instance of <c>Entity</c> to mark</param>
		/// <param name="spr">Preloaded <c>Sprite</c> to mark the entity with</param>
		public static void markEntityOnScreen(Entity entity, Sprite spr, Color? color = null)
		{
			// get the position of the entity on screen & update the screen position of the sprite
			PointF screenPos = Screen.WorldToScreen(entity.Position);
			spr.Position = screenPos;			// update screen position of sprite
			spr.Color = color ?? spr.Color;		// update sprite color if needed; if null, keep original color
			spr.Draw();
		}


		public static void markEntityOnScreen(Entity entity, string textureDict, string assetName, Color? color = null, SizeF? size = null)
		{
			// get the position of the entity on screen
			PointF screenPos = Screen.WorldToScreen(entity.Position);
			Sprite spr = new Sprite(textureDict, assetName, defaultSizeF, screenPos, color ?? defaultColor, 0f, true);
			spr.Draw();
		}


		#endregion
	}
}
