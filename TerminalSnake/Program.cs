using TerminalSnake.Dto;
using TerminalSnake.GameLogic;
using TerminalSnake.Rendering;
using TerminalSnake.Util;
using Pastel;
using RenderEngine;
using System;
using System.Drawing;

namespace TerminalSnake
{
    //Numero di call 2
    public class Program
    {
        static BackgoundMusic   _backgoundMusic;
        static ConsoleConfig    _consoleConfig;
        static ConsoleRender    _consoleRender;
        static ConsoleOut       _consoleOut;
        static PlayerDto        _player;
        static EnemyDto         _enemy;
        static List<IGameLogic> _gameLogics;

        public static void Main(string[] args)
        {
            try
            {
                InitGame();

                var debugData = new List<string>();
                var headers = new List<string>();

                //Debug player movment
                //_player.playerBodies.Add(new PlayerBody
                //{
                //     pos_col = 5,
                //     pos_row = 4,
                //});
                //_player.playerBodies.Add(new PlayerBody
                //{
                //    pos_col = 5,
                //    pos_row = 3,
                //});
                //_player.playerBodies.Add(new PlayerBody
                //{
                //    pos_col = 5,
                //    pos_row = 2,
                //});

                do
                {
                    headers = new List<string>()
                    {
                        $"\t\t\t\tSNAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKE",
                        "",
                        $"Point: {_player.points}"
                    };

                    _gameLogics.ForEach(x => x.BeforeRender());
                    
                    #if DEBUG
                    debugData = new List<string>()
                    {
                        $"FieldSize: Row {_consoleConfig.n_row} - Col {_consoleConfig.n_column}",
                        $"FieldSize Usable:",
                        $"          Row: 1 - {_consoleConfig.n_row - 2}",
                        $"          Col: 1 - {_consoleConfig.n_column - 2}",
                        $"Player Position: {_player.pos_row} - {_player.pos_col} \t Enemy Position: {_enemy.pos_row} - {_enemy.pos_col}"
                    };    
                   
                    #endif

                    _consoleRender.Render(headers, debugData);
                    _gameLogics.ForEach(x => x.AfterRender());
                } while (true);
                             
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            }
            
        }

        private static void InitGame()
        {
            _consoleConfig = new ConsoleConfig();
            _consoleOut = new ConsoleOut(_consoleConfig, true);

            _consoleOut.ConsoleWriteLine("Init - render", ConsoleColors.White);

            _consoleRender = new ConsoleRender(_consoleConfig);
            _consoleRender.InitalizeRender();

            _consoleOut.ConsoleWriteLine("Init - render - Done", ConsoleColors.White);

            _consoleOut.ConsoleWriteLine("Init - GameLogics", ConsoleColors.White);
            _player = new PlayerDto();
            _enemy = new EnemyDto();
            _gameLogics = new List<IGameLogic>();

            foreach (Type gameLogic in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IGameLogic))))
            {
                if(gameLogic != null)
                {
                    var instance = (IGameLogic)Activator.CreateInstance(gameLogic);
                    if(instance != null)
                    {
                        _consoleOut.ConsoleWriteLine($"GameLogics - {instance.Name}", ConsoleColors.Blue);
                        instance.Initialize(_consoleConfig, _consoleOut, _consoleRender.GetRenderMatrix(), _player, _enemy);
                        _gameLogics.Add(instance);
                    }                   
                }               
            }
            _consoleOut.ConsoleWriteLine("Init - GameLogics - Done", ConsoleColors.White);

            _consoleOut.ConsoleWriteLine("Init - Music - Start", ConsoleColors.White);
            _backgoundMusic = new BackgoundMusic();
            _backgoundMusic.Play();
            _consoleOut.ConsoleWriteLine("Init - Music - Done", ConsoleColors.White);

            Thread.Sleep(1000);

            _backgoundMusic.PlayAsync();
            _consoleRender.Clean();
        }
    }
}