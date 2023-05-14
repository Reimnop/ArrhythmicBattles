using Microsoft.Extensions.DependencyInjection;

namespace FlexFramework;

public interface IStartup
{
    void ConfigureServices(IServiceCollection serviceCollection);
}