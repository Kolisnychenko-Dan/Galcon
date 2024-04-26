using Fusion;
using Tools;

namespace Networking.Abstractions
{
	public interface ILobbyService
	{
		public NetworkDictionary<PlayerRef, int> PlayerRefToIdMap { get; }
	}
}