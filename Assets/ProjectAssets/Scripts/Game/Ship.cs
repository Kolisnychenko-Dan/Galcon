using Fusion;

namespace Game
{
	public class Ship : NetworkBehaviour
	{
		[Networked] public int OwnerId { get; private set; }
	}
}