using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class Game : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public int mineCount = 3;

    private Board board;
    private CellGrid grid;
    private bool gameover;
    private bool generated;

    private float multiplier = 1.0f;
    private int playerCoins = 1000;
    private int betAmount = 0;

    public GameObject betInputUI;
    public InputField betInputField;
    public Text coinsText;
    public Text multiplierText;
    public GameObject cashOutButton;
    public Button increaseBetButton;
    public Button decreaseBetButton;
    public Text greetingText;

    public GameObject restartButton;

    public RotatingCoin rotatingCoin;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        ShowBetInput();
        UpdateUI();
        restartButton.SetActive(false);
    }

    private void ShowBetInput()
    {
        betInputUI.SetActive(true);
        cashOutButton.SetActive(false);
        board.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);
        greetingText.gameObject.SetActive(true);

        // Enable and show the bet adjustment buttons
        increaseBetButton.gameObject.SetActive(true);
        decreaseBetButton.gameObject.SetActive(true);
        increaseBetButton.interactable = true;
        decreaseBetButton.interactable = true;

        if (rotatingCoin != null)
        {
            rotatingCoin.StartRotation();
        }

        UpdateUI();
    }

    public void PlaceBet()
    {
        if (int.TryParse(betInputField.text, out betAmount) && betAmount > 0 && betAmount <= playerCoins)
        {
            playerCoins -= betAmount;
            multiplier = 1.0f;
            NewGame();

            betInputUI.SetActive(false);
            greetingText.gameObject.SetActive(false);
            board.gameObject.SetActive(true);
            multiplierText.gameObject.SetActive(true);

            // Disable and hide the bet adjustment buttons
            increaseBetButton.gameObject.SetActive(false);
            decreaseBetButton.gameObject.SetActive(false);
            increaseBetButton.interactable = false;
            decreaseBetButton.interactable = false;
        }
        else
        {
            Debug.LogError("Invalid bet amount.");
        }
    }


    private void NewGame()
    {
        StopAllCoroutines();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        gameover = false;
        generated = false;

        grid = new CellGrid(width, height);
        board.Draw(grid);

        cashOutButton.SetActive(true);
        multiplierText.gameObject.SetActive(true);

        UpdateUI();
    }

    private void Update()
    {
        if (!gameover)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        coinsText.text = $"{playerCoins}";
        multiplierText.text = $"Multiplier: x{multiplier:F1}";
    }

    private void Reveal()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (TryGetCellAtMousePosition(out Cell cell))
        {
            if (!generated)
            {
                grid.GenerateMines(cell, mineCount);
                grid.GenerateNumbers();
                generated = true;
            }

            Reveal(cell);
        }
        else
        {
            Debug.Log("No valid cell found at clicked position.");
        }
    }

    private void Reveal(Cell cell)
    {
        if (cell.revealed) return;

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            default:
                cell.revealed = true;
                multiplier += 0.5f;
                CheckWinCondition();
                break;
        }

        board.Draw(grid);
        UpdateUI();
    }

    private void Explode(Cell cell)
    {
        gameover = true;
        multiplier = 0;
        Debug.Log($"You hit a mine! You lost your bet of {betAmount} coins.");
        cell.exploded = true;
        cell.revealed = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = grid[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                }
            }
        }

        restartButton.SetActive(true);
        UpdateUI();
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        gameover = true;
        float winnings = GetWinnings();
        Debug.Log($"You revealed all safe tiles! You win {winnings} coins!");
        UpdateUI();
    }


    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        cell = null;

        if (board == null || board.tilemap == null)
        {
            Debug.LogWarning("Board or Tilemap is not initialized. Ignoring click.");
            return false;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);

        if (grid == null)
        {
            Debug.LogWarning("Grid is not initialized. Ignoring click.");
            return false;
        }

        if (cellPosition.x >= 0 && cellPosition.x < grid.Width &&
            cellPosition.y >= 0 && cellPosition.y < grid.Height)
        {
            return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
        }

        Debug.LogWarning("Clicked position is outside the grid bounds. Ignoring click.");
        return false;
    }

    public void RestartGame()
    {
        betAmount = 0;
        multiplier = 1.0f;

        restartButton.SetActive(false);
        ShowBetInput();

        if (rotatingCoin != null)
        {
            rotatingCoin.StartRotation();
        }
    }
    public void IncreaseBet()
    {
        int currentBet = 0;
        if (int.TryParse(betInputField.text, out currentBet))
        {
            if (currentBet + 10 <= playerCoins)
            {
                currentBet += 10;
                betInputField.text = currentBet.ToString();
            }
        }
        else
        {
            currentBet = 10;
            betInputField.text = currentBet.ToString();
        }
    }

    public void DecreaseBet()
    {
        int currentBet = 0;
        if (int.TryParse(betInputField.text, out currentBet))
        {
            if (currentBet - 10 >= 0)
            {
                currentBet -= 10;
                betInputField.text = currentBet.ToString();
            }
        }
        else
        {
            currentBet = 0;
            betInputField.text = currentBet.ToString();
        }
    }

    public void CashOut()
    {
        if (!gameover)
        {
            float winnings = GetWinnings();
            playerCoins += Mathf.CeilToInt(winnings);
            Debug.Log($"You cashed out! You won {winnings} coins. Total coins: {playerCoins}");
            gameover = true;
            ShowBetInput();
        }
    }

    public float GetWinnings()
    {
        if (gameover)
        {
            return betAmount * multiplier;
        }
        else
        {
            Debug.LogWarning("Game is not over yet. Winnings cannot be calculated.");
            return 0f;
        }
    }

}
