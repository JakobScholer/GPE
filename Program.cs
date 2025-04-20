namespace GPE
{
    class Program
    {
        private readonly Digikey.ApiCaller digikey;
        Program() {
            digikey = new();
        }
        private async Task GetProductDetailsFromAllSuppliers()
        {
            Console.WriteLine("Enter product number:");
            string productNumber = Console.ReadLine() ?? string.Empty;

            await digikey.CallProductDetailsAsync(productNumber);
        }

        public static async Task Main(string[] args)
        {
            Program program = new();
            await program.GetProductDetailsFromAllSuppliers();
        }
    }
}
