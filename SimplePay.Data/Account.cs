namespace SimplePay.Data;

public record Account(User User, List<Transfer> Transfers, decimal Balance, List<Shop.Shop> Shops);