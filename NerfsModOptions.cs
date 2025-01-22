using BepInEx.Logging;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Nerfs;

public class NerfsModOptions : OptionInterface
{
    private readonly ManualLogSource Logger;

    public NerfsModOptions(Nerfs modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
        //PlayerSpeed = this.config.Bind<float>("PlayerSpeed", 1f, new ConfigAcceptableRange<float>(0f, 100f));
        GeneralArtificerSpearAngles = this.config.Bind<bool>("GeneralArtificerSpearAngles", false);

        GourmandSpeedModifier = this.config.Bind<float>("GourmandSpeedModifier", 1.0f, new ConfigAcceptableRange<float>(0f, 5f));
        GourmandEmptySpeedModifier = this.config.Bind<float>("GourmandEmptySpeedModifier", 1.1f, new ConfigAcceptableRange<float>(0f, 5f));
        GourmandCraftingNerfs = this.config.Bind<bool>("GourmandCraftingNerfs", true);
        GourmandSlideModifier = this.config.Bind<float>("GourmandSlideModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        GourmandSpearDamageModifier = this.config.Bind<float>("GourmandSpearDamageModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        GourmandExhaustionModifier = this.config.Bind<float>("GourmandExhaustionModifier", 1f, new ConfigAcceptableRange<float>(0f, 5f));
        GourmandEmptyExhaustionModifier = this.config.Bind<float>("GourmandEmptyExhaustionModifier", 1.1f, new ConfigAcceptableRange<float>(0f, 5f));

        ArtificerSpeedModifier = this.config.Bind<float>("ArtificerSpeedModifier", 0.9f, new ConfigAcceptableRange<float>(0f, 5f));
        ArtificerThrowingAbility = this.config.Bind<int>("ArtificerThrowingAbility", 2, new ConfigAcceptableRange<int>(0, 2));
        ArtificerScavPopulationModifier = this.config.Bind<float>("ArtificerScavPopulationModifier", 0.5f, new ConfigAcceptableRange<float>(0f, 5f));
        ArtificerCraftingCost = this.config.Bind<int>("ArtificerCraftingCost", 2, new ConfigAcceptableRange<int>(1, 9));
        ArtificerJumpModifier = this.config.Bind<float>("ArtificerJumpModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        ArtificerJumpFoodLoss = this.config.Bind<bool>("ArtificerJumpFoodCost", true);
        ArtificerJumpFoodCost = this.config.Bind<int>("ArtificerJumpFoodPips", 1, new ConfigAcceptableRange<int>(0, 20));
        ArtificerJumpsPerCost = this.config.Bind<int>("ArtificerJumpsPerCost", 1, new ConfigAcceptableRange<int>(0, 20));
        ArtificerJumpRequiresFood = this.config.Bind<bool>("ArtificerJumpRequiresFood", false);
        ArtificerExtraParryCost = this.config.Bind<int>("ArtificerExtraParryCost", 1, new ConfigAcceptableRange<int>(0, 20));
        ArtificerExplosionStunPyroCost = this.config.Bind<float>("ArtificerExplosionStunPyroCost", 0.4f, new ConfigAcceptableRange<float>(0f, 5f));
        ArtificerScavengerMeat = this.config.Bind<int>("ArtificerScavengerMeat", 2, new ConfigAcceptableRange<int>(0, 9));
        ArtificerThrowAngle = this.config.Bind<int>("ArtificerThrowAngle", 25, new ConfigAcceptableRange<int>(0, 90));
        ArtificerThrowAngleModifier = this.config.Bind<float>("ArtificerThrowAngleModifier", 0.6f, new ConfigAcceptableRange<float>(0f, 2f));
        ArtificerBothEndings = this.config.Bind<bool>("ArtificerBothEndings", true);

        RivuletSpeedModifier = this.config.Bind<float>("RivuletSpeedModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        RivuletThrowingAbility = this.config.Bind<int>("RivuletThrowingAbility", 0, new ConfigAcceptableRange<int>(0, 2));
        RivuletSlideModifier = this.config.Bind<float>("RivuletSlideModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        RivuletJumpModifier = this.config.Bind<float>("RivuletJumpModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        RivuletCellSlowsCycles = this.config.Bind<bool>("RivuletCellSlowsCycles", false);
        RivuletCellRainIntensity = this.config.Bind<float>("RivuletCellRainIntensity", 0.6f, new ConfigAcceptableRange<float>(0f, 5f));
        RivuletCellCycleModifier = this.config.Bind<float>("RivuletCellCycleModifier", 0.6f, new ConfigAcceptableRange<float>(0f, 5f));
        RivuletCellPrecycleChance = this.config.Bind<float>("RivuletCellPrecycleChance", 0.6f, new ConfigAcceptableRange<float>(0f, 1f));
        RivuletRotGrabModifier = this.config.Bind<float>("RivuletRotGrabModifier", 0.5f, new ConfigAcceptableRange<float>(0f, 5f));

        SpearmasterSpeedModifier = this.config.Bind<float>("SpearmasterSpeedModifier", 0.9f, new ConfigAcceptableRange<float>(0f, 5f));
        SpearmasterThrowingAbility = this.config.Bind<int>("SpearmasterThrowingAbility", 2, new ConfigAcceptableRange<int>(0, 2));
        SpearmasterSpearsUntilCost = this.config.Bind<int>("SpearmasterSpearsUntilCost", 2, new ConfigAcceptableRange<int>(0, 4));
        SpearmasterPipsPerSpearCost = this.config.Bind<int>("SpearmasterPipsPerSpearCost", 1, new ConfigAcceptableRange<int>(0, 20));
        SpearmasterGrowSpearExhaustion = this.config.Bind<float>("SpearmasterGrowSpearExhaustion", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        SpearmasterThrowSpearExhaustion = this.config.Bind<float>("SpearmasterThrowSpearExhaustion", 0.4f, new ConfigAcceptableRange<float>(0f, 5f));
        SpearmasterBiteDeathChanceModifier = this.config.Bind<float>("SpearmasterBiteDeathChanceModifier", 1.5f, new ConfigAcceptableRange<float>(0f, 10f));
        SpearmasterJumpModifier = this.config.Bind<float>("SpearmasterJumpModifier", 1.1f, new ConfigAcceptableRange<float>(0f, 5f));
        SpearmasterGrowSpeedModifier = this.config.Bind<float>("SpearmasterGrowSpeedModifier", 1.0f, new ConfigAcceptableRange<float>(0f, 5f));
        SpearmasterNeedleDamageModifier = this.config.Bind<float>("SpearmasterNeedleDamageModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));

        SaintSpeedModifier = this.config.Bind<float>("SaintSpeedModifier", 1.0f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintTongueModifier = this.config.Bind<float>("SaintTongueModifier", 0.5f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintTongueLengthModifier = this.config.Bind<float>("SaintTongueLengthModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintSwingSpeedModifier = this.config.Bind<float>("SaintSwingSpeedModifier", 1.5f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintTongueSwingModifier = this.config.Bind<float>("SaintTongueSwingModifier", 0.6f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintImproveTongueAim = this.config.Bind<bool>("SaintImproveTongueAim", true);
        SaintSwallowedWarmthModifier = this.config.Bind<float>("SaintSwallowedWarmthModifier", 0.6f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintLanternWarmthModifier = this.config.Bind<float>("SaintLanternWarmthModifier", 0.9f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintObjectWarmthModifier = this.config.Bind<float>("SaintObjectWarmthModifier", 1.0f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintGodTimeModifier = this.config.Bind<float>("SaintGodTimeModifier", 0.8f, new ConfigAcceptableRange<float>(0f, 5f));
        SaintRechargeRateModifier = this.config.Bind<float>("SaintRechargeRateModifier", 1f, new ConfigAcceptableRange<float>(0f, 5f));

        ArenaArtificerSpearSpeedModifier = this.config.Bind<float>("ArenaArtificerSpearSpeedModifier", 0.7f, new ConfigAcceptableRange<float>(0f, 5f));
        ArenaArtificerSpearSpeedEverywhere = this.config.Bind<bool>("ArenaArtificerSpearSpeedEverywhere", false);


        //sync mod options
        RainMeadowCompat.EasyConfigSync.RegisterConfigs(
            GeneralArtificerSpearAngles,

            GourmandSpeedModifier, GourmandEmptySpeedModifier, GourmandCraftingNerfs,
            GourmandSlideModifier, GourmandSpearDamageModifier, GourmandExhaustionModifier,
            GourmandEmptyExhaustionModifier,

            ArtificerSpeedModifier, ArtificerThrowingAbility, ArtificerScavPopulationModifier,
            ArtificerCraftingCost, ArtificerJumpModifier, ArtificerJumpFoodLoss,
            ArtificerJumpFoodCost, ArtificerJumpsPerCost, ArtificerJumpRequiresFood,
            ArtificerExtraParryCost, ArtificerExplosionStunPyroCost, ArtificerScavengerMeat,
            ArtificerThrowAngle, ArtificerThrowAngleModifier, ArtificerBothEndings,

            RivuletSpeedModifier, RivuletThrowingAbility, RivuletSlideModifier,
            RivuletJumpModifier, RivuletCellSlowsCycles, RivuletCellRainIntensity,
            RivuletCellCycleModifier, RivuletCellPrecycleChance, RivuletRotGrabModifier,

            SpearmasterSpeedModifier, SpearmasterThrowingAbility, SpearmasterSpearsUntilCost,
            SpearmasterPipsPerSpearCost, SpearmasterGrowSpearExhaustion, SpearmasterThrowSpearExhaustion,
            SpearmasterBiteDeathChanceModifier, SpearmasterJumpModifier, SpearmasterGrowSpeedModifier,
            SpearmasterNeedleDamageModifier,

            SaintSpeedModifier, SaintTongueModifier, SaintTongueLengthModifier,
            SaintSwingSpeedModifier, SaintTongueSwingModifier, SaintImproveTongueAim,
            SaintSwallowedWarmthModifier, SaintLanternWarmthModifier, SaintObjectWarmthModifier,
            SaintGodTimeModifier, SaintRechargeRateModifier,

            ArenaArtificerSpearSpeedModifier, ArenaArtificerSpearSpeedEverywhere
        );

    }

    //public readonly Configurable<float> PlayerSpeed;
    public Configurable<bool> GeneralArtificerSpearAngles;

    public Configurable<float> GourmandSpeedModifier;
    public Configurable<float> GourmandEmptySpeedModifier;
    public Configurable<bool> GourmandCraftingNerfs;
    public Configurable<float> GourmandSlideModifier;
    public Configurable<float> GourmandSpearDamageModifier;
    public Configurable<float> GourmandExhaustionModifier;
    public Configurable<float> GourmandEmptyExhaustionModifier;

    public Configurable<float> ArtificerSpeedModifier;
    public Configurable<int> ArtificerThrowingAbility;
    public Configurable<float> ArtificerScavPopulationModifier;
    public Configurable<int> ArtificerCraftingCost;
    public Configurable<float> ArtificerJumpModifier;
    public Configurable<bool> ArtificerJumpFoodLoss;
    public Configurable<int> ArtificerJumpFoodCost;
    public Configurable<int> ArtificerJumpsPerCost;
    public Configurable<bool> ArtificerJumpRequiresFood;
    public Configurable<int> ArtificerExtraParryCost;
    public Configurable<float> ArtificerExplosionStunPyroCost;
    public Configurable<int> ArtificerScavengerMeat;
    public Configurable<int> ArtificerThrowAngle;
    public Configurable<float> ArtificerThrowAngleModifier;
    public Configurable<bool> ArtificerBothEndings;
    public Configurable<float> ArenaArtificerSpearSpeedModifier;
    public Configurable<bool> ArenaArtificerSpearSpeedEverywhere;

    public Configurable<float> RivuletSpeedModifier;
    public Configurable<int> RivuletThrowingAbility;
    public Configurable<float> RivuletSlideModifier;
    public Configurable<float> RivuletJumpModifier;
    public Configurable<bool> RivuletCellSlowsCycles;
    public Configurable<float> RivuletCellRainIntensity;
    public Configurable<float> RivuletCellCycleModifier;
    public Configurable<float> RivuletCellPrecycleChance;
    public Configurable<float> RivuletRotGrabModifier;

    public Configurable<float> SpearmasterSpeedModifier;
    public Configurable<int> SpearmasterThrowingAbility;
    public Configurable<int> SpearmasterSpearsUntilCost;
    public Configurable<int> SpearmasterPipsPerSpearCost;
    public Configurable<float> SpearmasterGrowSpearExhaustion;
    public Configurable<float> SpearmasterThrowSpearExhaustion;
    public Configurable<float> SpearmasterBiteDeathChanceModifier;
    public Configurable<float> SpearmasterJumpModifier;
    public Configurable<float> SpearmasterGrowSpeedModifier;
    public Configurable<float> SpearmasterNeedleDamageModifier;

    public Configurable<float> SaintSpeedModifier;
    public Configurable<float> SaintTongueModifier;
    public Configurable<float> SaintTongueLengthModifier;
    public Configurable<float> SaintSwingSpeedModifier;
    public Configurable<float> SaintTongueSwingModifier;
    public Configurable<bool> SaintImproveTongueAim;
    public Configurable<float> SaintSwallowedWarmthModifier;
    public Configurable<float> SaintLanternWarmthModifier;
    public Configurable<float> SaintObjectWarmthModifier;
    public Configurable<float> SaintGodTimeModifier;
    public Configurable<float> SaintRechargeRateModifier;

    private UIelement[] UIArrGeneralOptions;
    private UIelement[] UIArrPresets;
    private UIelement[] UIArrGourmand;
    private UIelement[] UIArrArtificer;
    private UIelement[] UIArrRivulet;
    private UIelement[] UIArrSpearmaster;
    private UIelement[] UIArrSaint;


    public override void Initialize()
    {
        var presetsTab = new OpTab(this, "Presets");
        var gourmandTab = new OpTab(this, "Gourmand");
        var artificerTab = new OpTab(this, "Artificer");
        var rivuletTab = new OpTab(this, "Rivulet");
        var spearmasterTab = new OpTab(this, "Spearmaster");
        var saintTab = new OpTab(this, "Saint");
        this.Tabs = new[]
        {
            presetsTab,
            gourmandTab,
            artificerTab,
            rivuletTab,
            spearmasterTab,
            saintTab
        };

        //General Options
        float h = 550f, g = -30f, w = 10f, d = 100f;
        UIArrGeneralOptions = new UIelement[]
        {
            //General Options
            new OpLabel(w, h, "General Options", true),
            new OpCheckBox(GeneralArtificerSpearAngles, w, h+=g){description="Allows all slugcats to throw spears at angles like Artificer can in this mod (configurable in Artificer's settings)."}, new OpLabel(d, h, "Angled Spear Throws For Everyone"),
            //Presets
            new OpLabel(w, h+=g*2, "Presets", true)
        };

        //Preset buttons
        OpSimpleButton disableButton = new(new Vector2(150f, h+=g*2), new Vector2(150f, 40f), "Disable All Nerfs") { description = "Disables all nerfs in this mod. You can then manually re-enable the ones you want." };
        disableButton.OnClick += DisableButtonMethod;

        OpSimpleButton resetButton = new(new Vector2(150f, h+=g*2), new Vector2(150f, 40f), "Reset to Default") { description = "Resets all settings to the default value I set them to have initially. Enables most settings." };
        resetButton.OnClick += ResetButtonMethod;

        OpSimpleButton extremeButton = new(new Vector2(150f, h+=g*2), new Vector2(150f, 40f), "Extreme Settings") { description = "The perfect option for all your Downpour haters. This will be miserable! Maybe fun as a challenge run?" };
        extremeButton.OnClick += ExtremeButtonMethod;

        OpSimpleButton overpoweredButton = new(new Vector2(150f, h += g * 2), new Vector2(150f, 40f), "Overpowered Mode") { description = "Tired of dying? Want to make the game absurd? Okay. Your choice." };
        overpoweredButton.OnClick += OverpoweredButtonMethod;

        UIArrPresets = new UIelement[]
        {
            //Presets
            //new OpLabel(w, h+=g, "Presets", true),
            disableButton,
            resetButton,
            extremeButton,
            overpoweredButton
        };

        presetsTab.AddItems(UIArrGeneralOptions);
        presetsTab.AddItems(UIArrPresets);

        //Gourmand
        h = 550f; g = -30f; w = 10f; d = 100f;
        UIArrGourmand = new UIelement[]
        {
            new OpLabel(w, h, "Gourmand", true),
            new OpUpdown(GourmandSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Gourmand's movement speed stats (swimming unaffected)."}, new OpLabel(d, h, "Speed Modifier"),
            new OpUpdown(GourmandEmptySpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Further multiplies Gourmand's movement speed stats (swimming unaffected) when at 0 food pips."}, new OpLabel(d, h, "No Food Speed Modifier"),
            new OpCheckBox(GourmandCraftingNerfs, w, h+=g){description="Disables the crafting of over-powered items like firebug eggs and singularity bombs. Also disables crafting live creatures."}, new OpLabel(d, h, "Crafting Nerfs"),
            new OpUpdown(GourmandSlideModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Gourmand's speed whilst sliding."}, new OpLabel(d, h, "Slide Speed Modifier"),
            new OpUpdown(GourmandSpearDamageModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Gourmand's spear damage when not exhausted."}, new OpLabel(d, h, "Spear Damage Modifier"),
            new OpUpdown(GourmandExhaustionModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the general rate at which Gourmand becomes exhausted. Higher rates = faster exhaustion. Does not affect recovering from exhaustion."}, new OpLabel(d, h, "Exhaustion Rate Modifier"),
            new OpUpdown(GourmandEmptyExhaustionModifier, new Vector2(w, h+=g), 80f, 1){description="Further multiplies the rate at which Gourmand becomes exhausted when at 0 food pips. Higher rates = faster exhaustion. Does not affect recovering from exhaustion."}, new OpLabel(d, h, "No Food Exhaustion Modifier")
        };
        gourmandTab.AddItems(UIArrGourmand);

        //Artificer
        h = 550f; g = -30f; w = 10f; d = 100f;
        UIArrArtificer = new UIelement[]
        {
            new OpLabel(w, h, "Artificer", true),
            new OpUpdown(ArtificerSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Artificer's movement speed stats (swimming unaffected)."}, new OpLabel(d, h, "Speed Modifier"),
            new OpUpdown(true, ArtificerThrowingAbility, new Vector2(w, h+=g), 80f){description="Sets Artificer's spear damage/speed: 0 = Monk, 1 = Survivor, 2 = Hunter."}, new OpLabel(d, h, "Throwing Ability"),
            new OpUpdown(ArtificerScavPopulationModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the number of Scavengers that spawn during Artificer's campaign."}, new OpLabel(d, h, "Scavenger Population Modifier"),
            new OpUpdown(true, ArtificerCraftingCost, new Vector2(w, h+=g), 80f){description="Food pips consumed to convert items into explosives."}, new OpLabel(d, h, "Explosives Crafting Cost"),
            new OpUpdown(ArtificerJumpModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Artificer's explosion jump velocity."}, new OpLabel(d, h, "Explosion Jump Modifier"),
            //new OpCheckBox(ArtificerJumpFoodLoss, w, h+=g){description="Removes a quarter pip of food when performing a parry or an explosion jump."}, new OpLabel(d, h, "Explosion/Parry Food Loss"),
            new OpUpdown(true, ArtificerJumpFoodCost, new Vector2(w, h+=g), 80f){description="The number of quarter pips of food to remove when performing a parry or an explosion jump."}, new OpLabel(d, h, "/"), new OpUpdown(true, ArtificerJumpsPerCost, new Vector2(w+d, h), 80f){description="The number of explosion jumps/parries that can be performed before any food is taken away."}, new OpLabel(d*2, h, "Quarter Pips Per Explosion Jump/Parry"),
            new OpCheckBox(ArtificerJumpRequiresFood, w, h+=g){description="Requires Artificer to have enough food to perform an explosion jump."}, new OpLabel(d, h, "Explosion Jump Requires Food"),
            new OpUpdown(true, ArtificerExtraParryCost, new Vector2(w, h+=g), 80f){description="Increases the counter more for parries (metric = explosion jumps). A value of 5 will make Artificer briefly stunned, whilst 9 makes her instantly die."}, new OpLabel(d, h, "Extra Parry Cost/Cooldown"),
            new OpUpdown(ArtificerExplosionStunPyroCost, new Vector2(w, h+=g), 80f, 1){description="How much should be added to the explosion jump counter per second of stun by external explosion. In short, causes Artificer to overheat when stunned by explosions."}, new OpLabel(d, h, "Add. Jump Counter Per Stun Second"),
            new OpUpdown(true, ArtificerScavengerMeat, new Vector2(w, h+=g), 80f){description="Sets the amount of food pips granted for Artificer by eating scavengers (4 in base game) (only applies in Artificer's campaign)."}, new OpLabel(d, h, "Scavenger Food Amount"),
            new OpUpdown(true, ArtificerThrowAngle, new Vector2(w, h+=g), 80f){description="Max angle in degrees that Artificer can angle spear (or any other \"weapon\") throws (controller recommended)."}, new OpLabel(d, h, "Spear Throw Max Angle"),
            new OpUpdown(ArtificerThrowAngleModifier, new Vector2(w, h+=g), 80f, 1){description="0.6 Recommended. Skews the analogue stick's angle when throwing spears. Lower values tend towards more horizontal throws. 1 = no skew, 0 = always horizontal."}, new OpLabel(d, h, "Spear Throw Angle Modifier"),
            new OpCheckBox(ArtificerBothEndings, w, h+=g){description="Allows Artificer to revisit echoes after her Metropolis ending, and makes echoes grant karma increases."}, new OpLabel(d, h, "Compatible Endings"),
            //Arena
            new OpLabel(w, h+=g, "Arena Options", true), new OpLabel(d*2, h, "(Only applies in arena mode, not story campaigns)"),
            new OpUpdown(ArenaArtificerSpearSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the speed of Artificer's spear throws in arena mode."}, new OpLabel(d, h, "Spear Speed Modifier"),
                new OpCheckBox(ArenaArtificerSpearSpeedEverywhere, d*3, h){description="Causes the left option to apply in all gameplay (including story campaigns), not just arena mode."}, new OpLabel(d*4, h, "Spear Speed Everywhere")
        };
        artificerTab.AddItems(UIArrArtificer);

        //Rivulet
        h = 550f; g = -30f; w = 10f; d = 100f;
        UIArrRivulet = new UIelement[]
        {
            new OpLabel(w, h, "Rivulet", true),
            new OpUpdown(RivuletSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Rivulet's movement speed stats (swimming unaffected)."}, new OpLabel(d, h, "Speed Modifier"),
            new OpUpdown(true, RivuletThrowingAbility, new Vector2(w, h+=g), 80f){description="Sets Rivulet's spear damage/speed: 0 = Monk, 1 = Survivor, 2 = Hunter."}, new OpLabel(d, h, "Throwing Ability"),
            new OpUpdown(RivuletSlideModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Rivulet's speed whilst sliding."}, new OpLabel(d, h, "Slide Speed Modifier"),
            new OpUpdown(RivuletJumpModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Rivulet's jump height and length."}, new OpLabel(d, h, "Jump Modifier"),
            new OpCheckBox(RivuletCellSlowsCycles, w, h+=g){description="Whether cycles are longer after taking the Rarefaction Cell (true in default game)."}, new OpLabel(d, h, "Cell Extends Cycles"),
            new OpUpdown(RivuletCellRainIntensity, new Vector2(w, h+=g), 80f, 1){description="Multiplies the intensity of precycle rain after taking the Rarefaction Cell."}, new OpLabel(d, h, "Rain Intensity After Cell"),
            new OpUpdown(RivuletCellCycleModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the length of cycles during which precycles occur after taking the Rarefaction Cell."}, new OpLabel(d, h, "Cycle Modifier After Cell (Precycles only)"),
            new OpUpdown(RivuletCellPrecycleChance, new Vector2(w, h+=g), 80f, 1){description="Adds an additional chance for cycles to be precycles after taking the Rarefaction Cell."}, new OpLabel(d, h, "Additional Precycle Chance After Cell"),
            new OpUpdown(RivuletRotGrabModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the strength with which rot walls pull in and consume Rivulet."}, new OpLabel(d, h, "Rot Pull Strength Modifier")
        };
        rivuletTab.AddItems(UIArrRivulet);

        //Spearmaster
        h = 550f; g = -30f; w = 10f; d = 100f;
        UIArrSpearmaster = new UIelement[]
        {
            new OpLabel(w, h, "Spearmaster", true),
            new OpUpdown(SpearmasterSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Spearmaster's movement speed stats (swimming unaffected)."}, new OpLabel(d, h, "Speed Modifier"),
            new OpUpdown(true, SpearmasterThrowingAbility, new Vector2(w, h+=g), 80f){description="Sets Spearmaster's spear damage/speed: 0 = Monk, 1 = Survivor, 2 = Hunter."}, new OpLabel(d, h, "Throwing Ability"),
            //new OpUpdown(true, SpearmasterSpearsUntilCost, new Vector2(w, h+=g), 80f){description="Takes a quarter pip of food for every X spears grown. Disabled if set to 0."}, new OpLabel(d, h, "Spears per Quarter Pip"),
            new OpUpdown(true, SpearmasterPipsPerSpearCost, new Vector2(w, h+=g), 80f){description="The number of quarter pips to take each time food is subtracted for growing spears."}, new OpLabel(d, h, "/"), new OpUpdown(true, SpearmasterSpearsUntilCost, new Vector2(w+d, h), 80f){description="How many spears can be grown before food is taken. 0 = disabled, 1 = every spear, 2 = every other spear, etc."}, new OpLabel(d*2, h, "Quarter Pips Per Spear"),
            new OpUpdown(SpearmasterGrowSpearExhaustion, new Vector2(w, h+=g), 80f, 1){description="Makes Spearmaster exhausted when growing a spear with no food. 0 = no exhaustion, 1 = severe exhaustion."}, new OpLabel(d, h, "Foodless Grown Spear Exhaustion"),
            new OpUpdown(SpearmasterThrowSpearExhaustion, new Vector2(w, h+=g), 80f, 1){description="Allows Spearmaster to become exhausted (to a limited degree) by throwing many spears. 0 = no exhaustion, 1 = severe exhaustion."}, new OpLabel(d, h, "Max Thrown Spear Exhaustion"),
            new OpUpdown(SpearmasterBiteDeathChanceModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the chance of Spearmaster being killed instantly by lizard bites."}, new OpLabel(d, h, "Bite Death Chance Modifier"),
            new OpUpdown(SpearmasterJumpModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Spearmaster's jump height."}, new OpLabel(d, h, "Jump Modifier"),
            new OpUpdown(SpearmasterGrowSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the rate at which Spearmaster produces spears."}, new OpLabel(d, h, "Spear Growth Speed Modifier"),
            new OpUpdown(SpearmasterNeedleDamageModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the damage dealt by spears (needles) produced by Spearmaster. Does not affect normal spears."}, new OpLabel(d, h, "Needle Damage Modifier")
        };
        spearmasterTab.AddItems(UIArrSpearmaster);

        //Saint
        h = 550f; g = -30f; w = 10f; d = 100f;
        UIArrSaint = new UIelement[]
        {
            new OpLabel(w, h, "Saint", true),
            new OpUpdown(SaintSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies Saint's movement speed stats (swimming unaffected)."}, new OpLabel(d, h, "Speed Modifier"),
            new OpUpdown(SaintTongueModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the launch velocity gained by releasing Saint's tongue."}, new OpLabel(d, h, "Tongue Launch Modifier"),
            new OpUpdown(SaintSwingSpeedModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the speed at which Saint swings with his tongue."}, new OpLabel(d, h, "Tongue Swing Speed Modifier"),
            new OpUpdown(SaintTongueSwingModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the x distance Saint can swing from his tongue when hanging mostly stationary."}, new OpLabel(d, h, "Tongue Swing Distance Modifier"),
            new OpUpdown(SaintTongueLengthModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the total length of Saint's tongue."}, new OpLabel(d, h, "Tongue Length Modifier"),
            new OpCheckBox(SaintImproveTongueAim, w, h+=g){description="Alters Saint's tongue to factor in the modified tongue length AND allows Saint's tongue to be shot in more than 8 directions using the analogue stick."}, new OpLabel(d, h, "Improve Tongue Aim"),
            new OpUpdown(SaintGodTimeModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the max charge of Saint's \"god mode\" at karma cap 10+ (does not apply in Rubicon)."}, new OpLabel(d, h, "\"God Timer\" Length Modifier"),
            new OpUpdown(SaintRechargeRateModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the rate at which Saint's \"god mode\" charges up."}, new OpLabel(d, h, "\"God Timer\" Recharge Modifier"),
            new OpUpdown(SaintSwallowedWarmthModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the warmth provided by swallowed lanterns (technically applies to all slugcats). Does not affect held lanterns."}, new OpLabel(d, h, "Swallowed Warmth Modifier"),
            new OpUpdown(SaintLanternWarmthModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the warmth provided by all external heat sources, including held lanterns. Does not affect swallowed lanterns."}, new OpLabel(d, h, "Object Warmth Modifier"),
            new OpUpdown(SaintObjectWarmthModifier, new Vector2(w, h+=g), 80f, 1){description="Multiplies the speed at which slugcats' temperature changes. Affects both heat sources and hypothermia."}, new OpLabel(d, h, "Hypothermia/Heat Rate Modifier")
        };
        saintTab.AddItems(UIArrSaint);

    }

    private void DisableButtonMethod(UIfocusable trigger)
    {
        GeneralArtificerSpearAngles.Value = false;

        GourmandSpeedModifier.Value = 1f;
        GourmandEmptySpeedModifier.Value = 1f;
        GourmandCraftingNerfs.Value = false;
        GourmandSlideModifier.Value = 1f;
        GourmandSpearDamageModifier.Value = 1f;
        GourmandExhaustionModifier.Value = 1f;
        GourmandEmptyExhaustionModifier.Value = 1f;

        ArtificerSpeedModifier.Value = 1f;
        ArtificerThrowingAbility.Value = 2;
        ArtificerScavPopulationModifier.Value = 1f;
        ArtificerCraftingCost.Value = 1;
        ArtificerJumpModifier.Value = 1f;
        ArtificerJumpFoodLoss.Value = false;
        ArtificerJumpFoodCost.Value = 0;
        ArtificerJumpsPerCost.Value = 0;
        ArtificerJumpRequiresFood.Value = false;
        ArtificerExtraParryCost.Value = 0;
        ArtificerExplosionStunPyroCost.Value = 0f;
        ArtificerScavengerMeat.Value = 4;
        ArtificerThrowAngle.Value = 0;
        //ArtificerThrowAngleModifier;
        ArtificerBothEndings.Value = false;
        ArenaArtificerSpearSpeedModifier.Value = 1f;

        RivuletSpeedModifier.Value = 1f;
        RivuletThrowingAbility.Value = 1;
        RivuletSlideModifier.Value = 1f;
        RivuletJumpModifier.Value = 1f;
        RivuletCellSlowsCycles.Value = true;
        RivuletCellRainIntensity.Value = 1f;
        RivuletCellCycleModifier.Value = 1f;
        RivuletCellPrecycleChance.Value = 0f;
        RivuletRotGrabModifier.Value = 1f;

        SpearmasterSpeedModifier.Value = 1f;
        SpearmasterThrowingAbility.Value = 2;
        SpearmasterSpearsUntilCost.Value = 0;
        SpearmasterPipsPerSpearCost.Value = 0;
        SpearmasterGrowSpearExhaustion.Value = 0f;
        SpearmasterThrowSpearExhaustion.Value = 0f;
        SpearmasterBiteDeathChanceModifier.Value = 1f;
        SpearmasterJumpModifier.Value = 1f;
        SpearmasterGrowSpeedModifier.Value = 1f;
        SpearmasterNeedleDamageModifier.Value = 1f;

        SaintSpeedModifier.Value = 1f;
        SaintTongueModifier.Value = 1f;
        SaintTongueLengthModifier.Value = 1f;
        SaintSwingSpeedModifier.Value = 1f;
        SaintTongueSwingModifier.Value = 1f;
        SaintImproveTongueAim.Value = false;
        SaintSwallowedWarmthModifier.Value = 1f;
        SaintLanternWarmthModifier.Value = 1f;
        SaintObjectWarmthModifier.Value = 1f;
        SaintGodTimeModifier.Value = 1f;
        SaintRechargeRateModifier.Value = 1f;

        this.ShowConfigs();
    }

    private void ResetButtonMethod(UIfocusable trigger)
    {
        try
        {
            foreach (KeyValuePair<string, ConfigurableBase> pair in this.config.configurables)
                pair.Value.BoxedValue = pair.Value.defaultValue;

            this.ShowConfigs();
        }
        catch (System.Exception ex) { Logger.LogError(ex); }
    }

    private void ExtremeButtonMethod(UIfocusable trigger)
    {
        GeneralArtificerSpearAngles.Value = false;

        GourmandSpeedModifier.Value = 0.9f;
        GourmandEmptySpeedModifier.Value = 0.9f;
        GourmandCraftingNerfs.Value = true;
        GourmandSlideModifier.Value = 0.6f;
        GourmandSpearDamageModifier.Value = 0.6f;
        GourmandExhaustionModifier.Value = 1.5f;
        GourmandEmptyExhaustionModifier.Value = 1.3f;

        ArtificerSpeedModifier.Value = 0.8f;
        ArtificerThrowingAbility.Value = 1;
        ArtificerScavPopulationModifier.Value = 1.5f;
        ArtificerCraftingCost.Value = 3;
        ArtificerJumpModifier.Value = 0.5f;
        ArtificerJumpFoodLoss.Value = true;
        ArtificerJumpFoodCost.Value = 2;
        ArtificerJumpsPerCost.Value = 1;
        ArtificerJumpRequiresFood.Value = true;
        ArtificerExtraParryCost.Value = 5;
        ArtificerExplosionStunPyroCost.Value = 1.2f;
        ArtificerScavengerMeat.Value = 0;
        ArtificerThrowAngle.Value = 30;
        //ArtificerThrowAngleModifier;
        ArtificerBothEndings.Value = false;
        ArenaArtificerSpearSpeedModifier.Value = 0.5f;

        RivuletSpeedModifier.Value = 0.6f;
        RivuletThrowingAbility.Value = 0;
        RivuletSlideModifier.Value = 0.6f;
        RivuletJumpModifier.Value = 0.7f;
        RivuletCellSlowsCycles.Value = false;
        RivuletCellRainIntensity.Value = 1.5f;
        RivuletCellCycleModifier.Value = 0.4f;
        RivuletCellPrecycleChance.Value = 0.9f;
        RivuletRotGrabModifier.Value = 1f;

        SpearmasterSpeedModifier.Value = 0.8f;
        SpearmasterThrowingAbility.Value = 1;
        SpearmasterSpearsUntilCost.Value = 1;
        SpearmasterPipsPerSpearCost.Value = 2;
        SpearmasterGrowSpearExhaustion.Value = 1.2f;
        SpearmasterThrowSpearExhaustion.Value = 0.8f;
        SpearmasterBiteDeathChanceModifier.Value = 2.5f;
        SpearmasterJumpModifier.Value = 0.9f;
        SpearmasterGrowSpeedModifier.Value = 0.7f;
        SpearmasterNeedleDamageModifier.Value = 0.7f;

        SaintSpeedModifier.Value = 0.9f;
        SaintTongueModifier.Value = 0.4f;
        SaintTongueLengthModifier.Value = 0.5f;
        SaintSwingSpeedModifier.Value = 0.8f;
        SaintTongueSwingModifier.Value = 0.4f;
        SaintImproveTongueAim.Value = true;
        SaintSwallowedWarmthModifier.Value = 0.3f;
        SaintLanternWarmthModifier.Value = 0.4f;
        SaintObjectWarmthModifier.Value = 1.5f;
        SaintGodTimeModifier.Value = 0.4f;
        SaintRechargeRateModifier.Value = 0.7f;

        this.ShowConfigs();
    }

    private void OverpoweredButtonMethod(UIfocusable trigger)
    {
        GeneralArtificerSpearAngles.Value = true;

        GourmandSpeedModifier.Value = 1.5f;
        GourmandEmptySpeedModifier.Value = 1.3f;
        GourmandCraftingNerfs.Value = false;
        GourmandSlideModifier.Value = 2.0f;
        GourmandSpearDamageModifier.Value = 2.0f;
        GourmandExhaustionModifier.Value = 0.7f;
        GourmandEmptyExhaustionModifier.Value = 0.9f;

        ArtificerSpeedModifier.Value = 1.5f;
        ArtificerThrowingAbility.Value = 2;
        ArtificerScavPopulationModifier.Value = 2.0f;
        ArtificerCraftingCost.Value = 1;
        ArtificerJumpModifier.Value = 1.5f;
        ArtificerJumpFoodLoss.Value = false;
        ArtificerJumpFoodCost.Value = 0;
        ArtificerJumpsPerCost.Value = 0;
        ArtificerJumpRequiresFood.Value = false;
        ArtificerExtraParryCost.Value = 0;
        ArtificerExplosionStunPyroCost.Value = 0f;
        ArtificerScavengerMeat.Value = 6;
        ArtificerThrowAngle.Value = 45;
        //ArtificerThrowAngleModifier;
        ArtificerBothEndings.Value = true;
        ArenaArtificerSpearSpeedModifier.Value = 1.5f;

        RivuletSpeedModifier.Value = 1.5f;
        RivuletThrowingAbility.Value = 2;
        RivuletSlideModifier.Value = 1.5f;
        RivuletJumpModifier.Value = 1.5f;
        RivuletCellSlowsCycles.Value = true;
        RivuletCellRainIntensity.Value = 0.3f;
        RivuletCellCycleModifier.Value = 2.0f;
        RivuletCellPrecycleChance.Value = 0.1f;
        RivuletRotGrabModifier.Value = 0.1f;

        SpearmasterSpeedModifier.Value = 1.5f;
        SpearmasterThrowingAbility.Value = 2;
        SpearmasterSpearsUntilCost.Value = 0;
        SpearmasterPipsPerSpearCost.Value = 0;
        SpearmasterGrowSpearExhaustion.Value = 0f;
        SpearmasterThrowSpearExhaustion.Value = 0f;
        SpearmasterBiteDeathChanceModifier.Value = 0.5f;
        SpearmasterJumpModifier.Value = 1.2f;
        SpearmasterGrowSpeedModifier.Value = 1.5f;
        SpearmasterNeedleDamageModifier.Value = 1.2f;

        SaintSpeedModifier.Value = 1.5f;
        SaintTongueModifier.Value = 1.5f;
        SaintTongueLengthModifier.Value = 1.2f;
        SaintSwingSpeedModifier.Value = 2.0f;
        SaintTongueSwingModifier.Value = 1.3f;
        SaintImproveTongueAim.Value = true;
        SaintSwallowedWarmthModifier.Value = 1.5f;
        SaintLanternWarmthModifier.Value = 2.0f;
        SaintObjectWarmthModifier.Value = 0.5f;
        SaintGodTimeModifier.Value = 2.0f;
        SaintRechargeRateModifier.Value = 1.5f;

        this.ShowConfigs();
    }

}