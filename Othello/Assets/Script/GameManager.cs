using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Disc discBlackUp;
    [SerializeField] private Disc discWhiteUp;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private UIManager uIManager;

    private Dictionary<Player, Disc> discPrebabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private Disc[,] discs = new Disc[8, 8];
    private bool canMove = true;
    private List<GameObject> highlights = new List<GameObject>();
    private void Start()
    {
        discPrebabs[Player.White] = discWhiteUp;
        discPrebabs[Player.Black] = discBlackUp;
        

        AddStartDiscs();
        ShowLegalMoves();
        uIManager.setPlayerText(gameState.CurrentPlayer);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                PositionEnum boardPos = SceneToBoardPos(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach(PositionEnum boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
            GameObject highlight = Instantiate(highlightPrefab,scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private void OnBoardClicked(PositionEnum boardPos)
    {
        if (!canMove)
        {
            return;
        }
        if (gameState.MakeMove(boardPos, out MoveInfoEnum moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfoEnum moveInfo)
    {
        canMove= false;
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return showTurnOutcome(moveInfo);
        ShowLegalMoves();
        canMove= true;
    }

    private PositionEnum SceneToBoardPos(Vector3 scenePos)
    {
        int col = (int)(scenePos.x - 0.25f);
        int row = 7 - (int)(scenePos.z - 0.25f);
        return new PositionEnum(row, col);
    }

    private Vector3 BoardToScenePos(PositionEnum boardPos)
    {
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.Row + 0.75f);
    }
    private void SpawnDisc(Disc prefab, PositionEnum boardPos)
    {
        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenePos,Quaternion.identity);
    }
    private void AddStartDiscs()
    {
        foreach(PositionEnum boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDisc(discPrebabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<PositionEnum> positions)
    {
        foreach(PositionEnum boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfoEnum moveInfo)
    {
        SpawnDisc(discPrebabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uIManager.SetSkippedText(skippedPlayer);
        yield return uIManager.AnimateTopText();
    }

    private IEnumerator ShowGameOver(Player winner)
    {
        uIManager.SetTopText("Niether Player Can Move");
        yield return uIManager.AnimateTopText();
        yield return uIManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);
        
        yield return ShowCounting();
        
        uIManager.setWinnerText(winner);
        yield return uIManager.ShowEndScreen();
    }

    private IEnumerator showTurnOutcome(MoveInfoEnum moveInfo)
    {
        if(gameState.GameOver)
        {
            yield return ShowGameOver(gameState.Winner);
            yield break;
        }
        Player currentPlayer = gameState.CurrentPlayer;

        if(currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Oppoment());
        }
        uIManager.setPlayerText(currentPlayer);
    }

    private IEnumerator ShowCounting()
    {
        int black = 0; 
        int white = 0;

        foreach(PositionEnum pos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[pos.Row, pos.Col];
            if(player == Player.Black)
            {
                black++;
                uIManager.SetBlackScoreText(black);
            }else if (player == Player.White)
            {
                white++;
                uIManager.SetWhiteScoreText(white);
            }
            discs[pos.Row, pos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame()
    {
        yield return uIManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }
}
