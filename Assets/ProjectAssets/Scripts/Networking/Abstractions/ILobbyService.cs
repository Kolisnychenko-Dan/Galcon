using Fusion;
using Tools;

namespace Networking.Abstractions
{
	public interface ILobbyService
	{
		IReadOnlyBijectiveDictionary<PlayerRef, int> PlayerRefToIdMap { get; }
	}
}