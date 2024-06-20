using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshPro gameStateText;        // text about state of game
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private Transform topLeftAnchor;

    private GridCell[,] _boardArray = new GridCell[3, 3];
    private List<GridCell> optionList = new List<GridCell>();
    private int[] _gridMapper = { 1, 0, -1 };

    private string _activeSymbol;
    private bool _allowInput;

    private bool _playerOneIsAI;
    private bool _playerTwoIsAI;
    private bool _playerOneIsActive;
    private int _numMoves;

    private void Awake()
    {
        PopulateGrid();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (_allowInput)
        {
            if (_playerOneIsActive && _playerOneIsAI || !_playerOneIsActive && _playerTwoIsAI)
            {
                DoAIMove();
            }
            else
            {
                HandleInput();
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // check if we hit a blank space
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                GridCell gridCell = hit.transform.gameObject.GetComponent<GridCell>();
                if (gridCell != null && gridCell.isBlank())
                {
                    SelectCell(gridCell);
                }
            }
        }
    }

    void SelectCell(GridCell gridCell)
    {
        _numMoves++;
        gridCell.SetSymbol(_activeSymbol);

        ToggleActiveSymbol();

        UpdateGameTurnText(_activeSymbol + ", it is your turn.");

        if (CheckForWin())
        {
            _allowInput = false;

            StartCoroutine(EndGameWithDelay());
        }
    }

    // player gets a chance to see last move before return screen dims it
    IEnumerator EndGameWithDelay()
    {
        UpdateGameTurnText(winnerText.text);

        yield return new WaitForSeconds(3.0f);
        gameController.ShowReturnPanel();
    }


    void DoAIMove()
    {
        _allowInput = false;

        UpdateGameTurnText(_activeSymbol + " is being played by AI. Thinking...");

        StartCoroutine(ProcessAI());
    }

    IEnumerator ProcessAI()
    {
        // wait a little bit so it looks like we are thinking hard
        yield return new WaitForSeconds(3.0f);

        optionList.Clear();

        // get list of choices
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                if (_boardArray[row,column].isBlank())
                {
                    optionList.Add(_boardArray[row, column]);
                }
            }
        }

        bool didPick = false;
        // check each choice for the win!
        foreach (var cell in optionList)
        {
            cell.SetSymbol(_activeSymbol);
            if (AIWinCheck())
            {
                _allowInput = true;
                // selected it for real
                SelectCell(cell);
                didPick = true;
                break; // we are winners time to leave
            }
            cell.SetSymbol(" ");
        }

        // if no winner then pick randomly
        if (!didPick)
        {
            _allowInput = true;
            SelectCell(optionList[Random.Range(0, optionList.Count)]);
        }
    }

    void ToggleActiveSymbol()
    {
        _playerOneIsActive = !_playerOneIsActive;
        _activeSymbol = _activeSymbol.Equals("O") ? "X" : "O";
    }

    void UpdateGameTurnText(string newText)
    {
        gameStateText.text = newText;
    }

    void UpdateWinnerText(string newText)
    {
        winnerText.text = newText;
    }

    public bool CheckForWin()
    {
        string winningSymbol = CheckForRowWin();
        if (winningSymbol != null)
        {
            UpdateWinnerText(winningSymbol + " wins on a row.");
            return true;
        }

        winningSymbol = CheckColumnsForWin();
        if (winningSymbol != null)
        {
            UpdateWinnerText(winningSymbol + " wins on a column.");
            return true;
        }

        winningSymbol = CheckDiagonalsForWin();
        if (winningSymbol != null)
        {
            UpdateWinnerText(winningSymbol + " wins on diagonal!");
            return true;
        }

        // check for draw
        if (_numMoves >= 9)
        {
            UpdateWinnerText("The game is a draw!");
            return true;
        }

        return false;
    }

    public bool AIWinCheck()
    {
        if (CheckForRowWin() != null || CheckColumnsForWin() != null || CheckDiagonalsForWin() != null)
        {
            return true;
        }

        return false;
    }

    string CheckForRowWin()
    {
        // check rows
        for (int row = 0; row < 3; row++)
        {
            string startSymbol = _boardArray[row, 0].GetSymbol();
            if (startSymbol == _boardArray[row, 1].GetSymbol() && startSymbol == _boardArray[row, 2].GetSymbol() &&
                startSymbol != " ")
            {
                // we have a winning row!
                return startSymbol;
            }
        }

        return null;
    }

    string CheckColumnsForWin()
    {
        // check columns
        for (int col = 0; col < 3; col++)
        {
            string startSymbol = _boardArray[0, col].GetSymbol();
            if (startSymbol == _boardArray[1, col].GetSymbol() && startSymbol == _boardArray[2, col].GetSymbol() &&
                startSymbol != " ")
            {
                // we have a winning column!
                return startSymbol;
            }
        }

        return null;
    }

    // check diagonals
    string CheckDiagonalsForWin()
    {
        string diagSymbol = _boardArray[1, 1].GetSymbol();
        if (diagSymbol != " " && ((diagSymbol == _boardArray[0, 0].GetSymbol() && diagSymbol == _boardArray[2, 2].GetSymbol())
            || (diagSymbol == _boardArray[0, 2].GetSymbol() && diagSymbol == _boardArray[2, 0].GetSymbol())))
        {
            // we have a winning diagonal!
            return diagSymbol;
        }

        return null;
    }

    // Populate using untiy grid top left corner is -1,1
    void PopulateGrid()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                Vector3Int vInt = new Vector3Int(column - 1, _gridMapper[row]);
                Vector3 startPosition = grid.CellToWorld(vInt);
                _boardArray[row, column] = Instantiate(gridCellPrefab, startPosition, Quaternion.identity, gameObject.transform).GetComponent<GridCell>();
            }
        }
    }

    public void Reset(bool playerOneIsAI, bool playerTwoIsAI)
    {
        gameObject.SetActive(true);

        _playerOneIsAI = playerOneIsAI;
        _playerTwoIsAI = playerTwoIsAI;

        foreach (var cell in _boardArray)
        {
            cell.Reset();
        }

        // x is always first
        _activeSymbol = "X";
        _numMoves = 0;

        // decide what player gets to be X and first
        _playerOneIsActive = Random.Range(0, 2) == 0 ? true : false;

        string turnText = _playerOneIsActive ? "Player 1" : "Player 2";

        UpdateGameTurnText(turnText + " is first. Playing with symbol " + _activeSymbol);
        _allowInput = true;

    }
}
