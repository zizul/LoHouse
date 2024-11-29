using UnityEngine;

public class AlignChildrenOnX : MonoBehaviour
{
    [SerializeField] private float maxArcWidth = 5f; // Maksymalna szerokoœæ ³uku dla du¿ej liczby kart
    [SerializeField] private float minArcWidth = 2f; // Minimalna szerokoœæ ³uku dla ma³ej liczby kart
    [SerializeField] private float arcHeight = 2f; // Wysokoœæ ³uku (na osi Y)
    [SerializeField] private float maxTilt = 30f; // Maksymalne przechylenie na osi Z dla du¿ej liczby kart
    [SerializeField] private float minTilt = 5f; // Minimalne przechylenie na osi Z dla ma³ej liczby kart

    private int childCount = 0;

    private void Start()
    {
        childCount = transform.childCount;
        ArrangeInArc();
    }

    private void Update()
    {
        if (childCount != transform.childCount)
        {
            ArrangeInArc();
            Debug.Log("AlignChildrenOnX");
        }
    }

    private void ArrangeInArc()
    {
        childCount = transform.childCount;
        if (childCount == 0) return;

        // Jeœli tylko jedna karta, ustaw j¹ na œrodku
        if (childCount == 1)
        {
            Transform onlyChild = transform.GetChild(0);
            onlyChild.localPosition = new Vector3(0, arcHeight, 0);
            onlyChild.localRotation = Quaternion.identity;
            return;
        }

        // Dopasowujemy szerokoœæ ³uku: im mniej kart, tym mniejsza szerokoœæ (œciœlej)
        float adjustedArcWidth = Mathf.Lerp(minArcWidth, maxArcWidth, (float)(childCount - 1) / 10f);

        // Dopasowujemy maksymalny tilt (pochylenie Z): mniej kart = mniejsze przechylenie
        float adjustedTilt = Mathf.Lerp(minTilt, maxTilt, (float)(childCount - 1) / 10f);

        float centerIndex = (childCount - 1) / 2f; // Œrodek ³uku

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Pozycja na osi X, centrowana wzglêdem liczby kart
            float xPos = Mathf.Lerp(-adjustedArcWidth / 2f, adjustedArcWidth / 2f, (float)i / (childCount - 1));

            // Wysokoœæ na osi Y: parabola, œrodek najwy¿ej
            float normalizedPos = Mathf.Abs((i - centerIndex) / centerIndex); // Normalizacja
            float yPos = arcHeight * (1 - normalizedPos * normalizedPos); // Parabola: 1 - x^2

            // Pochylenie na osi Z
            float tiltAngle = Mathf.Lerp(-adjustedTilt, adjustedTilt, (float)i / (childCount - 1));

            // Ustawienie pozycji i rotacji
            child.localPosition = new Vector3(xPos, yPos, -i);
            child.localRotation = Quaternion.Euler(0, 0, tiltAngle);

            CardBehaviour cardBehaviour = child.gameObject.GetComponent<CardBehaviour>();
            if (cardBehaviour != null)
            {
                cardBehaviour.SetSpriteSortingOrder(i);
                cardBehaviour.currentRotation = Quaternion.Euler(0, 0, tiltAngle);
            }
        }
    }
}