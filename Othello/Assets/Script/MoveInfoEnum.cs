using System.Collections.Generic;

public class MoveInfoEnum
{
    public Player Player { get; set; }
    public PositionEnum Position { get; set; }
    public List<PositionEnum> Outflanked { get; set; }
}
