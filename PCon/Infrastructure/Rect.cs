namespace PCon.Infrastructure
{
    public struct Rect
    {
        private bool Equals(Rect other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals(object obj)
        {
            return obj is Rect other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;
                return hashCode;
            }
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        private int Bottom { get; set; }

        public int Height => Bottom - Top;
        public int Width => Right - Left;

        public static bool operator !=(Rect r1, Rect r2)
        {
            return !(r1 == r2);
        }

        public static bool operator ==(Rect r1, Rect r2)
        {
            return r1.Left == r2.Left && r1.Right == r2.Right && r1.Top == r2.Top && r1.Bottom == r2.Bottom;
        }
    }
}