namespace WorldStressMapTool
{
    public class NamedCoordinate : BaseCoordinate
    {
        public NamedCoordinate(string name, double latitude, double longitude) : base(latitude, longitude)
        {
            Name = name;
            ProjectedX = 0.0;
            ProjectedY = 0.0;
        }
        public string Name { get; internal set; }
    }


}