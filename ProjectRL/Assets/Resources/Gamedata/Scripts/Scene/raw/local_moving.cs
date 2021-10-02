using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class local_moving : MonoBehaviour
{
    public Vector3 old_pos;
    public Vector3 new_pos;

      
    public void Start_move(Image image, RectTransform RT, Vector3 new_position)
    {

        StartCoroutine(Move_CG( image,  RT,  new_position));
    }
    private IEnumerator Move_CG( Image image, RectTransform RT, Vector3 new_position)
    {
        while (RT.localPosition.x != new_position.x)
        {
            RT.localPosition = Vector3.MoveTowards(RT.localPosition, new_position, Time.deltaTime * 1250f);
            yield return null;
        }
    }
    public void All_stop()
    {
     
        StopAllCoroutines();
    }
}
