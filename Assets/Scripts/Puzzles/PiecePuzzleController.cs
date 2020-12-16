using UnityEngine;

public class PiecePuzzleController : PuzzleBase, IInteractable
{
	enum Direction { Up, Left, Down, Right, Null };

	[SerializeField] private GameObject piecePrefab = null;
	[SerializeField] private GameObject firstPiece = null;
	[SerializeField] private Texture[] textures = null;
	[SerializeField] private Material emptyMat = null;
	[SerializeField] private Camera puzzleCamera = null;
	[SerializeField] private LayerMask pieceLayermask;
	[SerializeField] private float pieceSpacing = 0.08f;
	[SerializeField] private int rows = 3;
	[SerializeField] private int columns = 3;

	private Piece[,] pieces = new Piece[0, 0];
	private bool isPlaying = false;
	private GameObject player = null;

	private void Awake()
	{
		// Setup pieces by instantiating and then assigning postion and textures.
		Collider firstPieceCollider = this.firstPiece.GetComponent<Collider>();
		float xOffset = firstPieceCollider.bounds.size.x + this.pieceSpacing;
		float zOffset = firstPieceCollider.bounds.size.z + this.pieceSpacing;
		
		this.pieces = new Piece[this.rows, this.columns];
		int textureIndex = 0;
		for(int row = 0; row < this.pieces.GetLength(0); row++)
		{
			for(int col = 0; col < this.pieces.GetLength(1); col++, textureIndex++)
			{
				GameObject newPiece = GameObject.Instantiate<GameObject>(this.piecePrefab,
					this.firstPiece.transform.position, this.firstPiece.transform.rotation);

				newPiece.transform.SetParent(this.transform);

				newPiece.transform.position += new Vector3(col * xOffset, 0.0f, -row * zOffset);
				
				this.pieces[row, col] = newPiece.GetComponent<Piece>();
				this.pieces[row, col].SetGridIndex(new GridIndex(row, col));
				this.pieces[row, col].SetInitialTexture(this.textures[textureIndex]);
			}
		}

		this.pieces[this.rows - 1, this.columns - 1].SetEmpty(emptyMat);
	}

	private void Start()
	{
		this.RandomizeBoard();
	}

	private void Update() 
	{
		if(!this.isPlaying)
			return;

		// Interact with a piece.
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			RaycastHit hit;
			Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit, 10f, this.pieceLayermask))
			{
				this.MoveToFreeSpace(hit.transform.GetComponent<Piece>());

				if(this.IsComplete())
					this.OnComplete();
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape))
			this.ChangePlayState(false, this.player);
	}

    protected override void OnComplete()
    {
        base.OnComplete();

		this.ChangePlayState(false, this.player);
    }

	private void RandomizeBoard()
	{
		// Attempt to move a random piece 200 times (creates a randomized puzzle).
		for (int i = 0; i < 200; i++)
		{
			int randRow = Random.Range(0, this.rows);
			int randCol = Random.Range(0, this.columns);
			this.MoveToFreeSpace(this.pieces[randRow, randCol]);
		}
	}

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		if(this.isComplete)
			return;

		// "play" the puzzle and remove self from the game temporarily.
		this.ChangePlayState(true, interactor.transform.parent.gameObject);
	}

	// Hide the player and play the puzzle game by enabling the camera.
	public void ChangePlayState(bool isPlaying, GameObject player)
	{
		this.player = player;
		this.isPlaying = isPlaying;

		this.player.gameObject.SetActive(!this.isPlaying);
		this.puzzleCamera.gameObject.SetActive(this.isPlaying);
		this.GetComponent<BoxCollider>().enabled = !this.isPlaying;
	}

	private void MoveToFreeSpace(Piece piece)
	{
		this.MovePiece(piece, this.GetFirstFreeDirection(piece));
	}

	private void MovePiece(Piece piece, Direction direction)
	{
		Piece newPiece = null;

		switch(direction) 
		{
			case Direction.Up:
				newPiece = this.pieces[piece.index.row - 1, piece.index.col];
				break;

			case Direction.Left:
				newPiece = this.pieces[piece.index.row, piece.index.col - 1];
				break;

			case Direction.Down:
				newPiece = this.pieces[piece.index.row + 1, piece.index.col];
				break;

			case Direction.Right:
				newPiece = this.pieces[piece.index.row, piece.index.col + 1];
				break;
		}

		if(newPiece != null)
		{
			newPiece.SetTexture(piece.material.mainTexture);
			piece.SetEmpty(this.emptyMat);
		}
	}

	private Direction GetFirstFreeDirection(Piece piece)
	{
		if(this.IsFreeRightOf(piece))
			return Direction.Right;
		if(this.IsFreeLeftOf(piece))
			return Direction.Left;
		if(this.IsFreeAbove(piece))
			return Direction.Up;
		if(this.IsFreeBelow(piece))
			return Direction.Down;

		return Direction.Null;
	}

	private bool IsFreeRightOf(Piece piece)
	{
		if(piece.index.col + 1 >= this.pieces.GetLength(1))
			return false;

		return (this.pieces[piece.index.row, piece.index.col + 1].isEmpty);
	}

	private bool IsFreeLeftOf(Piece piece)
	{
		if(piece.index.col - 1 < 0)
			return false;

		return (this.pieces[piece.index.row, piece.index.col - 1].isEmpty);
	}

	private bool IsFreeAbove(Piece piece)
	{
		if(piece.index.row - 1 < 0)
			return false;

		return (this.pieces[piece.index.row - 1, piece.index.col].isEmpty);
	}

	private bool IsFreeBelow(Piece piece)
	{
		if(piece.index.row + 1 >= this.pieces.GetLength(0))
			return false;

		return (this.pieces[piece.index.row + 1, piece.index.col].isEmpty);
	}

	private bool IsComplete()
	{
		int textureIndex = 0;
		for(int row = 0; row < this.pieces.GetLength(0); row++)
		{
			for(int col = 0; col < this.pieces.GetLength(1); col++, textureIndex++)
			{
				if(this.pieces[row, col].material.mainTexture != this.textures[textureIndex])
					return false;
			}
		}

		return true;
	}
}
