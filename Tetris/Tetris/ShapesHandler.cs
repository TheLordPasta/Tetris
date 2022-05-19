using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHAPES;

namespace Tetris
{
    [Serializable]
    public class ShapesHandler 
    {
        //This Class will load the shapes array in its constructor 
        //and have a method to randomly get a shape from the array

        private static Shape[] shapesArray;

        static ShapesHandler()
        {
            shapesArray = new Shape[]
            {
                new OShape(), new DiagonalShape(), 
                new LikeShape(), new IShape(),
                new IShape(), new TShape(),
                new LShape(), new JShape(),
                new ZShape(), new SShape()
            };
        }

        

        public static Shape GetRandomShape()
        {
            Shape shape = shapesArray[new Random().Next(shapesArray.Length)];
            return shape;
        }
        ~ShapesHandler() { }
    }
}
