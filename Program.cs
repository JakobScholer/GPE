using Microsoft.Extensions.Configuration;

namespace GPE
{
    class Program
    {
        private readonly Digikey.ApiCaller _digikey = new();
        private readonly Farnell.ApiCaller _farnell = new();
        
        private async Task GetProductDetailsFromAllSuppliers()
        {
            Console.WriteLine("Enter product number:");
            string productNumber = Console.ReadLine() ?? string.Empty;

            await _digikey.GetProductDetailsAsync(productNumber);
            await _farnell.GetProductDetailsAsync(productNumber);
        }

        public static async Task Main()
        {
            Program program = new();
            await program.GetProductDetailsFromAllSuppliers();
        }
    }
}
