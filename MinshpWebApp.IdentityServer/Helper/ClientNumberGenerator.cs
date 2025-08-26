namespace MinshpWebApp.IdentityServer.Helper
{
    public class ClientNumberGenerator
    {
        private int _lastNumber;
        private readonly int _digits;

        public ClientNumberGenerator(int start = 0, int digits = 6)
        {
            _lastNumber = start;
            _digits = digits;
        }

        public string GenerateNext()
        {
            _lastNumber++;
            return $"CL{_lastNumber.ToString().PadLeft(_digits, '0')}";
        }
    }
}
