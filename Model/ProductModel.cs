public class ProductModel
{
    private string name;
    private int quantity;
    private string description;
    private int price;
    private string currency;

    private string imageURL;

    public string GetName() => name;
    public int GetQuantity() => quantity;
    public string GetDescription() => description;
    public int GetPrice() => price;
    public string GetCurrency() => currency;

    public string GetImageURL() => imageURL;

    public void SetName(string name) => this.name = name;
    public void SetQuantity(int quantity) => this.quantity = quantity;
    public void SetDescription(string description) => this.description = description;
    public void SetPrice(int price) => this.price = price;
    public void SetCurrency(string currency) => this.currency = currency;
    public void SetImageURL(string imageURL) => this.imageURL = imageURL;

    public override string ToString()
    {
        return $"ImageURL: {GetImageURL()}, Name: {GetName()}, Quantity: {GetQuantity()}, Description: {GetDescription()}, Price: {GetPrice()} {GetCurrency()}";
    }
}