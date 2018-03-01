namespace Project.Model
{
    public class Grid
    {
        public Grid(long row, long col)
        {
            Columns = col;
            Rows = row;
        }
        public long Columns { get; set; }
        public long Rows { get; set; }
    }
}
