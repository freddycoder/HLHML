namespace HLHML.AnalyseurLexical
{
    public interface ILexer
    {
        public int Position { get; }

        public char CurrentChar { get; set; }

        public char PeekChar { get; }

        DernierTerme DernierTerme { get; }

        Terme ObtenirProchainTerme();

        public void Incrementer();
    }
}
