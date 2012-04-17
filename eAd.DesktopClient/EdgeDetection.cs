using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DesktopClient
{
    class EdgeDetection
    {
        public static Edges DetectEdgesCollision(Rectangle a, Rectangle b)
        {
            var result = Edges.None;

            if (a == b) return Edges.Identical;
            b.Intersect(a);
            if (b.IsEmpty) return Edges.None;
            if (a == b) return Edges.Covers;


            if (a.Top == b.Top && (a.Right >= b.Right && a.Left <= b.Left))
                result |= Edges.Top;
            if (a.Bottom == b.Bottom && (a.Right >= b.Right && a.Left <= b.Left))
                result |= Edges.Bottom;
            if (a.Left == b.Left && (a.Bottom >= b.Bottom && a.Top <= b.Top))
                result |= Edges.Left;
            if (a.Right == b.Right && (a.Bottom >= b.Bottom && a.Top <= b.Top))
                result |= Edges.Right;


            return result == Edges.None ? Edges.Inside : result;
        }
        [TestMethod]
        public void RectDoesNotIntersect()
        {
            var a = new Rectangle(0, 0, 10, 10);
            var b = new Rectangle(20, 20, 10, 10);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.None, result);
        }


        [TestMethod]
        public void RectAreNested()
        {
            var a = new Rectangle(0, 0, 30, 30);
            var b = new Rectangle(10, 10, 10, 10);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.Inside, result);
        }
        [TestMethod]
        public void RectCollidesOnTopAndLeft()
        {
            var a = new Rectangle(10, 10, 10, 10);
            var b = new Rectangle(0, 0, 10, 10);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.Left | Edges.Top, result);
        }
        [TestMethod]
        public void RectCollidesOnBottom()
        {
            var a = new Rectangle(0, 0, 20, 20);
            var b = new Rectangle(10, 10, 5, 50);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.Bottom, result);
        }

        [TestMethod]
        public void RectAreIdenticals()
        {
            var a = new Rectangle(10, 10, 10, 10);
            var b = new Rectangle(10, 10, 10, 10);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.Identical, result);
        }
        [TestMethod]
        public void RectBCoversA()
        {
            var a = new Rectangle(10, 10, 10, 10);
            var b = new Rectangle(0, 0, 30, 30);

            var result = DetectEdgesCollision(a, b);

            Assert.AreEqual<Edges>(Edges.Covers, result);
        }     
    }
}
