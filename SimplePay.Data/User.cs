namespace SimplePay.Data;

public record User(string Login, string PasswordHash, Shop.Shop? Shop)
{
    public Account Account { get; init; }
}