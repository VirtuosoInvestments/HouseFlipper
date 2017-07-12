namespace HouseFlipper.BusinessObjects
{
    public class PropertySearchOptions
    {
        public bool Active { get; set; }        
        public bool Sold { get; set; }
        public double WithinRadius { get; set; }
    }
}
