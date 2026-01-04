namespace DO;

public record Product(
    int IDNumber,
    string ProductName,
    Categories Category,
    double Price,
    int QuantityInStock 
);

// Product: data record for products
