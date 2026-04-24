namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

public class UserAddressModel
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public UserGeolocationModel Geolocation { get; set; } = new();
}

public class UserGeolocationModel
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
