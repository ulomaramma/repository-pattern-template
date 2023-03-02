using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace FoodRecipeApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MyAction()
        {
            string signature = Request.Headers["Xcoins-Signature"];
            string nonce = Request.Headers["Nonce"];

            // Read request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string requestBody = await reader.ReadToEndAsync();

                // Concatenate request body with nonce
                string message = requestBody + nonce;

                // Perform signature verification
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String("publicKeyInBase64"), out _);
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    byte[] signatureBytes = Convert.FromBase64String(signature);

                    bool isVerified;
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        isVerified = rsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                    }

                    // Output verification result
                    Console.WriteLine("Verified: " + isVerified);

                    // Handle verification result
                    if (isVerified)
                    {
                        // Update database ...
                        Console.WriteLine("Success: true");
                        return Ok(new { success = true });
                    }
                    else
                    {
                        return BadRequest("Verification failed");
                    }
                }
            }
        }
    }
}
