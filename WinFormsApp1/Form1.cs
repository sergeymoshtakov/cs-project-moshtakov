using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private const int boardWidth = 10;
        private const int boardHeight = 20;
        private const int blockSize = 20;
        private const int timerInterval = 500; // in milliseconds
        private readonly Color[] blockColors = { Color.Black, Color.Orange, Color.Blue, Color.Red, Color.Green, Color.Purple, Color.Teal, Color.Yellow };
        private int[,] board = new int[boardHeight, boardWidth];
        private int currentX = boardWidth / 2;
        private int currentY = 0;
        private int currentShape;
        private int rotation;
        private Timer timer = new Timer();
        private Random random = new Random();

        private readonly int[][][] shapes = {
            new int[][] { new int[] { 1, 1, 1, 1 } }, // I
            new int[][] { new int[] { 1, 1, 0 }, new int[] { 0, 1, 1 } }, // Z
            new int[][] { new int[] { 0, 1, 1 }, new int[] { 1, 1, 0 } }, // S
            new int[][] { new int[] { 1, 1, 1 }, new int[] { 0, 0, 1 } }, // J
            new int[][] { new int[] { 1, 1, 1 }, new int[] { 1, 0, 0 } }, // L
            new int[][] { new int[] { 1, 1, 0 }, new int[] { 0, 1, 1 } }, // O
            new int[][] { new int[] { 0, 1, 0 }, new int[] { 1, 1, 1 } } // T
        };

        public Form1()
        {
            InitializeComponent();
            timer.Interval = timerInterval;
            timer.Tick += new EventHandler(OnTimerTick);
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    MoveLeft();
                    break;
                case Keys.Right:
                    MoveRight();
                    break;
                case Keys.Down:
                    MoveDown();
                    break;
                case Keys.Up:
                    Rotate();
                    break;
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    int colorIndex = board[y, x];
                    if (colorIndex > 0)
                    {
                        SolidBrush brush = new SolidBrush(blockColors[colorIndex]);
                        e.Graphics.FillRectangle(brush, new Rectangle(x * blockSize, y * blockSize, blockSize, blockSize));
                    }
                }
            }

            int[][] currentShapeData = shapes[currentShape];
            for (int i = 0; i < currentShapeData.Length; i++)
            {
                for (int j = 0; j < currentShapeData[i].Length; j++)
                {
                    if (currentShapeData[i][j] > 0)
                    {
                        SolidBrush brush = new SolidBrush(blockColors[currentShape + 1]);
                        e.Graphics.FillRectangle(brush, new Rectangle((currentX + j) * blockSize, (currentY + i) * blockSize, blockSize, blockSize));
                    }
                }
            }
        }

        private void MoveLeft()
        {
            if (CanMove(currentShape, rotation, currentX - 1, currentY))
            {
                currentX--;
                Refresh();
            }
        }

        private void MoveRight()
        {
            if (CanMove(currentShape, rotation, currentX + 1, currentY))
            {
                currentX++;
                Refresh();
            }
        }

        private void MoveDown()
        {
            if (CanMove(currentShape, rotation, currentX, currentY + 1))
            {
                currentY++;
            }
            else
            {
                // Lock the current shape into the board
                int[][] currentShapeData = shapes[currentShape];
                for (int i = 0; i < currentShapeData.Length; i++)
                {
                    for (int j = 0; j < currentShapeData[i].Length; j++)
                    {
                        if (currentShapeData[i][j] > 0)
                        {
                            board[currentY + i, currentX + j] = currentShape + 1;
                        }
                    }
                }

                // Check for completed lines
                for (int i = 0; i < boardHeight; i++)
                {
                    bool completed = true;
                    for (int j = 0; j < boardWidth; j++)
                    {
                        if (board[i, j] == 0)
                        {
                            completed = false;
                            break;
                        }
                    }
                    if (completed)
                    {
                        for (int j = 0; j < boardWidth; j++)
                        {
                            board[i, j] = 0;
                        }
                        for (int k = i - 1; k >= 0; k--)
                        {
                            for (int j = 0; j < boardWidth; j++)
                            {
                                board[k + 1, j] = board[k, j];
                            }
                        }
                    }
                }

                // Check for game over
                if (!CanMove(0, 0, boardWidth / 2, 0))
                {
                    timer.Stop();
                    MessageBox.Show("Game over!");
                }

                // Spawn a new shape
                currentX = boardWidth / 2;
                currentY = 0;
                currentShape = random.Next(shapes.Length);
                rotation = 0;
            }
            Refresh();
        }

        private void Rotate()
        {
            int newRotation = (rotation + 1) % shapes[currentShape].Length;
            if (CanMove(currentShape, newRotation, currentX, currentY))
            {
                rotation = newRotation;
                Refresh();
            }
        }

        private bool CanMove(int shapeIndex, int rotation, int x, int y)
        {
            int[][] shapeData = shapes[shapeIndex];
            for (int i = 0; i < shapeData.Length; i++)
            {
                for (int j = 0; j < shapeData[i].Length; j++)
                {
                    if (shapeData[i][j] > 0)
                    {
                        int newX = x + j;
                        int newY = y + i;
                        if (newX < 0 || newX >= boardWidth || newY >= boardHeight || board[newY, newX] > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}


