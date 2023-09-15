public class PositionEnum
{
    public int Row { get;}
    public int Col { get;}

    public PositionEnum(int row, int col)
    {
        Row = row; 
        Col = col;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is PositionEnum other)
        {
            return Row == other.Row && Col == other.Col;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return 8 * Row + Col;
    }
}

