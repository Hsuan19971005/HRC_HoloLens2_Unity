using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**RobotStation
 * ²�z�G�����H�Q�����I����Ʈ榡
 */
public class RobotStation : MonoBehaviour
{
    public Vector3 position = new Vector3();//�����H��y�y��
    public Quaternion quaternion = new Quaternion();//�����H��y�|����
    public List<ScheduledTrajectory> StationTrajectories = new List<ScheduledTrajectory>();//�����H�Q�����I�����ȲM��
    public string station_button_name;//UI������Button ID�W��
}
