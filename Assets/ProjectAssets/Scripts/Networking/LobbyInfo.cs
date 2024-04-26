using Fusion;

namespace Networking
{
	public struct LobbyInfo : INetworkStruct
	{
		public int PlayerCount { get; set; }
	}
}