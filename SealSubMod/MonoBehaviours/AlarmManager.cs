namespace SealSubMod.MonoBehaviours;

internal class AlarmManager : MonoBehaviour
{
    private AnimateAlarm[] alarms;

    private void Awake()
    {
        alarms = GetComponentsInChildren<AnimateAlarm>();
    }

    public void SetAlarmsEnabled(bool enabled)
    {
        alarms.ForEach((a) => a.SetAlarmEnabled(enabled));
    }
}