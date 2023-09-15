using System.Collections.Generic;

public class GameState
{
    public const int Rows = 8;
    public const int Cols = 8;

    public Player[,] Board { get;}
    public Dictionary<Player, int> DiscCount { get;}
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }
    public Dictionary<PositionEnum, List<PositionEnum>> LegalMoves { get;private set; }

    public GameState()
    {
        Board = new Player[Rows, Cols];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;

        DiscCount = new Dictionary<Player, int>()
        {
            {Player.Black, 2}, 
            {Player.White, 2}
        };
        CurrentPlayer = Player.Black;
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    public bool MakeMove(PositionEnum pos, out MoveInfoEnum moveInfo)
    {
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }

        Player movePlayer = CurrentPlayer;
        List<PositionEnum> outflanked = LegalMoves[pos];

        Board[pos.Row, pos.Col] = movePlayer;

        FlipDiscs(outflanked);
        UpdateDiscCounts(movePlayer, outflanked.Count);
        PassTurn();

        moveInfo = new MoveInfoEnum { Player = movePlayer, Position = pos, Outflanked = outflanked };
        return true;
    }



    public IEnumerable<PositionEnum> OccupiedPositions()
    {
        for(int r = 0; r < Rows; r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                if (Board[r,c] != Player.None)
                {
                    yield return new PositionEnum(r,c);
                }
            }
        }
    }

    private void FlipDiscs(List<PositionEnum> positions)
    {
        foreach(PositionEnum pos in positions)
        {
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Oppoment();
        }
    }

    private void UpdateDiscCounts(Player movePlayer, int outflankedCount)
    {
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Oppoment()] -= outflankedCount;
    }

    private void ChangePlayer()
    {

        CurrentPlayer = CurrentPlayer.Oppoment();
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {
        if (DiscCount[Player.Black] > DiscCount[Player.White])
        {
            return Player.Black;
        }
        if (DiscCount[Player.White] > DiscCount[Player.Black])
        {
            return Player.White;
        }
        return Player.None;
    }

    private void PassTurn()
    {
        ChangePlayer();
        if (LegalMoves.Count > 0)
        {
            return;
        }

        ChangePlayer();

        if (LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None;
            GameOver = true;
            Winner = FindWinner();
        }
    }


    private bool isInsideBoard(int r,int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Cols;
    }

    private List<PositionEnum> OutflankInDir(PositionEnum pos, Player player, int rDelta, int cDelta)
    {
        List<PositionEnum> outflanked = new List<PositionEnum>();
        int r = pos.Row + rDelta;
        int c = pos.Col + cDelta;

        while(isInsideBoard(r,c) && Board[r,c] != Player.None) 
        {
            if (Board[r,c] == player.Oppoment()) 
            { 
                outflanked.Add(new PositionEnum(r,c));
                r += rDelta;
                c += cDelta;
            }
            else if (Board[r,c] == player)
            {
                return outflanked;
            }
        }

        return new List<PositionEnum>();
    }

    private List<PositionEnum> Outflanked(PositionEnum pos, Player player)
    {
        List<PositionEnum> outflanked = new List<PositionEnum>();
        
        for(int rDelta = -1; rDelta <= 1; rDelta++) 
        {
            for(int cDelta = -1; cDelta <=1; cDelta++)
            {
                if(rDelta == 0 && cDelta == 0) 
                {
                    continue;
                }

                outflanked.AddRange(OutflankInDir(pos,player,rDelta,cDelta));
            }
        }
        return outflanked;
    }

    private bool isMoveLegal(Player player, PositionEnum pos, out List<PositionEnum> outflanked) 
    {
        if (Board[pos.Row,pos.Col] != Player.None)
        {
            outflanked = null; 
            return false;
        }

        outflanked = Outflanked(pos, player);
        return outflanked.Count > 0;
    }
    private Dictionary<PositionEnum, List<PositionEnum>> FindLegalMoves(Player player)
    {
        Dictionary<PositionEnum, List<PositionEnum>> legalMoves = new Dictionary<PositionEnum, List<PositionEnum>>();
        for(int r = 0; r < Rows; r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                PositionEnum pos = new PositionEnum(r,c);

                if(isMoveLegal(player, pos, out List<PositionEnum> outflanked))
                {
                    legalMoves[pos] = outflanked;
                }
            }
        }
        return legalMoves;
    }
}
