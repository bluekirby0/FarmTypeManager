﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager.Monsters
{
    /// <summary>An instance of Stardew's Serpent class, adjusted for use by this mod.</summary>
    class SerpentFTM : Serpent
    {
        //this override forces any instance of GameLocation to call drawAboveAllLayers, fixing a bug where flying monsters are invisible on some maps
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b); //call the base version of this, if one exists
            base.drawAboveAllLayers(b); //call the extra draw method used by flying monsters
        }

        /// <summary>Creates an instance of Stardew's Serpent class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public SerpentFTM(Vector2 position)
            :base(position)
        {

        }
    }
}
