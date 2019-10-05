﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates a list of all valid tiles for object spawning in the provided SpawnArea.</summary>
            /// <param name="area">A SpawnArea listing an in-game map name and the valid regions/terrain within it that may be valid spawn points.</param>
            /// <param name="quarryTileIndex">The list of quarry tile indices for this spawn process.</param>
            /// <param name="customTileIndex">The list of custom tile indices for this spawn process.</param>
            /// <returns>A completed list of all valid tile coordinates for this spawn process in this SpawnArea.</returns>
            public static List<Vector2> GenerateTileList(SpawnArea area, InternalSaveData save, int[] quarryTileIndex, int[] customTileIndex)
            {
                HashSet<Vector2> validTiles = new HashSet<Vector2>(); //a set of all open, valid tiles for new spawns in the provided area

                foreach (string type in area.IncludeTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
                {
                    if (type.Equals("quarry", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "quarry" tile index list
                    {
                        validTiles.UnionWith(GetTilesByIndex(area, quarryTileIndex));
                    }
                    else if (type.Equals("custom", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "custom" tile index list
                    {
                        validTiles.UnionWith(GetTilesByIndex(area, customTileIndex));
                    }
                    else  //add any tiles with properties matching "type" (e.g. tiles with the "Diggable" property, "Grass" type, etc; if the "type" is "All", this will just add every valid tile)
                    {
                        validTiles.UnionWith(GetTilesByProperty(area, type));
                    }
                }
                foreach (string include in area.IncludeAreas) //check for valid tiles in each "include" zone for the area
                {
                    validTiles.UnionWith(GetTilesByVectorString(area, include));
                }

                if (area is LargeObjectSpawnArea objArea && objArea.FindExistingObjectLocations) //if this area is the large object type and is set to use existing object locations
                {
                    if (save.ExistingObjectLocations.ContainsKey(area.UniqueAreaID)) //if this area has save data for existing locations
                    {
                        foreach (string include in save.ExistingObjectLocations[area.UniqueAreaID]) //check each saved "include" string for the area
                        {
                            validTiles.UnionWith(GetTilesByVectorString(area, include));
                        }
                    }
                    else //if this area has not generated any save data for existing locations yet (note: this *shouldn't* be reachable)
                    {
                        Monitor.Log($"Issue: This area never saved its object location data: {area.UniqueAreaID}", LogLevel.Info);
                        Monitor.Log($"FindExistingObjectLocations will not function for this area. Please report this to the mod's author.", LogLevel.Info);
                    }
                }

                foreach (string exclude in area.ExcludeAreas) //check for valid tiles in each "exclude" zone for the area (validity isn't technically relevant here, but simpler to code, and tiles' validity cannot currently change during this process)
                {
                    HashSet<Vector2> excludedTiles = GetTilesByVectorString(area, exclude); //get list of valid tiles in the excluded area
                    validTiles.ExceptWith(excludedTiles); //remove any previously valid tiles that match the excluded area
                }

                return validTiles.ToList(); //convert the set to a list and return it
            }
        }
    }
}