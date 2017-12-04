using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Aimer : MonoBehaviour, Reciever
{

	public float squareSideLength = 1;
	public int squaresInRow = 1;
	public float cycleDuration = 1;
	public Texture2D crosshairImage;
	public Texture2D ammoImage;

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
		aSource_ = GetComponent<AudioSource>();
		changeParameter( Aimer.modeList[ammo_] );
	}

	bool giveAmmo(int amount)
	{
		if (ammo_ >= maxAmmo )
		{
			return false;
		}

		ammo_ += amount;
		ammo_ = Mathf.Min( ammo_, maxAmmo );
		changeParameter( Aimer.modeList[ammo_] );

		return true;
	}

	void OnValidate ()
	{
		changeParameter( squareSideLength, squaresInRow, cycleDuration );
	}

	void changeParameter( Mode mode )
	{
		changeParameter( mode.squareSideLength, mode.squaresInRow, mode.cycleDuration );
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

	void debugDrawSquares()
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

		debugDrawSquares();

		if ( Input.GetButtonDown( "Fire1" ) && ammo_ > 0 )
		{
			aSource_.Play();
			//Vector3 wayBackFirePoint = transform.position.Clone() + ( -transform.forward * 100000 );
			Vector3 gridWorldPoint = transform.TransformPoint( getCurrentPos().ZeroFill() );
			//Debug.DrawRay( Camera.main.transform.position, (gridWorldPoint - Camera.main.transform.position ) *25, Color.magenta, 10 );

			RaycastHit info;
			if ( Physics.Raycast( Camera.main.transform.position, gridWorldPoint - Camera.main.transform.position, out info ) )
			{
				info.transform.SendMessage( "Hit", SendMessageOptions.DontRequireReceiver );
			}

			ammo_--;
			changeParameter( modeList[ammo_] );
		}
	}

	void OnGUI ()
	{
		{
			// Draw crosshair
			int size = crosshairImage.width / 2;
			Vector3 screenPos = Camera.main.WorldToScreenPoint( transform.TransformPoint( getCurrentPos().ZeroFill() ) );
			GUI.DrawTexture( new Rect( screenPos.x - ( size / 2 ), ( Camera.main.pixelHeight - screenPos.y ) - ( size / 2 ), size, size ), crosshairImage );
		}

		{
			// Draw ammo display
			int size = ammoImage.width/2;
			int xpos = Camera.main.pixelWidth - ( 2*size );
			int ypos = Camera.main.pixelHeight - ( 2*size );
			int spacing = size/3;
			for ( int i = 0; i < ammo_; i++ )
			{
				GUI.DrawTexture( new Rect( xpos, ypos, size, size ), ammoImage );
				ypos -= size + spacing;
			}
		}


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

	public bool recieve ()
	{
		 return giveAmmo( 1 );
	}

	public struct Mode
	{
		public float squareSideLength;
		public int squaresInRow;
		public float cycleDuration;
	}

	public static int maxAmmo = 5;

	public static List<Mode> modeList = new List<Mode>
	{
		new Mode { squareSideLength = 0.25f, squaresInRow = 1, cycleDuration = 1  },
		new Mode { squareSideLength = 0.25f, squaresInRow = 1, cycleDuration = 1  },
		new Mode { squareSideLength = 0.25f, squaresInRow = 2, cycleDuration = 3  },
		new Mode { squareSideLength = 0.25f, squaresInRow = 3, cycleDuration = 5  },
		new Mode { squareSideLength = 0.25f, squaresInRow = 5, cycleDuration = 8  },
		new Mode { squareSideLength = 0.25f, squaresInRow = 7, cycleDuration = 11  },
	};

	private AudioSource aSource_;
	private int ammo_ = maxAmmo;
	private Vector2 squareFrom_;
	private Vector2 squareTo_;
	private float timeIntoCurrentPair_ = 0;
	private List<Vector2> squareCenters_ = new List<Vector2>();
	private List<int> currentIterationOrder_ = new List<int>();


}
