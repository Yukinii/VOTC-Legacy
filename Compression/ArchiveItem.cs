namespace Compression
{
    internal class ArchiveItem
    {
        public readonly long Position;
        public readonly int Size;
        public readonly string Name;
        internal ArchiveItem(long offset, int size, string name)
        {
            Position = offset;
            Size = size;
            Name = name;
        }
    }
}