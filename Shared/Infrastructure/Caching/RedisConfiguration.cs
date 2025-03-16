namespace API.Shared.Infrastructure.Caching;

public class RedisConfiguration
{
  public string Configuration { get; set; } = "localhost:6379";
  public string InstanceName { get; set; } = "SchnellKuendigen:";
}