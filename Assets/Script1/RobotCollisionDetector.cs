using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**RobotCollisionDetector 機器人碰撞偵測器
 * 簡述：用以偵測機器人是否與其他物件碰撞，碰撞時改變其顏色為Red，非碰撞時恢復原顏色
 * 使用方式：
 * 將腳本放置在Robot Visual中附有Collider的物件下，Collider啟動IsTrigger，並將該物件放置在ArmNoCollision的Layer中，其餘機器人Collision物件則另行放置於其他Layer。
 * 在Project Setting中對碰撞關係進行設定，ArmNoCollision layer彼此不碰撞，ArmNoCollision與其餘機器人Collision物件所在Layer也不碰撞。
 * 目的說明：
 * 機器人分為Visual與Collision兩種Collider，兩者範圍重疊，必須讓兩者互不碰撞才能順利執行機器人碰撞顯色功能
 */
public class RobotCollisionDetector : MonoBehaviour
{
    private Color origin_color;
    void Start()
    {
        this.origin_color = this.GetComponent<Renderer>().material.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<Renderer>().material.color = Color.red;
    }
    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<Renderer>().material.color = this.origin_color;
    }
}
