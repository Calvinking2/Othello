public enum Player
{
    None, Black, White
}

public static class PlayerExtensions
{
    public static Player Oppoment(this Player player)
    {
        if(player == Player.White) {
            return Player.Black;
        }
        else if(player == Player.Black)
        {
            return Player.White;
        }
        return Player.None;
    }
}