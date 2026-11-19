namespace Couchy.Api.Entities;

public class HomeDetails
{
    public int? MaxGuests { get; set; }

    public bool? HasWifi { get; set; }
    public bool? HasKitchen { get; set; }
    public bool? HasWashingMachine { get; set; }
    public bool? HasHeating { get; set; }
    public bool? HasAirConditioning { get; set; }
    public bool? HasBathroom { get; set; }
    public bool? PetsAllowed { get; set; }
    public bool? SmokingAllowed { get; set; }

    public string? SleepingArrangement { get; set; }
    public string? HouseRules { get; set; }
}
