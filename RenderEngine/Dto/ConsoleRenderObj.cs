using TerminalSnake.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderEngine.Dto
{
    public class ConsoleRenderObj
    {
        public ConsoleColors color { set; get; } = ConsoleColors.White;
        public char character { set; get; }
}
}
