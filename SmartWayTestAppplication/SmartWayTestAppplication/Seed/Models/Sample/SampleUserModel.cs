namespace SmartWayTestAppplication.Seed.Models.Sample;

public class SampleUserModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public Guid OrganizationId { get; set; }

    public List<string> Roles { get; set; } = null!;
}