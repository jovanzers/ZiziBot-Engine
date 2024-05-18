namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("AppSettings")]
public class AppSettingsEntity : EntityBase
{
    public string Root { get; set; }
    public string Name { get; set; }
    public string Field { get; set; }
    public string DefaultValue { get; set; }
    public string InitialValue { get; set; }
    public string Value { get; set; }
}