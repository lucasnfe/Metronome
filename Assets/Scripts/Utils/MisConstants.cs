using UnityEngine;
using System.Collections;

public class MisConstants {

	// PHYSICS SYSTEM CONSTANTS
	public static readonly float GRAVITY              = Physics2D.gravity.y/20f;
	public static readonly float PLAYER_SKIN          = 0.0005f;
	public static readonly float MAX_SPEED     		  = 1f;

	// TILE_SIZE = A power of two number -1f in order the avoid placement bugs
	public static readonly float TILE_SIZE      	  = 64f - 1f;
	public static readonly int   PIXEL_UNIT     	  = 100;
	
	// LEVEL GENERATOR CONSTANTS
	public static readonly int   MAX_GAP_SIZE 		  = 2;
	public static readonly float NON_ENEMIES_AREA 	  = 4f;

	// EVENT PLATFORM TAGS
	public static readonly string TAG_KILLZONE 		  = "Killzone";
	public static readonly string TAG_WALL 		      = "Wall";

	public enum ENEMIES {

		ALIEN
	}
}
