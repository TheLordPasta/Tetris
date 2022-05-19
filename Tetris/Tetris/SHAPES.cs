using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SHAPES
{
    public enum ID {empty, Basic,O, I, T, L, J, Z, S }
    [Serializable]
    public class Shape 
    {
        //The Shape class allows to create/store shapes using 2D arrays
        protected ID shapeID;
        protected Color[] colorsArr = new Color[9]
           {
                Color.Empty,Color.Gray,
                Color.FromArgb(249, 240, 170),
                Color.FromArgb(150, 200, 228),
                Color.FromArgb(230, 180, 232),
                Color.FromArgb(244, 205, 140),
                Color.FromArgb(130, 218, 204),
                Color.FromArgb(234, 150, 183),
                Color.FromArgb(205, 241, 140)
           };
        private Color shapeColor;
        private bool circleFilled;
        private bool triangleFilled;
        private int width;
        private int height;
        public ID[,] dots;
        private ID[,] backupDots;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool CircleFilled
        {
            get { return circleFilled; }
            set { circleFilled = value; }
        }
        public bool TriangleFilled
        {
            get { return triangleFilled; }
            set { triangleFilled = value; }
        }
        public ID ShapeID
        {
            get { return shapeID; }
            set { shapeID = value; }
        }

        public Color ShapeColor
        {
            get { return shapeColor; }
            set { shapeColor = value; }
        }
        public ID[,] Dots
        {
            get { return dots; }
            //set { shapeColor = value; }
        }

        

        public Shape()
        {
            shapeID = ID.Basic;
            shapeColor = colorsArr[(int)shapeID];
            CircleFilled = false;
            TriangleFilled = false;
        }
        public void turn()
        {
            //back the dots' values into backup dots for rolling back if needed
            backupDots = dots;
            dots = new ID[width, height];

            for (int i=0;i<width;i++)
            {
                for (int j=0;j<height;j++)
                {
                    dots[i, j] = backupDots[height - 1 - j, i];
                }
            }

            int temp = width;
            width = height;
            height = temp;
        }

        public void rollback()
        {
            dots = backupDots;

            int temp = width;
            width = height;
            height = temp;
        }


        ~Shape() { }
    }
    [Serializable]
    public class OShape : Shape
    {
        public OShape()
        {
            Width = 2;
            Height = 2;
           
            ShapeID = ID.O;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
                {ID.O,ID.O },
                {ID.O,ID.O }
            };
            
        }
        ~OShape() { }
    }
    [Serializable]
    public class DiagonalShape:OShape //Inheritance
    {
        public DiagonalShape()
        {
            dots = new ID[,]
            {
                {ID.O,ID.empty },
                {ID.empty,ID.O}
            };
            CircleFilled = true;


        }
        //public void drawPolygonShape(Graphics g, Point[] p)
        //{
        //    Color c1 = Color.BlanchedAlmond;
        //    SolidBrush br;

        //    for (int i = 0; i < Width; i++)
        //    {
        //        for (int j = 0; j < Height; j++)
        //        {
        //            br = new SolidBrush(c1);
        //            if (dots[j, i] != ID.empty)
        //            {
        //                g.FillPolygon(br, p);
        //            }
        //        }
        //    }
        //}
        ~DiagonalShape() { }
    }
    [Serializable]
    public class LikeShape: OShape //Inheritance
    {
        public LikeShape()
        {
            Width = 3;
            Height = 3;
            dots = new ID[,]
            {
              {ID.empty,ID.empty, ID.O },
              {ID.O, ID.O,  ID.O },
               {ID.O, ID.O,  ID.O },

            };
            TriangleFilled = true;
        }
        //public void drawEllipseShape(Graphics g, float x, float y)
        //{
        //    Color c1 = Color.Black;
        //    SolidBrush br;

        //    for (int i = 0; i < Width; i++)
        //    {
        //        for (int j = 0; j < Height; j++)
        //        {
        //            br = new SolidBrush(c1);
        //            if (dots[j, i] != ID.empty)
        //            {
        //                g.FillEllipse(br, x, y, 10, 10);
        //            }
        //        }
        //    }
        //}
        ~LikeShape() { }
    }
    [Serializable]
    public class IShape : Shape
    {
        public IShape()
        {
            Width = 1;
            Height = 4;
            ShapeID = ID.I;
            ShapeColor = colorsArr[(int)shapeID];

            dots = new ID[,]
            {
               { ID.I },
               { ID.I },
               { ID.I },
               { ID.I}
            };
        }
        ~IShape() { }
    }
    [Serializable]
    public class TShape : Shape
    {
        public TShape()
        {
            Width = 3;
            Height = 2;
            ShapeID = ID.T;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
               {ID.empty, ID.T, ID.empty},
               { ID.T   , ID.T,    ID.T }
            };
        }
        ~TShape() { }
    }
    [Serializable]
    public class LShape : Shape
    {
        public LShape()
        {
            Width = 3;
            Height = 2;
            ShapeID = ID.L;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
               {ID.empty,ID.empty, ID.L },
               { ID.L   , ID.L   , ID.L }
            };
        }
        ~LShape() { }
    }
    [Serializable]
    public class JShape : Shape
    {
        public JShape()
        {
            Width = 3;
            Height = 2;
            ShapeID = ID.J;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
               {ID.J,  ID.empty,  ID.empty },
               {ID.J,    ID.J,        ID.J }
            };
        }
        ~JShape() { }
    }
    [Serializable]
    public class ZShape : Shape
    {
        public ZShape()
        {
            Width = 3;
            Height = 2;
            ShapeID = ID.Z;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
               {ID.Z,ID.Z,ID.empty },
               {ID.empty,ID.Z,ID.Z }
            };
        }
        ~ZShape() { }
    }
    [Serializable]
    public class SShape : Shape
    {
        public SShape()
        {
            Width = 3;
            Height = 2;
            ShapeID = ID.S;
            ShapeColor = colorsArr[(int)shapeID];
            dots = new ID[,]
            {
               {ID.empty,ID.S,ID.S },
               {ID.S,ID.S,ID.empty }
            };
        }
        ~SShape() { }
    }

    //New shapes can be added here..
   
}

