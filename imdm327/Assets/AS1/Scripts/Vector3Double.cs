using System;

public struct Vector3Double
{
    public double x, y, z;

    public Vector3Double(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3Double operator +(Vector3Double a, Vector3Double b)
    {
        return new Vector3Double(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3Double operator -(Vector3Double a, Vector3Double b)
    {
        return new Vector3Double(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3Double operator *(Vector3Double a, double d)
    {
        return new Vector3Double(a.x * d, a.y * d, a.z * d);
    }

    public static Vector3Double operator /(Vector3Double a, double d)
    {
        return new Vector3Double(a.x / d, a.y / d, a.z / d);
    }

    public double magnitude
    {
        get
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
    }

    public Vector3Double normalized
    {
        get
        {
            double mag = magnitude;
            if (mag > 1E-5)
                return this / mag;
            else
                return new Vector3Double(0, 0, 0);
        }
    }

    public static Vector3Double zero
    {
        get
        {
            return new Vector3Double(0, 0, 0);
        }
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}
