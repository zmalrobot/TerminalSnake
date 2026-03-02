using TerminalSnake.Rendering;


namespace TerminalSnake.Dto
{
    public class EnemyDto
    {
        public bool isActive { get; set; } = false;
        public int point { get; set; } = 10;

        public int pos_row { get; set; } = 5;
        public int pos_col { get; set; } = 5;

        public char enemy_char { get; set; } = '░';
        public ConsoleColors enemy_color { get; set; } = ConsoleColors.Red;
    }


 
}
