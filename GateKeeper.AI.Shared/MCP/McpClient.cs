using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateKeeper.AI.Shared.MCP;

public interface IMcpClientHost
{
    Task<IMcpClient> Create();
    Task<List<McpClientTool>> GetTools(bool print = false);
}
public class McpClientHost(string gitHubToken) : IMcpClientHost
{
    private IMcpClient _mcpClient;

    public async Task<IMcpClient> Create()
    {
        _mcpClient = await McpClientFactory.CreateAsync(new SseClientTransport(new()
        {
            Endpoint = new Uri("https://api.githubcopilot.com/mcp/"),
            AdditionalHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {gitHubToken ?? string.Empty}" }
                    },

        }));
        return _mcpClient;
    }

    public async Task<List<McpClientTool>> GetTools(bool print = false)
    {
        if (_mcpClient == null)
        {
            throw new InvalidOperationException("MCP client is not initialized. Call CreateMcpClient() first.");
        }
        var tools = await _mcpClient.ListToolsAsync().ConfigureAwait(false);
        Console.WriteLine($"\nAvailable GitHub MCP tools ({tools.Count}):");
        if (print)
        {
            foreach (var tool in tools)
            {
                Console.WriteLine($"  • {tool.Name}: {tool.Description}");
            }
        }
        //Where(x => x.Name.Contains("pull_request")).
        return tools.ToList();
    }
}
