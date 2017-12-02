using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Aimer : MonoBehaviour {

	public float sqareSideLength = 1;
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

	void debugDrawSquare(Vector3 sc3, Color c)
	{
		Vector3 sll = sc3.Clone();
		sll.x -= sqareSideLength / 2;
		sll.y -= sqareSideLength / 2;

		Vector3 slr = sc3.Clone();
		slr.x += sqareSideLength / 2;
		slr.y -= sqareSideLength / 2;

		Vector3 sul = sc3.Clone();
		sul.x -= sqareSideLength / 2;
		sul.y += sqareSideLength / 2;

		Vector3 sur = sc3.Clone();
		sur.x += sqareSideLength / 2;
		sur.y += sqareSideLength / 2;

		Debug.DrawLine( sll, slr, c );
		Debug.DrawLine( slr, sur, c );
		Debug.DrawLine( sur, sul, c );
		Debug.DrawLine( sul, sll, c );
	}

	void debugDraw()
	{
		foreach (var sc2 in squareCenters_ )
		{
			Vector3 sc3 = transform.position.Clone() + sc2.ZeroFill();
			debugDrawSquare( sc3, Color.white );
		}
	}

	// Update is called once per frame
	void Update ()
	{
		debugDraw();

		timeIntoCurrentPair_ += Time.deltaTime;

		if ( timeIntoCurrentPair_ > TimePerPair )
		{
			squareFrom_ = squareTo_;
			squareTo_ = getNextSquare();
			timeIntoCurrentPair_ = timeIntoCurrentPair_ - TimePerPair;
		}

		var lerpFactor = Mathf.Sin( (timeIntoCurrentPair_ / TimePerPair)*(Mathf.PI/2) );

		var result = Vector2.Lerp( squareFrom_, squareTo_, lerpFactor );
		Vector3 sc3 = transform.position.Clone() + result.ZeroFill();

		debugDrawSquare( transform.position.Clone() + squareFrom_.ZeroFill(), Color.blue );
		debugDrawSquare( transform.position.Clone() + squareTo_.ZeroFill(), Color.green );
		debugDrawSquare( sc3, Color.red );
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

		float upperLeftY = ( sqareSideLength / 2 ) * ( squaresInRow - 1 );
		float upperLeftX = -upperLeftY;

		squareCenters_.Clear();

		for (int i = 0; i < squaresInRow; i++ )
		{
			for (int j = 0; j < squaresInRow; j++ )
			{
				var newSquareCenter = new Vector2( upperLeftX + ( i * sqareSideLength ), upperLeftY - ( j * sqareSideLength ) );
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
