using System.Collections;

public interface IGameManager
{
	ManagerStatus status {get;}

    IEnumerator Startup();
}

public enum ManagerStatus
{
	Shutdown,
	Initializing,
	Started
}
