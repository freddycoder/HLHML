namespace HLHML
{
    public class DernierTerme
    {
        public Terme? Terme { get; set; }
        public int Position { get; set; }

        public override string ToString()
        {
            return Terme?.ToString() ?? string.Empty;
        }
    }
}