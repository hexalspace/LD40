using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Aimer : MonoBehaviour {

	public float squareSideLength = 1;
	public int squaresInRow = 1;
	public float cycleDuration = 1;

	public int NumberOfSquares {
		get
		{
			return squaresInRow * squaresInRow;
		}
	}

	public float TimePerPair
	{
		get
		{
			return cycleDuration / NumberOfSquares;
		}
	}

	// Use this for initialization
	void Start ()
	{
		calcSquareCenters();
		calcSquareOrdering();
	}

	void OnValidate ()
	{
		changeParameter( squareSideLength, squaresInRow, cycleDuration );
	}

	void changeParameter(float squareSideLength, int squaresInRow, float cycleDuration)
	{
		this.squareSideLength = squareSideLength;
		this.squaresInRow = squaresInRow;
		this.cycleDuration = cycleDuration;

		squareFrom_ = getCurrentPos();
		timeIntoCurrentPair_ = 0;
		calcSquareCenters();
		calcSquareOrdering();
	}

	void debugDrawSquare(Vector3 localSpaceVector, Color c)
	{
		Vector3 sll = localSpaceVector.Clone();
		sll.x -= squareSideLength / 2;
		sll.y -= squareSideLength / 2;

		Vector3 slr = localSpaceVector.Clone();
		slr.x += squareSideLength / 2;
		slr.y -= squareSideLength / 2;

		Vector3 sul = localSpaceVector.Clone();
		sul.x -= squareSideLength / 2;
		sul.y += squareSideLength / 2;

		Vector3 sur = localSpaceVector.Clone();
		sur.x += squareSideLength / 2;
		sur.y += squareSideLength / 2;

		debugDrawLineLocalCoord( sll, slr, c );
		debugDrawLineLocalCoord( slr, sur, c );
		debugDrawLineLocalCoord( sur, sul, c );
		debugDrawLineLocalCoord( sul, sll, c );
	}

	void debugDrawLineLocalCoord ( Vector3 localStart, Vector3 localEnd, Color c )
	{
		Vector3 worldStart = transform.TransformPoint( localStart );
		Vector3 worldEnd = transform.TransformPoint( localEnd );

		Debug.DrawLine( worldStart, worldEnd, c );

	}

	void debugDraw()
	{
		foreach (var sc2 in squareCenters_ )
		{
			debugDrawSquare( sc2.ZeroFill(), Color.white );
		}

		debugDrawSquare( squareFrom_.ZeroFill(), Color.blue );
		debugDrawSquare( squareTo_.ZeroFill(), Color.green );
		debugDrawSquare( getCurrentPos().ZeroFill(), Color.red );
	}

	Vector2 getCurrentPos()
	{
		var lerpFactor = Mathf.Sin( ( timeIntoCurrentPair_ / TimePerPair ) * ( Mathf.PI / 2 ) );
		return Vector2.Lerp( squareFrom_, squareTo_, lerpFactor );
	}

	// Update is called once per frame
	void Update ()
	{
		timeIntoCurrentPair_ += Time.deltaTime;

		if ( timeIntoCurrentPair_ > TimePerPair )
		{
			squareFrom_ = squareTo_;
			squareTo_ = getNextSquare();
			timeIntoCurrentPair_ = timeIntoCurrentPair_ - TimePerPair;
		}

		debugDraw();

	}

	Vector2 getNextSquare()
	{
		if ( currentIterationOrder_.Count == 0 )
		{
			calcSquareOrdering();
		}

		var result = squareCenters_[currentIterationOrder_[0]];

		currentIterationOrder_.RemoveAt( 0 );

		return result;
	}

	void calcSquareCenters()
	{
		// Calculate Square Positions
		if ( squaresInRow < 1 )
		{
			throw new System.Exception( "squaresInRow cannot be less than 1" );
		}

		float upperLeftY = ( squareSideLength / 2 ) * ( squaresInRow - 1 );
		float upperLeftX = -upperLeftY;

		squareCenters_.Clear();

		for (int i = 0; i < squaresInRow; i++ )
		{
			for (int j = 0; j < squaresInRow; j++ )
			{
				var newSquareCenter = new Vector2( upperLeftX + ( i * squareSideLength ), upperLeftY - ( j * squareSideLength ) );
				squareCenters_.Add( newSquareCenter );
			}
		}
	}

	void calcSquareOrdering ()
	{
		// Calculate a traversal order for current square set
		var unusedIndexes = new List<int>();

		for ( int i = 0; i < NumberOfSquares; i++ )
		{
			unusedIndexes.Add( i );
		}

		currentIterationOrder_.Clear();

		while ( unusedIndexes.Count > 0 )
		{
			var selectedUnusedIndexIndex = Random.Range( 0, unusedIndexes.Count );
			currentIterationOrder_.Add( unusedIndexes[selectedUnusedIndexIndex] );
			unusedIndexes[selectedUnusedIndexIndex] = unusedIndexes[unusedIndexes.Count - 1];
			unusedIndexes.RemoveAt( unusedIndexes.Count - 1 );
		}
	}

	private Vector2 squareFrom_;
	private Vector2 squareTo_;
	private float timeIntoCurrentPair_ = 0;
	private List<Vector2> squareCenters_ = new List<Vector2>();
	private List<int> currentIterationOrder_ = new List<int>();


}
