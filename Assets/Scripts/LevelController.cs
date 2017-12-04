using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public interface Goal
{
	bool isComplete ();
}

public class LevelController : MonoBehaviour
{
	//TODO, add this to game.txt
	public static readonly float cubeSize = 2.0f;

	public static readonly string gameFileName = "Game";
	public static readonly string gameFileHeaderSeperator = "**********";
	public static readonly string gameFileFloorSeperator = "---";
	public static readonly string gameFileLevelSeperator = "***";
	public static readonly string gameFileLineStartComment = "//";

	private Dictionary<string, Object> stringToUnityObject_ = new Dictionary<string, Object>();
	private Dictionary<char, string> charToString_ = new Dictionary<char, string>();
	private List<Level> levels_ = new List<Level>();
	private int currentLevelIndex_ = 0;

	private List<Goal> currentGoals_ = new List<Goal>();

	// Ideally would reuse these
	private List<GameObject> currentLevelObjects_ = new List<GameObject>();

	private class Level
	{
		public List<List<List<char>>> floors = new List<List<List<char>>>();

		public void addFloor ( List<List<char>> floor )
		{
			floors.Add( floor );
		}
	}

	// Use this for initialization
	void Start ()
	{
		TextAsset gameFile = Resources.Load<TextAsset>( gameFileName );
		string gameFileText = gameFile.text;
		var headerFooter = gameFileText.Split( new string[] { gameFileHeaderSeperator }, System.StringSplitOptions.RemoveEmptyEntries );

		if ( headerFooter.Length != 2 )
		{
			throw new System.Exception( "Unable to find " + gameFileHeaderSeperator + " in " + gameFileName );
		}

		var headerLines = headerFooter[0].Split( new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		var headerLinesNoComments = headerLines.Where( l => !l.StartsWith( gameFileLineStartComment ) );
		loadResourceDefinitions( headerLinesNoComments );

		var footerLines = headerFooter[1].Split( new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries );
		var footerLinesNoComments = footerLines.Where( l => !l.StartsWith( gameFileLineStartComment ) );
		loadLevels( footerLinesNoComments );

		loadLevel( currentLevelIndex_ );
	}

	void loadResourceDefinitions( IEnumerable<string> lines )
	{
		foreach (var line in lines)
		{
			var pair = line.Split( '=' );
			if (pair.Length != 2)
			{
				throw new System.Exception( "Bad line found when loading resources " + line );
			}
			if (pair[0].Length != 1)
			{
				throw new System.Exception( "Bad line found when loading resources " + line );
			}

			charToString_[pair[0][0]] = pair[1];
			stringToUnityObject_[pair[1]] = Resources.Load( pair[1] );
		}
	}

	void loadLevels ( IEnumerable<string> lines )
	{
		var lastLine = "";

		var currentLevel = new Level();
		var currentFloor = new List<List<char>>();

		foreach (var line in lines)
		{
			lastLine = line;
			if (line == gameFileLevelSeperator)
			{
				levels_.Add( currentLevel );
				currentLevel = new Level();
				continue;
			}

			if (line == gameFileFloorSeperator )
			{
				currentLevel.addFloor( currentFloor );
				currentFloor = new List<List<char>>();
				continue;
			}

			var row = new List<char>();
			currentFloor.Add( row );
			foreach ( char c in line )
			{
				if (!charToString_.ContainsKey(c))
				{
					throw new System.Exception( "Invalid level char found:" + c );
				}
				row.Add( c );
			}
		}

		if ( lastLine != gameFileLevelSeperator )
		{
			throw new System.Exception( "File should end with " + gameFileLevelSeperator );
		}
	}

	private void loadLevel ( int index )
	{
		foreach (var gObj in currentLevelObjects_ )
		{
			Destroy( gObj );
		}
		currentLevelObjects_.Clear();
		currentGoals_.Clear();

		Level currentLevel = levels_[index];

		Vector3 currentPosition = new Vector3( 0, 0, 0 );
		foreach (var floor in currentLevel.floors)
		{
			currentPosition.y += cubeSize;
			currentPosition.z = 0;
			foreach (var row in floor)
			{
				currentPosition.z -= cubeSize;
				currentPosition.x = 0;
				foreach (var charObj in row)
				{
					currentPosition.x += cubeSize;
					Object gameObj = stringToUnityObject_[charToString_[charObj]];

					GameObject instantiatedObq = null;
					try
					{
						instantiatedObq = (GameObject) Instantiate( gameObj, currentPosition, Quaternion.identity );
					}
					catch
					{
						throw new System.Exception( "Failed to instantiate object for " + charObj );
					}

					currentLevelObjects_.Add( instantiatedObq );

					foreach (var goal in instantiatedObq.GetComponents<Goal>())
					{
						currentGoals_.Add( goal );
					}
				}
			}
		}

	}

	// Update is called once per frame
	void Update ()
	{
		// Check for all targets done
		if ( currentGoals_.All( a => a.isComplete() ) )
		{
			currentLevelIndex_++;

			if ( currentLevelIndex_ > levels_.Count - 1)
			{
				currentLevelIndex_ = 0;
			}

			loadLevel( currentLevelIndex_ );
		}
	}
}
