using TerminalSnake.Rendering;


namespace TerminalSnake.Dto
{
    public class PlayerDto
    {
        public int points { get; set; } = 0;
        public bool isDead { get; set; } = false;

        public int pos_row { get; set; } = 5;
        public int pos_col { get; set; } = 5;

        public MovmentDirection movmentDirection { get; set; }

        public List<PlayerBody> playerBodies { get; set; } = new List<PlayerBody>();

        public void AddBody()
        {
            if(this.playerBodies == null)
                this.playerBodies = new List<PlayerBody>();

            var b = new PlayerBody()
            {
                pos_col = this.pos_col,
                pos_row = this.pos_row
            };
            this.playerBodies.Add(b);
        }

        public void AddScore(int point)
        {
            this.points = this.points + point;
        }
    }

    
    public class PlayerBody
    {
        public int pos_row { get; set; } = 1;
        public int pos_col { get; set; } = 1;
    }

    public enum MovmentDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
