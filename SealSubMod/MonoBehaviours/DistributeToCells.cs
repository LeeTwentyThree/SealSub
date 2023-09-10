namespace SealSubMod.MonoBehaviours;

internal class PowerSourcePrioritizer : MonoBehaviour
{
    [SerializeField] PowerRelay powerRelay;
    [SerializeField] BatterySource[] targetPowerSources;

    public void Update()
    {
        var powerCells = GetPowerCells();
        var everythingElse = powerRelay.inboundPowerSources.Where(source => !powerCells.Contains(source));


        if (powerCells.Count() <= 0) return;

        foreach (var source in everythingElse)
        {
            foreach (var cell in powerCells)
            {
                var missing = cell.GetMaxPower() - cell.GetPower();
                var filled = source.ConsumeEnergy(missing, out var amount);
                cell.AddEnergy(amount, out _);

                //if it couldn't fill the power cell
                //the source is out of energy
                //so move onto next one
                if (!filled) break;
            }
        }
    }
    private IEnumerable<IPowerInterface> GetPowerCells()
    {
        IEnumerable<IPowerInterface> sources;

        if (targetPowerSources.Length > 0) sources = targetPowerSources;
        else sources = powerRelay.inboundPowerSources.Where(source => source is BatterySource);

        sources = sources.Where(source => source.GetPower() < source.GetMaxPower());
        return sources;
    }
}
