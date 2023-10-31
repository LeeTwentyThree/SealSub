namespace SealSubMod.MonoBehaviours;

internal class AlarmManager : MonoBehaviour
{
    private AnimateAlarm[] alarms;
    public bool AlarmsEnabled { get; private set; }

    private void Awake()
    {
        alarms = GetComponentsInChildren<AnimateAlarm>();
    }

    public void SetAlarmsEnabled(bool enabled)
    {
        alarms.ForEach((a) => a.SetAlarmEnabled(enabled));
        AlarmsEnabled = enabled;
    }
    public void ToggleAlarms() => SetAlarmsEnabled(!AlarmsEnabled);
}