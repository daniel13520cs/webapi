using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using webapi.builder;
using System.Net.Http;
using PuppeteerSharp;
using AngleSharp.Html.Parser;
[Route("products")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpClientFactory _httpClientFactory;

    public FilesController(IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
    {
        _webHostEnvironment = webHostEnvironment;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string description, [FromForm] int quantity, [FromForm] int price, [FromForm] string name, [FromForm] string currency)
    {
        if (file == null || file.Length == 0) {
            return BadRequest("File is empty.");
        }

        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
        string imageURL = Path.Combine("images", "products");
        var uploadsFolder = Path.Combine(assemblyDirectory, "../../../", "wwwroot", imageURL);
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

    [HttpGet("url")]
    public async Task<IActionResult> GetProductInfo(string productUrl)
    {
        try
        {
            /*var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(productUrl);

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(response);

            // Extract product information from the HTML document
            //var name = document.QuerySelector(".product-name-selector").TextContent;
            //var imageUrl = document.QuerySelector(".product-image-selector").GetAttribute("src");

            ProductModel product = new ProductModelBuilder()
                //.SetName(name)
                //.SetImageURL(imageUrl)
                .Build();*/
            var headlessProduct = GetProductFromHeadlessBrowswer(productUrl);
            return Ok(new {headlessProduct});
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    private async Task<string> GetProductFromHeadlessBrowswer(string productUrl)
    {
        using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, ExecutablePath="C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe", }))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(productUrl);

            // Wait for some time to ensure the page is fully loaded
            await page.WaitForTimeoutAsync(1000);

            // Extract product information using PuppeteerSharp
            //var productName = await page.QuerySelectorAsync(".product-title");
            //var imageUrl = await page.QuerySelectorAsync(".product-image img");

            /*var product = new Product
            {
                Name = productName == null ? "N/A" : await productName.InnerTextAsync(),
                ImageUrl = imageUrl == null ? "N/A" : await imageUrl.GetAttributeAsync("src"),
                // Add other relevant properties
            };*/
            var pageContent = await page.GetContentAsync();

            return pageContent;
        }
    }
}
