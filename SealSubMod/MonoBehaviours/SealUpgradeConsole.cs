namespace SealSubMod.MonoBehaviours;

// This class is based on the base-game UpgradeConsole class
// The seal should have TWO of these!
internal class SealUpgradeConsole : HandTarget, IHandTarget
{
    public Equipment modules { get; private set; }

    [SerializeField] private GameObject modulesRoot;

    [SerializeField] private GameObject[] moduleModels;

    public override void Awake()
    {
        base.Awake();
        if (modules == null)
        {
            InitializeModules();
        }
    }

    private void InitializeModules()
    {
        modules = new Equipment(gameObject, modulesRoot.transform);
        modules.SetLabel("CyclopsUpgradesStorageLabel");
        UpdateVisuals();
        modules.onEquip += OnEquip;
        modules.onUnequip += OnUnequip;
        string[] slots = new string[]
        {
            "SealModule1",
            "SealModule2",
            "SealModule3",
            "SealModule4"
        };
        modules.AddSlots(slots);
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        UpdateVisuals();
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < moduleModels.Length; i++)
        {
            SetModuleVisibility($"Module{i}", moduleModels[i]);
        }
    }

    private void SetModuleVisibility(string slot, GameObject module)
    {
        module?.SetActive(modules.GetTechTypeInSlot(slot) > TechType.None);
    }

    public void OnHandClick(GUIHand hand)
    {
        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(modules, false);
        pda.Open(PDATab.Inventory, null, null);
    }

    public void OnHandHover(GUIHand hand)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UpgradeConsole", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }
}
