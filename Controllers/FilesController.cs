using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using webapi.builder;

[Route("products")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FilesController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string description, [FromForm] int quantity, [FromForm] int price, [FromForm] string name, [FromForm] string currency)
    {
        if (file == null || file.Length == 0) {
            return BadRequest("File is empty.");
        }

        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
        string imageURL = Path.Combine("wwwroot","images", "products");
        var uploadsFolder = Path.Combine(assemblyDirectory, "../../../", imageURL);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        ProductModel product = new ProductModelBuilder()
            .SetName(name)
            .SetDescription(description)
            .SetPrice(price)
            .SetCurrency(currency)
            .SetQuantity(quantity)
            .SetImageURL(Path.Combine(imageURL, uniqueFileName))
            .Build();
        
        MySqlDataAccess db = new MySqlDataAccess();
        db.CreateProduct(product);

        // You can return the file path or any other relevant information
        return Ok(new { FilePath = filePath });
    }

    [HttpGet("allProducts")]
    public async Task<IActionResult> GetAllProductsInfo() 
    {
        MySqlDataAccess db = new MySqlDataAccess();
        IList<ProductModel> products = db.GetAllProducts();
        foreach (var p in products) {
            Console.WriteLine(p);
        }
        return Ok(products);
    }
}
