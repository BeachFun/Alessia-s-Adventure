using UnityEngine;
using System.Collections;

/// <summary>
/// Автономный компонент, который ставит игру на паузу
/// PROG MISTERIO | 23:23 12.08.2023
/// </summary>
public class Pauser : MonoBehaviour
{
	private bool paused = false;

	public bool Paused
	{
		get => paused;
		set => paused = value;
	}

	void FixedUpdate()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			paused = !paused;
		}

		if (paused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}
