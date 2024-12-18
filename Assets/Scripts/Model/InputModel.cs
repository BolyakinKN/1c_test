using UniRx;
using UnityEngine;
using Zenject;

public class InputModel: ITickable
{
    private Vector2ReactiveProperty _inputValue = new ();

	public Vector2ReactiveProperty InputValue => _inputValue;

	public void Tick()
	{
		Vector2 input = new ();
		input.x = Input.GetAxis("Horizontal");
		input.y = Input.GetAxis("Vertical");
		_inputValue.Value = input;
	}
}
