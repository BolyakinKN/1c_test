using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour 
{
	[field: SerializeField]
	public TextMeshProUGUI HealthText { get; private set; }
	[field: SerializeField]
	public TextMeshProUGUI ResultText { get; private set; }
	[field: SerializeField]
	public Button RestartButton { get; private set; }
	[field: SerializeField]
	public CanvasGroup PanelEndGame { get; private set; }
}
