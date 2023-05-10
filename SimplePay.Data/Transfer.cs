namespace SimplePay.Data;

public record Transfer(
    decimal Value,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    Account To,
    Account From
);