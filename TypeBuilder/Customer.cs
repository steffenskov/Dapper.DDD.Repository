namespace TypeBuilderDemo;

public record Customer(Guid id, string Name, Address Address);

public record Address(string Street, string City);