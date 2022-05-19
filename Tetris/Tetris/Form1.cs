using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SHAPES;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Media;
using System.Threading;

namespace Tetris
{
    [Serializable]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            loadCanvas();

            currentShape = getRandomShapeWithCenterAligned();

            nextShape = getNextShape();

            timer1.Tick += timer1_Tick;

            timer1.Start();

            playMusic();
        }

        //----------------------Variables----------------------
        Bitmap canvasBitmap;
        Bitmap nextShapeBitmap;
        Bitmap workingBitmap;
        Graphics canvasGraphics;
        Graphics workingGraphics;
        Graphics nextShapeGraphics;
        Pen darkPen = new Pen(Color.FromArgb(38, 66, 139));
        Shape currentShape;
        Shape nextShape;
        LikeShape likeEqual = new LikeShape();
        Color[] colorsArr = new Color[9]
           {
                Color.Transparent,Color.Gray,
                Color.FromArgb(249, 240, 170),
                Color.FromArgb(150, 200, 228),
                Color.FromArgb(230, 180, 232),
                Color.FromArgb(244, 205, 140),
                Color.FromArgb(130, 218, 204),
                Color.FromArgb(234, 150, 183),
                Color.FromArgb(205, 241, 140)
           };

        int canvasWidth = 15;
        int canvasHeight = 20;
        ID[,] canvasDotArray;
        int dotSize = 20;
        int currentX;
        int currentY;
        int score;



        //----------------------Shape Related----------------------
        //----------------------DRAWING is used----------------------
        private void loadCanvas()
        {
            //Resize the picture box based on dot size and canvas size
            pictureBox1.Width = canvasWidth * dotSize;
            pictureBox1.Height = canvasHeight * dotSize;

            //Create Bitmap with picture box's size and draw it
            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            canvasGraphics = this.CreateGraphics();
            canvasGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            canvasGraphics.FillRectangle(Brushes.Gray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);

            //Load Bitmap into picture box
            pictureBox1.Image = canvasBitmap;

            //Initialize canvas dot array, all elements=0 by default
            canvasDotArray = new ID[canvasWidth, canvasHeight];
            for (int i = 0; i < canvasHeight; i++)
            {
                for (int j = 0; j < canvasWidth; j++)
                {
                    canvasDotArray[j, i] = ID.empty;
                }
            }

            //The dot array determines which blocks are filled with shapes
            //000011000
            ///11111000
            ///00000000
            ///this is an example of L shape 
        } //IS FOR THE CANVAS
        private void drawShape()
        {
            workingBitmap = new Bitmap(canvasBitmap);
            workingGraphics = Graphics.FromImage(workingBitmap);
            workingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SolidBrush br;

            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    br = new SolidBrush(colorsArr[(int)currentShape.dots[j, i]]);
                    if (currentShape.dots[j, i] != ID.empty)
                    {
                        workingGraphics.FillRectangle(br, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                        workingGraphics.DrawRectangle(darkPen, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                        if (currentShape.CircleFilled == true)
                        {
                            workingGraphics.FillEllipse(Brushes.CornflowerBlue, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                        }
                        if(currentShape.TriangleFilled == true)
                        {
                            Point[] pointsArray = new Point[3]
                            {
                                new Point((currentX +  (i + 1)) * dotSize, (currentY +j)* dotSize),
                                new Point((currentX + i) * dotSize, (currentY+ (j + 1)) * dotSize),
                                new Point((currentX + i) * dotSize ,(currentY + j) * dotSize)
                            };
                            workingGraphics.FillPolygon(Brushes.DarkSeaGreen, pointsArray);
                        }
                    }
                    
                }
            }

            pictureBox1.Image = workingBitmap;
        } //IS FOR THE FALLING SHAPE
        private Shape getNextShape()
        {
            Shape shape = getRandomShapeWithCenterAligned();
            SolidBrush br1 = new SolidBrush(shape.ShapeColor);
            SolidBrush br2;
            // Codes to show the next shape in the side panel
            nextShapeBitmap = new Bitmap(6 * dotSize, 6 * dotSize);
            nextShapeGraphics = Graphics.FromImage(nextShapeBitmap);

            nextShapeGraphics.FillRectangle(Brushes.MediumTurquoise, 0, 0, nextShapeBitmap.Width, nextShapeBitmap.Height); //for the minibox color
            nextShapeGraphics.DrawRectangle(Pens.MediumTurquoise, 0, 0, nextShapeBitmap.Width, nextShapeBitmap.Height);

            // Find the ideal position for the shape in the side panel
            var startX = (6 - shape.Width) / 2;
            var startY = (6 - shape.Height) / 2;

            for (int i = 0; i < shape.Height; i++)
            {
                for (int j = 0; j < shape.Width; j++)
                {
                    br2 = new SolidBrush(colorsArr[(int)shape.dots[i, j]]);
                    nextShapeGraphics.FillRectangle(br2, (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                    nextShapeGraphics.DrawRectangle((shape.dots[i, j] != ID.empty ? darkPen : Pens.Transparent), (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                    

                }
            }
            pictureBox2.Size = nextShapeBitmap.Size;
            pictureBox2.Image = nextShapeBitmap;

            return shape;
        } //IS FOR THE NEXT SHAPE MINI-BOX, ok!
        public void clearFilledRowsAndUpdateScore()
        {
            for (int i = 0; i < canvasHeight; i++)
            {
                int j;
                for (j = canvasWidth - 1; j >= 0; j--)
                {
                    if (canvasDotArray[j, i] == ID.empty)
                        break;
                }

                if (j == -1)
                {
                    //update score and level values
                    score++;
                    label1.Text = "Score: " + score;
                    label2.Text = "Level: " + score / 10;
                    //increase the speed
                    timer1.Interval -= 10;
                    //update dot array
                    for (j = 0; j < canvasWidth; j++)
                    {
                        for (int k = i; k > 0; k--)
                        {
                            canvasDotArray[j, k] = canvasDotArray[j, k - 1];
                        }

                        canvasDotArray[j, 0] = ID.empty;
                    }
                }

            }

            //draw panel based on the updated array vals
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    SolidBrush br = new SolidBrush((canvasDotArray[i, j] != ID.empty ? colorsArr[(int)canvasDotArray[i, j]] : Color.PaleTurquoise)); //OPTION 2- WITHOUT IMAGE BACKGROUND
                    canvasGraphics = Graphics.FromImage(canvasBitmap);
                    canvasGraphics.FillRectangle(br, i * dotSize, j * dotSize, dotSize, dotSize);
                    if (canvasDotArray[i, j] != ID.empty) canvasGraphics.DrawRectangle(darkPen, i * dotSize, j * dotSize, dotSize, dotSize);
                   
                }
            }

            pictureBox1.Image = canvasBitmap;


        } //IS FOR CLEARING FILLED ROWS

        //----------------------Only Shape Handling----------------------
        private Shape getRandomShapeWithCenterAligned()
        {
            Shape shape = ShapesHandler.GetRandomShape();

            //Calculate the x,y vals as if the shape lies in the center
            currentX = 7;
            currentY = -shape.Height;

            return shape;
        }

        private bool moveShape(int moveDown = 0, int moveSide = 0)
        {
            int newX = currentX + moveSide;
            int newY = currentY + moveDown;

            //return if shape reaches bottom/side bar
            if (newX < 0 || newX + currentShape.Width > canvasWidth
                || newY + currentShape.Height > canvasHeight)
            {
                return false;
            }

            //return if shape touches any other shapes
            {
                for (int i = 0; i < currentShape.Width; i++)
                {
                    for (int j = 0; j < currentShape.Height; j++)
                    {
                        if (newY + j > 0 && canvasDotArray[newX + i, newY + j] != ID.empty
                            && currentShape.dots[j, i] != ID.empty)
                        {
                            return false;
                        }
                    }
                }

                currentX = newX;
                currentY = newY;

                drawShape();

                return true;
            }
        }


        private void updateCanvasDotArrayWithCurrentShape()
        {
            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.dots[j, i] != ID.empty)
                    {
                        checkIfGameOver();
                        canvasDotArray[currentX + i, currentY + j] = currentShape.ShapeID;
                    }
                }
            }
        }


        //----------------------Gameplay Related----------------------
        private void checkIfGameOver()
        {
            if (currentY < 0)
            {
                timer1.Stop();
                MessageBox.Show("Game Over");
                currentY = 0;
                this.Close();
                Thread.Sleep(100);
                Application.Restart();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool isMovingSuccessful = moveShape(moveDown: 1);

            //if shape reaches bottom/touches other shapes
            if (!isMovingSuccessful)
            {
                //copy working image into canvas image
                canvasBitmap = new Bitmap(workingBitmap);

                updateCanvasDotArrayWithCurrentShape();

                //get next shape
                currentShape = nextShape;
                nextShape = getNextShape();

                clearFilledRowsAndUpdateScore();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int verticalMove = 0;
            int horizontalMove = 0;

            //calculate vertical and horizontal move vals based on key pressed
            switch (e.KeyCode)
            {
                //move shape left, right and down(faster)
                case Keys.Left:
                    verticalMove--;
                    break;
                case Keys.Right:
                    verticalMove++;
                    break;
                case Keys.Down:
                    horizontalMove++;
                    break;
                //rotate shape CKW
                case Keys.Space:
                    currentShape.turn();
                    break;
                default:
                    return;
            }

            bool isMoveSuccessful = moveShape(horizontalMove, verticalMove);
            //if rotate is not possible- rollback the shape
            if (!isMoveSuccessful && e.KeyCode == Keys.Up)
            {
                currentShape.rollback();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SAVE_button_MouseClick(object sender, MouseEventArgs e)
        {
            timer1.Stop();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();// + "..\\myModels";
            saveFileDialog1.Filter = "tetris files (*.trs)|*.trs|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, nextShape);
                    formatter.Serialize(stream, canvasDotArray);
                    formatter.Serialize(stream, currentX);
                    formatter.Serialize(stream, currentY);
                    formatter.Serialize(stream, score);
                    
                }
            }
            timer1.Start();
        }

        private void LOAD_button_MouseClick(object sender, MouseEventArgs e)
        {
            timer1.Stop();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();// + "..\\myModels";
            openFileDialog1.Filter = "tetris files (*.trs)|*.trs" +
                "|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream stream = File.Open(openFileDialog1.FileName, FileMode.Open);
                var binaryFormatter = new BinaryFormatter();
                nextShape = (Shape)binaryFormatter.Deserialize(stream);
                canvasDotArray = (ID[,])binaryFormatter.Deserialize(stream);
                currentX = (int)binaryFormatter.Deserialize(stream);
                currentY = (int)binaryFormatter.Deserialize(stream);
                score = (int)binaryFormatter.Deserialize(stream);
                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                label1.Invalidate();
                label2.Invalidate();
                clearFilledRowsAndUpdateScore();
                label1.Text = "Score: " + score.ToString();
                label2.Text = "Level: " + (score / 10).ToString();
                
            }
            timer1.Start();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void playMusic()
        {
           SoundPlayer tetrisMusic = new SoundPlayer(Properties.Resources.music);
           //tetrisMusic.Play();
        }

        
    }
}
