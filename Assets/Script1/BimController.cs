using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
/**BimController BIM模型互動
 * 簡述：負責BIM模型移動功能的鎖定與開啟、BIM模型顯示狀態的切換
 * 使用方式：
 * 腳本放入GameEvents底下
 * bim_mesh_render_state_須配合BIM模型一開始的顯示狀態設置true or false
 * bim_gameobject_需要放入BIM模型物件
 */
public class BimController : MonoBehaviour
{
    [SerializeField]
    public GameObject bim_gameobject_;
    [HideInInspector]
    private bool bim_mesh_render_state_;
    private void Start()
    {
        GameEvents.current.actionSwitchBimMoveAxisConstraint += SwitchBimMoveAxisConstraint;
        GameEvents.current.actionSwitchBimMeshRender += SwitchBimMeshRender;
        this.bim_mesh_render_state_ = true;
    }
    private void SwitchBimMoveAxisConstraint()
    {
        if (this.bim_gameobject_ == null) return;
        AxisFlags axisFlags = this.bim_gameobject_.GetComponent<MoveAxisConstraint>().ConstraintOnMovement;
        int constraint_state = (int)axisFlags;
        if (constraint_state == 0)
        {
            this.bim_gameobject_.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)(-1);//Constrint EveryThing
            this.bim_gameobject_.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)(-1);
        }
        else
        {
            this.bim_gameobject_.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)(0);//Constraint Nothing
            this.bim_gameobject_.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)(0);
        }
        return;
    }
    private void SwitchBimMeshRender()
    {
        if (this.bim_mesh_render_state_)
        {
            for(int i = 0; i < this.bim_gameobject_.transform.childCount;i++)
            {
                GameObject child = this.bim_gameobject_.transform.GetChild(i).gameObject;
                if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().enabled=false;
            }
            this.bim_mesh_render_state_ = false;
        }
        else
        {
            for (int i = 0; i < this.bim_gameobject_.transform.childCount; i++)
            {
                GameObject child = this.bim_gameobject_.transform.GetChild(i).gameObject;
                if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().enabled = true;
            }
            this.bim_mesh_render_state_ = true;
        }
    }
}
