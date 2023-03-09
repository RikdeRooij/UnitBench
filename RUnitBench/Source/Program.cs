using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace NUnitBench
{
    public static class Program
    {
        //public readonly Shape[] shapes = new Shape[3]
        //                                 {
        //                                     new Rectangle(3.0f, 4.0f),
        //                                     new Triangle(5.0f, 2.0f),
        //                                     new Circle(3.3f)
        //                                 };
        //public readonly ShapeProperties[] shapeProps = new ShapeProperties[]
        //                                               {
        //                                                   new ShapeProperties(ShapeTypes.Rectangle, 3.0f, 4.0f),
        //                                                   new ShapeProperties(ShapeTypes.Triangle, 5.0f, 2.0f),
        //                                                   new ShapeProperties(ShapeTypes.Circle, 3.3f)
        //                                               };

        static void Main()
        {
        }
    }

    // --------------------------------

    // --------------------------------

    public enum ShapeTypes { Rectangle, Triangle, Circle }

    public readonly struct ShapeProperties
    {
        private readonly ShapeTypes type;
        private readonly float l;
        private readonly float h;
        public ShapeProperties(ShapeTypes type, float l, float h = 0)
        {
            this.type = type;
            this.l = l;
            this.h = h;
        }
        [System.Diagnostics.Contracts.Pure]
        public static float GetArea(ShapeProperties props)
        {
            return props.type switch
            {
                ShapeTypes.Rectangle => (props.l * props.h),
                ShapeTypes.Triangle => (props.l * props.h / 2.0f),
                ShapeTypes.Circle => (props.l * props.l * (float)Math.PI),
                _ => throw new InvalidOperationException("Unkown area for type: " + props.type)
            };
        }
    }

    [System.Diagnostics.Contracts.Pure]
    public class ShapePropertiesClass
    {
        private readonly ShapeTypes type;
        private readonly float l;
        private readonly float h;
        public ShapePropertiesClass(ShapeTypes type, float l, float h = 0)
        {
            this.type = type;
            this.l = l;
            this.h = h;
        }
        [System.Diagnostics.Contracts.Pure]
        public float GetArea()
        {
            return type switch
            {
                ShapeTypes.Rectangle => (l * h),
                ShapeTypes.Triangle => (l * h / 2.0f),
                ShapeTypes.Circle => (l * l * (float)Math.PI),
                _ => throw new InvalidOperationException("Unkown area for type: " + type)
            };
        }
        [System.Diagnostics.Contracts.Pure]
        public static float GetArea(ShapePropertiesClass props)
        {
            return props.type switch
            {
                ShapeTypes.Rectangle => (props.l * props.h),
                ShapeTypes.Triangle => (props.l * props.h / 2.0f),
                ShapeTypes.Circle => (props.l * props.l * (float)Math.PI),
                _ => throw new InvalidOperationException("Unkown area for type: " + props.type)
            };
        }
    }


    // --------------------------------
    // http://www.excoded.com/learn/csharp/cs_polymorphism.php

    public abstract class Shape
    {
        public abstract float GetArea();
    }

    public class Rectangle : Shape
    {
        private readonly float length;
        private readonly float width;

        public Rectangle(float length, float width)
        {
            this.length = length;
            this.width = width;
        }

        public override float GetArea()
        {
            return (length * width);
        }
    }

    public class Triangle : Shape
    {
        private readonly float baseline;
        private readonly float height;

        public Triangle(float baseline, float height)
        {
            this.baseline = baseline;
            this.height = height;
        }

        public override float GetArea()
        {
            return (baseline * height / 2.0f);
        }
    }

    public class Circle : Shape
    {
        private readonly float radius;

        public Circle(float radius)
        {
            this.radius = radius;
        }

        public override float GetArea()
        {
            return (radius * radius * (float)Math.PI);
        }
    }

}
