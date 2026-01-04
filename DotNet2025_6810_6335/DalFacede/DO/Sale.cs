namespace DO;

public record Sale(
    // Sale: data record for sales
    int IDNumber,
    int ProductIDNumber,
    int QuantityItemsRequiredtoReceivetheSale,
    double FullPrice,
    bool SaleOnlyforClubCustomers,
    string SaleStartDate,
    string SaleEndDate
);