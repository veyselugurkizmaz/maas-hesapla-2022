namespace WebAPI
{
    public static class Extensions
    {
        public static decimal Round(this decimal number, int decimals = 2)
        {
            return decimal.Round(number, decimals);
        }
    }
}
