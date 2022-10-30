using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesField : MonoBehaviour
{
    public GameObject EnemyContainerPrefab;
    public GameObject AddEnemyButton;

    public readonly List<EnemyField> EnemyFields = new List<EnemyField>();
    public int enemiesNum = 0;

    private readonly (float, float)[][] positions = new (float, float)[][]
    {
        new (float, float)[2] { (-120f, -30f), (120f, -30f) },
        new (float, float)[3] { (-140f, -70f), (0, -30f), (140f, -70f) },
        new (float, float)[4] { (-150f, -110f), (-55f, -30f), (55f, -30f), (150f, -110f) },
        new (float, float)[5] { (-160f, -130f), (-80f, -80f), (0f, -30f), (80f, -80f), (160f, -130f) }
    };

    private Vector2 smallSize = new Vector2(80f, 33f);

    public void AddEnemy()
    {
        bool small = false;
        GameObject enemy;
        if (enemiesNum == 0)
        {
            enemy = Instantiate(EnemyContainerPrefab, transform);
            AddEnemyButton.transform.localPosition -= new Vector3(0, 130f);
        }
        else
        {
            small = true;
            EnemyContainerPrefab.GetComponent<RectTransform>().sizeDelta = smallSize;
            (float x, float y)[] coords = positions[enemiesNum - 1];
            for (int i = 0; i < EnemyFields.Count; i++)
            {
                EnemyFields[i].small = small;
                EnemyFields[i].transform.localPosition = new Vector3(coords[i].x, coords[i].y, 0);
            }

            enemy = Instantiate(EnemyContainerPrefab, transform);
            enemy.transform.localPosition = new Vector3(coords[enemiesNum].x, coords[enemiesNum].y, 0);
        }

        enemiesNum++;
        EnemyField field = enemy.GetComponent<EnemyField>();
        field.id = enemiesNum;
        field.small = small;
        field.profile.SetName(enemiesNum.ToString());
        EnemyFields.Add(field);

        if (enemiesNum == 5)
        {
            AddEnemyButton.SetActive(false);
        }

        Game.AddEnemy(field.GetPlayer("AI", "Easy"));
    }
}
