using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image _image;
    [SerializeField] private float _timeDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private float _Target = 1f;
    private Coroutine drainHealthBar;
    private Color healthBarColor;

    // Start is called before the first frame update
    void Start()
    {
        //get iamge component
        _image = GetComponent<Image>();

        //initially setting the image color to the health gradient to 1 which is green.
        _image.color = _healthBarGradient.Evaluate(_Target);

        //initalize healthbar color
        CheckHealthBarGardientAmount();
    }

    void Update()
    {
        //health bar follows camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);            
    }

    public void SetHealthBar(float maxHealth, float currentHealth)
    {
        _Target = currentHealth/maxHealth;
        drainHealthBar = StartCoroutine(DrainHealthBar());
        CheckHealthBarGardientAmount();

    }

    //animating healthbar progress as it decreases
    private IEnumerator DrainHealthBar()
    {
        float elapsedTime = 0f;
        float fillAmount = _image.fillAmount;
        Color currentColor = _image.color;

        while (elapsedTime < _timeDrain)
        {
            elapsedTime += Time.deltaTime;

            //lerp the fill amount
            _image.fillAmount = Mathf.Lerp(fillAmount, _Target, (elapsedTime/_timeDrain));

            //lerp the color based on gradient
            _image.color = Color.Lerp(currentColor, healthBarColor, (elapsedTime/_timeDrain));
        }
        yield return null;
    }

    private void CheckHealthBarGardientAmount()
    {
        healthBarColor = _healthBarGradient.Evaluate(_Target);
    }
}
