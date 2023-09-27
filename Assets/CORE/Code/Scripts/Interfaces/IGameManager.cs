using System.Collections;

public interface IGameManager
{
	ManagerStatus Status {get;}

    IEnumerator Startup();
}

public enum ManagerStatus
{
	Shutdown,
	Initializing,
	Started
}
