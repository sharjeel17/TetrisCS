using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
            {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
            };

        private readonly Image[,] imageControls;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        //add 25x25 pixels to each cell in the canvas (black grid)
        private Image[,] SetupGameCanvas(gameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellsize = 25; //20x10 canvas + 2x10 invisible rows 

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    Image imageControl = new Image()
                    {
                        Width = cellsize,
                        Height = cellsize
                    };

                    Canvas.SetTop(imageControl, (row - 2) * cellsize);
                    Canvas.SetLeft(imageControl, col * cellsize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[row, col] = imageControl;
                }
            }
            return imageControls;
        }

        //draw the grid and place images depending on what the numbers are in the grid
        private void DrawGrid(gameGrid grid)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Columns; col++)
                {
                    int id = grid[row, col];
                    imageControls[row, col].Opacity = 1;
                    imageControls[row, col].Source = tileImages[id];
                }
            }
        }

        //draw holding block
        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions()) 
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue) 
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.id];
        }

        private void DrawGhostBlock(Block block) 
        {
            int dropDistance = gameState.BlockDropDistance();
            foreach (Position p in block.TilePositions()) 
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.id];
            }
        }
        //update and draw the grids and blocks of the game
        private void Draw(GameState game)
        {
            DrawGrid(game.GameGrid);
            DrawGhostBlock(game.CurrentBlock);
            DrawBlock(game.CurrentBlock);
            DrawNextBlock(game.Queue);
            ScoreText.Text = $"Score: {game.Score}";
        }

        //async method to loop game and move block down every 0.5sec
        //await makes thread non-blocking hence other functions in game run while task is awaited
        private async Task GameLoop() 
        {
            Draw(gameState);

            while (!gameState.GameOver)
            {
                int delay = Math.Max(100, 1000 - (gameState.Score * 10));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Final score: {gameState.Score}";
        }

        //Perform certain actions depending on which key is pressed down
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver) return;
            switch (e.Key) 
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right: gameState.MoveBlockRight(); 
                    break;
                case Key.Down: gameState.MoveBlockDown(); 
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);

        }

        //draw game once canvas is loaded
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //SoundPlayer musicPlayer = new SoundPlayer();
            //musicPlayer.SoundLocation = "song1.wav";
            //musicPlayer.Play();
            await GameLoop();
        }

        //play again menu is hidden and game starts again
        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();

        }
    }
}
