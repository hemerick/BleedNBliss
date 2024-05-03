using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] SimpleButton butt1;
    [SerializeField] SimpleButton butt2;
    [SerializeField] SimpleButton butt3;

    [SerializeField] Image im1;
    [SerializeField] Image im2;
    [SerializeField] Image im3;

    private void Start()
    {
        butt1.OnClick += ChangeColorToRed;
        butt2.OnClick += ChangeColorToGreen;
        butt3.OnClick += () => im3.color = Color.blue; // Altérnative ENCORE PLUS COURTE
    }

    public void ChangeColorToRed()
    {
        im1.color = Color.red;
    }

    public void ChangeColorToGreen() => im2.color = Color.green; //Altérnative courte

    
}
